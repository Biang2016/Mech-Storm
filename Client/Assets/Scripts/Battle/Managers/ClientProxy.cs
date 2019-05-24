using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class BattleClientProxy : ProxyBase
{
    protected Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    protected Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    private bool clientVersionValid = false;
    private string username = "";

    internal string UserName
    {
        get { return username; }
    }

    public BattleClientProxy(Socket socket, string serverVersion, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId, serverVersion);
        SendMessage(request);
    }

    internal GameManager BattleGameManager;
    internal BattlePlayer MyPlayer;
    internal BattlePlayer EnemyPlayer;

    internal void InitGameInfo()
    {
        MyPlayer = BattleGameManager.GetPlayerByClientId(ClientId);
        EnemyPlayer = MyPlayer.MyEnemyPlayer;
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;

        BattleGameManager?.OnLeaveGame(ClientId); //先结束对应的游戏

        Socket?.Close();

        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();

        isClosed = true;
    }

    public virtual void SendMessage(ServerRequestBase request)
    {
        if (isClosed) return;
        SendRequestsQueue.Enqueue(request);
        if (SendRequestsQueue.Count > 0)
        {
            try
            {
                SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue(), ClientId);
                DoSendToClient(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public delegate void DoSendToClientDelegate(SendMsg msg);

    public DoSendToClientDelegate DoSendToClient;

    public virtual void ReceiveMessage(ClientRequestBase request)
    {
        if (isClosed) return;
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    internal ResponseBundleBase CurrentClientRequestResponseBundle;

    /// <summary>
    /// Dispose of all request received before game start
    /// ServerGameManager will dispose of others during games.
    /// </summary>
    protected override void Response()
    {
        if (BattleGameManager != null && BattleGameManager.IsStopped)
        {
            BattleGameManager.StopGame();
        }

        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            if (ClientState == ClientStates.Playing)
            {
                if (BattleGameManager == null) return;

                try
                {
                    if (BattleGameManager.CurrentPlayer == BattleGameManager.GetPlayerByClientId(ClientId))
                    {
                        switch (r)
                        {
                            case WinDirectlyRequest req:
                                BattleGameManager?.OnWinDirectlyRequest(req, MyPlayer);
                                break;
                            case EndRoundRequest req:
                                BattleGameManager?.OnEndRoundRequest(req);
                                break;
                            case SummonMechRequest req:
                                BattleGameManager?.OnClientSummonMechRequest(req);
                                break;
                            case EquipWeaponRequest req:
                                BattleGameManager?.OnClientEquipWeaponRequest(req);
                                break;
                            case EquipShieldRequest req:
                                BattleGameManager?.OnClientEquipShieldRequest(req);
                                break;
                            case EquipPackRequest req:
                                BattleGameManager?.OnClientEquipPackRequest(req);
                                break;
                            case EquipMARequest req:
                                BattleGameManager?.OnClientEquipMARequest(req);
                                break;
                            case UseSpellCardRequest req:
                                BattleGameManager?.OnClientUseSpellCardRequest(req);
                                break;
                            case UseSpellCardToMechRequest req:
                                BattleGameManager?.OnClientUseSpellCardToMechRequest(req);
                                break;
                            case UseSpellCardToShipRequest req:
                                BattleGameManager?.OnClientUseSpellCardToShipRequest(req);
                                break;
                            case UseSpellCardToEquipRequest req:
                                BattleGameManager?.OnClientUseSpellCardToEquipRequest(req);
                                break;
                            case LeaveGameRequest req: //quit game normally
                                BattleGameManager?.OnLeaveGameRequest(req);
                                break;
                            case MechAttackMechRequest req:
                                BattleGameManager?.OnClientMechAttackMechRequest(req);
                                break;
                            case MechAttackShipRequest req:
                                BattleGameManager?.OnClientMechAttackShipRequest(req);
                                break;
                        }
                    }
                    else
                    {
                        switch (r)
                        {
                            case LeaveGameRequest req: //quit game normally
                                BattleGameManager?.OnLeaveGameRequest(req);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    BattleLog.Instance.Log.PrintError(e.ToString());

                    if (BattleGameManager != null && !BattleGameManager.IsStopped)
                    {
                        BattleGameManager.OnEndGameByServerError();
                        BattleGameManager.StopGame();
                    }

                    BattleGameManager = null;
                }
            }
        }
    }
}
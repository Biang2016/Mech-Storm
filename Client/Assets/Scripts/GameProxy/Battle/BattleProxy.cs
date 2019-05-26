using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class BattleProxy
{
    public ILog DebugLog => Battle.DebugLog;
    public SendMessageDelegate SendMessage;

    public int ClientID;
    public BuildInfo BuildInfo;
    public string UserName;

    public BattleProxy(int clientId, string userName, BuildInfo buildInfo, SendMessageDelegate sendMessage)
    {
        UserName = userName;
        BuildInfo = buildInfo;
        ClientID = clientId;
        SendMessage = sendMessage;
    }

    public ProxyBase.ClientStates ClientState;

    internal Battle Battle;
    internal GameManager BattleGameManager;
    internal BattlePlayer MyPlayer;
    internal BattlePlayer EnemyPlayer;

    internal void InitGameInfo()
    {
        MyPlayer = BattleGameManager.GetPlayerByClientId(ClientID);
        EnemyPlayer = MyPlayer.MyEnemyPlayer;
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;
        BattleGameManager?.OnLeaveGame(ClientID); //先结束对应的游戏
        isClosed = true;
    }

    internal ResponseBundleBase CurrentClientRequestResponseBundle;

    /// <summary>
    /// Dispose of all request received before game start
    /// ServerGameManager will dispose of others during games.
    /// </summary>
    public void Response(ClientRequestBase r)
    {
        if (isClosed) return;
        if (BattleGameManager != null && BattleGameManager.IsStopped)
        {
            BattleGameManager.StopGame();
        }

        if (ClientState == ProxyBase.ClientStates.Playing)
        {
            if (BattleGameManager == null) return;

            try
            {
                if (BattleGameManager.CurrentPlayer == BattleGameManager.GetPlayerByClientId(ClientID))
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
                DebugLog.PrintError(e.ToString());

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
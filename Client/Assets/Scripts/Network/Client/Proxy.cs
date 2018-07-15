using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class Proxy : ProxyBase
{
    private Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();
    private Queue<ServerRequestBase> ReceiveRequestsQueue = new Queue<ServerRequestBase>();

    public override ClientStates ClientState
    {
        get { return clientState; }
        set
        {
            clientState = value;
            ClientLog.CL.PrintClientStates("Client states: " + ClientState);
        }
    }

    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }

    #region 收发基础组件

    public void Send() //每帧调用
    {
        if (SendRequestsQueue.Count > 0)
        {
            ClientRequestBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(Client.CS.Send);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public void SendMessage(ClientRequestBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    public void ReceiveMessage(ServerRequestBase request)
    {
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    #endregion

    protected override void Response()
    {
        ServerRequestBase r = ReceiveRequestsQueue.Dequeue();
        if (r is ClientIdRequest)
        {
            ClientIdRequest request = (ClientIdRequest) r;
            ClientId = request.givenClientId;
            ClientState = ClientStates.GetId;
        }
        else if (r is PlayerRequest)
        {
            ClientState = ClientStates.Playing;
            NetworkManager.NM.SuccessMatched();
            RoundManager.RM.Initialize();
            RoundManager.RM.InitializePlayers((PlayerRequest) r);
        }
        else if (r is PlayerTurnRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.SetPlayerTurn, r));
        }
        else if (r is PlayerCostRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.SetPlayersCost, r));
        }
        else if (r is DrawCardRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnPlayerDrawCard, r));
        }
        else if (r is SummonRetinueRequest_Response)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnPlayerSummonRetinue, r));
        }
        else if (r is EquipWeaponRequest_Response)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnPlayerEquipWeapon, r));
        }
        else if (r is EquipShieldRequest_Response)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnPlayerEquipShield, r));
        }
        else if (r is RetinueAttackRetinueRequest_Response)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnRetinueAttackRetinue, r));
        }
        else if (r is WeaponAttributesRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnWeaponAttributesChange, r));
        }
        else if (r is RetinueAttributesRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnRetinueAttributesChange, r));
        }
        else if (r is ShieldAttributesRequest)
        {
            BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(RoundManager.RM.OnShieldAttributesChange, r));
        }
        else if (r is GameStopByLeaveRequest)
        {
            GameStopByLeaveRequest request = (GameStopByLeaveRequest) r;
            if (request.clientId == ClientId)
            {
                ClientLog.CL.PrintClientStates("你 " + request.clientId + " 退出了比赛");
            }
            else
            {
                ClientLog.CL.PrintReceive("你的对手 " + request.clientId + " 退出了比赛");
            }

            RoundManager.RM.StopGame();
            ClientState = ClientStates.SubmitCardDeck;
        }
    }

    public void CancelMatch()
    {
        CancelMatchRequest request = new CancelMatchRequest(ClientId);
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void LeaveGame()
    {
        LeaveGameRequest request = new LeaveGameRequest();
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
    }


    public void OnSendCardDeck(CardDeckInfo cardDeckInfo)
    {
        CardDeckRequest req = new CardDeckRequest(ClientId, cardDeckInfo);
        SendMessage(req);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void OnBeginMatch()
    {
        MatchRequest req = new MatchRequest(ClientId);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class Proxy : ProxyBase
{
    private Queue<ClientRequestBaseBase> SendRequestsQueue = new Queue<ClientRequestBaseBase>();
    private Queue<ServerRequestBaseBase> ReceiveRequestsQueue = new Queue<ServerRequestBaseBase>();

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
            ClientRequestBaseBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(Client.CS.Send);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public void SendMessage(ClientRequestBaseBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    public void ReceiveMessage(ServerRequestBaseBase request)
    {
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    #endregion

    protected override void Response()
    {
        ServerRequestBaseBase r = ReceiveRequestsQueue.Dequeue();
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
            RoundManager.RM.SetPlayerTurn((PlayerTurnRequest) r);
        }
        else if (r is PlayerCostRequest)
        {
            RoundManager.RM.SetPlayersCost((PlayerCostRequest) r);
        }
        else if (r is DrawCardRequest)
        {
            RoundManager.RM.OnPlayerDrawCard((DrawCardRequest) r);
        }
        else if (r is SummonRetinueRequest_Response)
        {
            RoundManager.RM.OnPlayerSummonRetinue((SummonRetinueRequest_Response) r);
        }
        else if (r is EquipWeaponRequest_Response)
        {
            RoundManager.RM.OnPlayerEquipWeapon((EquipWeaponRequest_Response) r);
        }
        else if (r is EquipShieldRequest_Response)
        {
            RoundManager.RM.OnPlayerEquipShield((EquipShieldRequest_Response) r);
        }
        else if (r is RetinueAttackRetinueRequest_Response)
        {
            RoundManager.RM.OnRetinueAttackRetinue((RetinueAttackRetinueRequest_Response) r);
        }
        else if (r is WeaponAttributesRequest)
        {
            RoundManager.RM.OnWeaponAttributesChange((WeaponAttributesRequest) r);
        }
        else if (r is RetinueAttributesRequest)
        {
            RoundManager.RM.OnRetinueAttributesChange((RetinueAttributesRequest) r);
        }
        else if (r is ShieldAttributesRequest)
        {
            RoundManager.RM.OnShieldAttributesChange((ShieldAttributesRequest) r);
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
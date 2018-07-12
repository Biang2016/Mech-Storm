using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

internal class Proxy : ProxyBase
{
    public Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();
    public Queue<ServerRequestBase> ReceiveRequestsQueue = new Queue<ServerRequestBase>();



    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }

    public void OnSendCardDeck(CardDeckInfo cardDeckInfo)
    {
        if (Client.CS.Proxy.ClientState == ClientStates.GetId)
        {
            CardDeckRequest req = new CardDeckRequest(ClientId, cardDeckInfo);
            Client.CS.SendMessage(req);
            Client.CS.Proxy.ClientState = ClientStates.SubmitCardDeck;
            ClientLog.CL.PrintClientStates("Client states: " + Client.CS.Proxy.ClientState);
        }
        else
        {
            ClientLog.CL.PrintWarning("请连接服务器");
        }
    }

    public void OnBeginMatch()
    {
        if (Client.CS.Proxy.ClientState == ClientStates.SubmitCardDeck)
        {
            MatchRequest req = new MatchRequest(ClientId);
            Client.CS.SendMessage(req);
            Client.CS.Proxy.ClientState = ClientStates.Matching;
            ClientLog.CL.PrintClientStates("Client states: " + Client.CS.Proxy.ClientState);
        }
        else
        {
            ClientLog.CL.PrintWarning("请发送卡组");
        }
    }

    public override void Response()
    {
        ServerRequestBase r = ReceiveRequestsQueue.Dequeue();
        if (r is ServerWarningRequest) //接收服务器回执警告
        {
        }
        else if (r is ClientIdRequest)
        {
            if (Client.CS.Proxy.ClientState == ClientStates.Nothing)
            {
                ClientIdRequest request = (ClientIdRequest) r;
                ClientId = request.givenClientId;
                ClientState = ClientStates.GetId;
                ClientLog.CL.PrintClientStates("Client states: " + ClientState);
            }
            else
            {
                ClientLog.CL.PrintError("请重置游戏");
            }
        }
        else if (r is PlayerRequest)
        {
            RoundManager.RM.Initialize();
            RoundManager.RM.InitializePlayers((PlayerRequest) r);
        }
        else if (r is PlayerTurnRequest)
        {
            RoundManager.RM.SetPlayerTurn((PlayerTurnRequest) r);
        }
        else if (r is DrawCardRequest)
        {
            RoundManager.RM.OnPlayerDrawCard((DrawCardRequest) r);
        }else if (r is SummonRetinueRequest_Response)
        {
            RoundManager.RM.OnPlayerSummonRetinue((SummonRetinueRequest_Response) r);
        }
    }


    public void ReSetClient()
    {
        Client.CS.Proxy.ClientState = ClientStates.Nothing;
        ResetClientRequest request = new ResetClientRequest();
        Client.CS.SendMessage(request);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

public class Proxy : ProxyBase
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
        if (r is ServerInfoRequest) //接收服务器回执信息
        {
            ServerInfoRequest request = (ServerInfoRequest) r;
            if (request.infoNumber == InfoNumbers.INFO_SEND_CLIENT_CARDDECK_SUC) ClientState = ClientStates.SubmitCardDeck;
            if (request.infoNumber == InfoNumbers.INFO_IS_MATCHING) ClientState = ClientStates.Matching;
        }
        else if (r is ServerWarningRequest) //接收服务器回执警告
        {
        }
        else if (r is ClientIdRequest)
        {
            ClientIdRequest request = (ClientIdRequest) r;
            ClientId = request.givenClientId;
            ClientState = ClientStates.GetId;
            ClientLog.CL.PrintClientStates("Client states: " + ClientState);
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
        }
    }


}
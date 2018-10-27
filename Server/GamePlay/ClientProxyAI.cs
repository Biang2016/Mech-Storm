using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

/// <summary>
/// 为了兼容联机对战模式，单人模式的AI也用一个ClientProxy来处理逻辑
/// AI不需要任何Socket有关的功能
/// </summary>
internal class ClientProxyAI : ClientProxy
{
    public override ClientStates ClientState
    {
        get => clientState;
        set => clientState = value;
    }

    public ClientProxyAI(int clientId, bool isStopReceive) : base(null, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    public override void SendMessage(ServerRequestBase request)
    {
    }

    public override void ReceiveMessage(ClientRequestBase request)
    {
    }

    protected override void Response()
    {
    }
}
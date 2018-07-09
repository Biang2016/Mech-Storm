using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ClientProxy
{
    //发包读包基础
    public Socket Socket;
    public int ClientId;
    public DataHolder DataHolder = new DataHolder();
    public bool IsStopReceive;

    ////收到请求队列
    //public Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    //public Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public int ClientStates;
    public ServerGameMatchManager MyServerGameMatchManager;
    public ServerGameManager MyServerGameManager;

    public ClientProxy(Socket socket, int clientId, bool isStopReceive)
    {
        Socket = socket;
        ClientId = clientId;
        IsStopReceive = isStopReceive;
        ClientIdRequest request = new ClientIdRequest(clientId);
        SendRequestsQueue.Enqueue(request);
    }

    public virtual void Response()
    {
        //while (ReceiveRequestsQueue.Count>0)
        //{
        //    ClientRequestBase request = (ClientRequestBase) ReceiveRequestsQueue.Dequeue();
        //    if(request is )
        //}
    }



}
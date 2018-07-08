using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Server
{
    private static Server _sv;

    public static Server SV
    {
        get
        {
            if (_sv == null)
            {
                _sv = new Server("127.0.0.1", 9999);
            }

            return _sv;
        }
        set { _sv = value; }
    }

    public string IP;
    public int Port;
    private Socket severSocket;
    private List<ClientData> clientList = new List<ClientData>();
    private Dictionary<int, Socket> socketDict = new Dictionary<int, Socket>();

    private Queue<ClientData> receiveDataQueue = new Queue<ClientData>();
    private Dictionary<int, Queue<Request>> sendDataQueueDict = new Dictionary<int, Queue<Request>>();

    public ProtoManager ServerProtoManager;
    private ServerGameMatchManager sgmm;

    public Server(string ip, int port)
    {
        IP = ip;
        Port = port;
    }

    public void Start()
    {
        AllCards tmp = AllCards.AC;
        OnRestartProtocols();
        sgmm = new ServerGameMatchManager();
        StartSeverSocket();
        Thread threadReceiveAndSend = new Thread(ReceiveAndSendMsg);
        threadReceiveAndSend.IsBackground = true;
        threadReceiveAndSend.Start();
    }

    void Update()
    {
        if (receiveDataQueue.Count > 0)
        {
            ServerProtoManager.TryDeserialize(receiveDataQueue.Dequeue());
        }

        foreach (KeyValuePair<int, Queue<Request>> kv in sendDataQueueDict)
        {
            SendQueueToClient(kv.Key);
        }
    }

    private void ReceiveAndSendMsg()
    {
        while (true)
        {
            Update();
        }
    }

    public void OnRestartProtocols()
    {
        ServerProtoManager = new ProtoManager();
        ServerProtoManager.AddProtocol<TestConnectRequest>(NetProtocols.TEST_CONNECT);
        ServerProtoManager.AddProtocol<ClientIdRequest>(NetProtocols.SEND_CLIENT_ID);
        ServerProtoManager.AddProtocol<ServerInfoRequest>(NetProtocols.INFO_NUMBER);
        ServerProtoManager.AddProtocol<ServerWarningRequest>(NetProtocols.WARNING_NUMBER);
        ServerProtoManager.AddProtocol<GameBeginRequest>(NetProtocols.GAME_BEGIN);
        ServerProtoManager.AddProtocol<PlayerRequest>(NetProtocols.PLAYER);
        ServerProtoManager.AddProtocol<PlayerCostRequest>(NetProtocols.PLAYER_COST_CHANGE);
        ServerProtoManager.AddProtocol<DrawCardRequest>(NetProtocols.DRAW_CARD);
        ServerProtoManager.AddProtocol<SummonRetinueRequest>(NetProtocols.SUMMON_RETINUE);
        ServerProtoManager.AddProtocol<PlayerTurnRequest>(NetProtocols.PLAYER_TURN);
        ServerProtoManager.AddProtocol<ClientEndRoundRequest>(NetProtocols.CLIENT_END_ROUND);
        ServerProtoManager.AddProtocol<CardDeckRequest>(NetProtocols.CARD_DECK_INFO);
        ServerProtoManager.AddRequestDelegate(NetProtocols.TEST_CONNECT, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.SEND_CLIENT_ID, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.INFO_NUMBER, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.WARNING_NUMBER, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.GAME_BEGIN, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.PLAYER, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.PLAYER_COST_CHANGE, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.DRAW_CARD, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.SUMMON_RETINUE, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.PLAYER_TURN, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.CLIENT_END_ROUND, Response);
        ServerProtoManager.AddRequestDelegate(NetProtocols.CARD_DECK_INFO, Response);
    }

    public void Stop()
    {
        foreach (ClientData clientData in clientList)
        {
            if (clientData.Socket != null && clientData.Socket.Connected)
            {
                clientData.Socket.Shutdown(SocketShutdown.Both);
                clientData.Socket.Close();
                ServerLog.Print("客户" + clientData.ClientId + "退出");
            }
        }

        clientList.Clear();
    }

    public void StartSeverSocket()
    {
        try
        {
            severSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将服务器的ip捆绑
            severSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
            //为服务器sokect添加监听
            severSocket.Listen(10);
            ServerLog.Print("[S]服务器启动成功");
            clientList = new List<ClientData>();
            //开始服务器时 一般接受一个服务就会被挂起所以要用多线程来解决
            Thread threadAccept = new Thread(Accept);
            threadAccept.IsBackground = true;
            threadAccept.Start();
        }
        catch
        {
            ServerLog.Print("[S]创建服务器失败");
        }
    }

    public void Accept()
    {
        Socket client = severSocket.Accept();
        ClientData clientData = new ClientData(client, 0, new DataHolder(), false);
        clientList.Add(clientData);
        IPEndPoint point = client.RemoteEndPoint as IPEndPoint;
        ServerLog.Print(point.Address + ":【" + point.Port + "】连接成功");
        ServerLog.Print("[S]客户端总数:" + clientList.Count);

        Thread threadReceive = new Thread(ReceiveSocket);
        threadReceive.IsBackground = true;
        threadReceive.Start(clientData);
        Accept();
    }

    #region 接收

    //接收客户端Socket连接请求
    private void ReceiveSocket(object obj)
    {
        ClientData clientData = obj as ClientData;
        clientData.DataHolder.Reset();
        while (!clientData.IsStopReceive)
        {
            if (!clientData.Socket.Connected)
            {
                //与客户端连接失败跳出循环  
                ServerLog.Print("[S]连接客户端失败,客户端ID" + clientData.ClientId);
                clientData.Socket.Close();
                break;
            }

            try
            {
                byte[] bytes = new byte[1024];
                int i = clientData.Socket.Receive(bytes);
                if (i <= 0)
                {
                    clientData.Socket.Close();
                    ServerLog.Print("[S]客户端关闭,客户端ID" + clientData.ClientId);
                    break;
                }

                clientData.DataHolder.PushData(bytes, i);
                while (clientData.DataHolder.IsFinished())
                {
                    receiveDataQueue.Enqueue(clientData);
                    clientData.DataHolder.RemoveFromHead();
                }
            }
            catch (Exception e)
            {
                ServerLog.Print("[S]Failed to ServerSocket error." + e);
                clientData.Socket.Close();
                break;
            }
        }
    }

    void Response(Socket socket, Request r)
    {
        string Log = "";
        Log += "服务器收到信息：[协议]" + r.GetProtocolName() + "[内容]";
        Log += r.DeserializeLog();
        ServerLog.Print(Log);

        if (r is ClientIdRequest)
        {
            ClientIdRequest resp = (ClientIdRequest)r;
            if (resp.purpose == ClientIdPurpose.RegisterClientId)
            {
                if (!socketDict.ContainsKey(resp.clientId))
                {
                    socketDict.Add(resp.clientId, socket);
                }
                else if (socketDict[resp.clientId] != socket)
                {
                    ServerWarningRequest request = new ServerWarningRequest(WarningNumbers.WARNING_NO_CLIENT_ID_EXISTED);
                    SendMessageToSocket(request, socket);
                }
                if (!sendDataQueueDict.ContainsKey(resp.clientId))
                {
                    sendDataQueueDict.Add(resp.clientId, new Queue<Request>());
                }
                sgmm.OnClientRegister(resp.clientId);
            }
            else if (resp.purpose == ClientIdPurpose.MatchGames)
            {
                sgmm.OnClientMatchGames(resp.clientId);
            }
        }
        else if (r is CardDeckRequest)
        {
            CardDeckRequest req = (CardDeckRequest)r;
            sgmm.OnReceiveCardDeckInfo(req.clientId, req.cardDeckInfo);
        }
        else if (r is ClientEndRoundRequest)
        {
            ClientEndRoundRequest request = (ClientEndRoundRequest)r;
            sgmm.PlayerGamesDictionary[request.clientId].OnEndRound();
        }
    }

    #endregion


    #region 发送信息

    //将要发送的信息送入队列
    public void SendMessageToClientId(Request req, int clientId)
    {
        //if (sendDataQueueDict.ContainsKey(clientId))
        //{
            sendDataQueueDict[clientId].Enqueue(req);
        //}
        //else
        //{
        //    ServerLog.Print("发送消息失败，客户端未连接到服务器,客户端ID" + clientId);
        //}
    }

    private void SendMessageToSocket(Request req, Socket socket)
    {
        Thread threadSend = new Thread(DoSendToClient);
        threadSend.IsBackground = true;
        threadSend.Start(new SendMsg(socket, req));
    }

    //处理特定客户端的信息队列
    private void SendQueueToClient(int clientId)
    {
        if (sendDataQueueDict.ContainsKey(clientId))
        {
            while (sendDataQueueDict[clientId].Count > 0)
            {
                Request req = sendDataQueueDict[clientId].Dequeue();
                Thread threadSend = new Thread(DoSendToClient);
                threadSend.IsBackground = true;
                threadSend.Start(new SendMsg(socketDict[clientId], req));
            }
        }
        else
        {
            ServerLog.Print("发送消息失败，客户端未连接到服务器,客户端ID" + clientId);
        }
    }


    //对特定客户端发送信息
    private void DoSendToClient(object obj)
    {
        SendMsg sendMsg = (SendMsg)obj;

        if (sendMsg == null)
        {
            ServerLog.Print("[S]sendMsg is null");
            return;
        }

        if (sendMsg.Client == null)
        {
            ServerLog.Print("[S]client socket is null");
            return;
        }

        if (!sendMsg.Client.Connected)
        {
            ServerLog.Print("[S]Not connected to client socket");
            sendMsg.Client.Close();
            return;
        }

        try
        {
            string log = "";
            log += "[S]发送信息给" + sendMsg.Client.RemoteEndPoint + "：[协议]" + sendMsg.Req.GetProtocolName() + "[内容]";
            log += sendMsg.Req.DeserializeLog();
            ServerLog.Print(log);

            DataStream bufferWriter = new DataStream(true);
            sendMsg.Req.Serialize(bufferWriter);
            byte[] msg = bufferWriter.ToByteArray();

            byte[] buffer = new byte[msg.Length + 4];
            DataStream writer = new DataStream(buffer, true);

            writer.WriteInt32((uint)msg.Length); //增加数据长度
            writer.WriteRaw(msg);

            byte[] data = writer.ToByteArray();
            IAsyncResult asyncSend = sendMsg.Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), sendMsg.Client);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                Stop();
            }
        }
        catch (Exception e)
        {
            ServerLog.Print("[S]Send error : " + e.ToString());
        }
    }

    private void SendCallback(IAsyncResult asyncConnect)
    {
        ServerLog.Print("[S]发送信息成功");
    }

    #endregion
}


public class SendMsg
{
    public SendMsg(Socket client, Request req)
    {
        Client = client;
        Req = req;
    }

    public Socket Client;
    public Request Req;
}
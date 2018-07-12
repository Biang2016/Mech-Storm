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

    private Server()
    {
    }

    public Server(string ip, int port)
    {
        IP = ip;
        Port = port;
    }

    public string IP;
    public int Port;
    private Socket severSocket;
    public ProtoManager ServerProtoManager;
    private Dictionary<int, ClientProxy> ClientsDict = new Dictionary<int, ClientProxy>();
    private Queue<ReceiveSocketData> receiveDataQueue = new Queue<ReceiveSocketData>();

    public ServerGameMatchManager SGMM;


    public void Start()
    {
        AllCards.AddAllCards();
        ServerLog.PrintServerStates("CardDeck Loaded");
        SGMM = new ServerGameMatchManager();

        OnRestartProtocols();
        StartSeverSocket();

        Thread threadReceive = new Thread(ReceiveMessage);
        threadReceive.IsBackground = true;
        threadReceive.Start();
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            if (receiveDataQueue.Count > 0)
            {
                ReceiveSocketData rsd = receiveDataQueue.Dequeue();
                ServerProtoManager.TryDeserialize(rsd.Data, rsd.Socket);
            }
        }
    }

    public void OnRestartProtocols()
    {
        ServerProtoManager = new ProtoManager();
        foreach (System.Reflection.FieldInfo fi in typeof(NetProtocols).GetFields())
        {
            ServerProtoManager.AddRequestDelegate((int) fi.GetRawConstantValue(), Response);
        }
    }

    public void Stop()
    {
        foreach (KeyValuePair<int, ClientProxy> kv in ClientsDict)
        {
            ClientProxy clientProxy = kv.Value;
            if (clientProxy.Socket != null && clientProxy.Socket.Connected)
            {
                clientProxy.Socket.Shutdown(SocketShutdown.Both);
                clientProxy.Socket.Close();
                ServerLog.PrintClientStates("客户" + clientProxy.ClientId + "退出");
            }
        }

        ClientsDict.Clear();
    }

    public void StartSeverSocket()
    {
        try
        {
            severSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将服务器的ip捆绑
            severSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
            //为服务器sokect添加监听
            severSocket.Listen(200);
            ServerLog.PrintServerStates("------------------ Server Start ------------------\n");
            //开始服务器时 一般接受一个服务就会被挂起所以要用多线程来解决
            Thread threadAccept = new Thread(Accept);
            threadAccept.IsBackground = true;
            threadAccept.Start();
        }
        catch
        {
            ServerLog.Print("Server start failed!");
        }
    }

    private int clientIdGenerator = 0;

    int GenerateClientId()
    {
        return clientIdGenerator++;
    }

    public void Accept()
    {
        Socket socket = severSocket.Accept();
        int clientId = GenerateClientId();
        ClientProxy clientProxy = new ClientProxy(socket, clientId, false);
        ClientsDict.Add(clientId, clientProxy);
        IPEndPoint point = socket.RemoteEndPoint as IPEndPoint;
        ServerLog.PrintClientStates(point.Address + ":" + point.Port + " connected. Client count: " + ClientsDict.Count);

        Thread threadReceive = new Thread(ReceiveSocket);
        threadReceive.IsBackground = true;
        threadReceive.Start(clientProxy);
        Accept();
    }

    #region 接收

    //接收客户端Socket连接请求
    private void ReceiveSocket(object obj)
    {
        ClientProxy clientProxy = obj as ClientProxy;
        clientProxy.DataHolder.Reset();
        while (!clientProxy.IsStopReceive)
        {
            if (!clientProxy.Socket.Connected)
            {
                //与客户端连接失败跳出循环  
                ServerLog.PrintClientStates("连接客户端失败,ID: " + clientProxy.ClientId + " IP: " + clientProxy.Socket.RemoteEndPoint);
                clientProxy.Socket.Close();
                break;
            }

            try
            {
                byte[] bytes = new byte[1024];
                int i = clientProxy.Socket.Receive(bytes);
                if (i <= 0)
                {
                    clientProxy.Socket.Close();
                    ServerLog.PrintClientStates("客户端关闭,ID: " + clientProxy.ClientId + " IP: " + clientProxy.Socket.RemoteEndPoint);
                    break;
                }

                clientProxy.DataHolder.PushData(bytes, i);
                while (clientProxy.DataHolder.IsFinished())
                {
                    ReceiveSocketData rsd = new ReceiveSocketData(clientProxy.Socket, clientProxy.DataHolder.mRecvData);
                    receiveDataQueue.Enqueue(rsd);
                    clientProxy.DataHolder.RemoveFromHead();
                }
            }
            catch (Exception e)
            {
                ServerLog.PrintError("Failed to ServerSocket error,ID: " + clientProxy.ClientId);
                clientProxy.Socket.Close();
                break;
            }
        }
    }

    void Response(Socket socket, Request r)
    {
        //统一日志打出
        ServerLog.PrintReceive("GetFrom    " + socket.RemoteEndPoint + "    [" + r.GetProtocolName() + "]    " + r.DeserializeLog());

        if (r is ClientRequestBase)
        {
            ClientRequestBase request = (ClientRequestBase) r;
            if (ClientsDict.ContainsKey(request.clientId))
            {
                ClientsDict[request.clientId].ReceiveRequestsQueue.Enqueue(request);
                ClientsDict[request.clientId].Response();
            }
        }
    }

    #endregion


    #region 发送信息

    //对特定客户端发送信息
    public void DoSendToClient(object obj)
    {
        SendMsg sendMsg = (SendMsg) obj;
        if (sendMsg == null)
        {
            ServerLog.PrintError("SendMsg is null");
            return;
        }

        if (sendMsg.Client == null)
        {
            ServerLog.PrintError("Client socket is null");
            return;
        }

        if (!sendMsg.Client.Connected)
        {
            ServerLog.PrintError("Not connected to client socket");
            sendMsg.Client.Close();
            return;
        }

        try
        {
            string log = "";
            log += "SendTo    " + sendMsg.Client.RemoteEndPoint + "    [" + sendMsg.Req.GetProtocolName() + "]    ";
            log += sendMsg.Req.DeserializeLog();
            ServerLog.PrintSend(log);

            DataStream bufferWriter = new DataStream(true);
            sendMsg.Req.Serialize(bufferWriter);
            byte[] msg = bufferWriter.ToByteArray();

            byte[] buffer = new byte[msg.Length + 4];
            DataStream writer = new DataStream(buffer, true);

            writer.WriteInt32((uint) msg.Length); //增加数据长度
            writer.WriteRaw(msg);

            byte[] data = writer.ToByteArray();
            IAsyncResult asyncSend = sendMsg.Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), sendMsg.Client);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(500, true);
            if (!success)
            {
                Stop();
            }
        }
        catch (Exception e)
        {
            ServerLog.PrintError("Send error : " + e);
        }
    }

    private void SendCallback(IAsyncResult asyncConnect)
    {
        //ServerLog.Print("发送信息成功");
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

public struct ReceiveSocketData
{
    public Socket Socket;
    public byte[] Data;

    public ReceiveSocketData(Socket socket, byte[] data)
    {
        Socket = socket;
        Data = data;
    }
}
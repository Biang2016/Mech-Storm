using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

internal class Server
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
    private Socket SeverSocket;
    private ProtoManager ServerProtoManager;
    public Dictionary<int, ClientProxy> ClientsDict = new Dictionary<int, ClientProxy>();
    private Queue<ReceiveSocketData> ReceiveDataQueue = new Queue<ReceiveSocketData>();

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

    private void OnRestartProtocols()
    {
        ServerProtoManager = new ProtoManager();
        foreach (System.Reflection.FieldInfo fi in typeof(NetProtocols).GetFields())
        {
            ServerProtoManager.AddRequestDelegate((int) fi.GetRawConstantValue(), Response);
        }
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            if (ReceiveDataQueue.Count > 0)
            {
                ReceiveSocketData rsd = ReceiveDataQueue.Dequeue();
                ServerProtoManager.TryDeserialize(rsd.Data, rsd.Socket);
            }
        }
    }


    public void Stop()
    {
        foreach (KeyValuePair<int, ClientProxy> kv in ClientsDict)
        {
            ClientProxy clientProxy = kv.Value;
            if (clientProxy.Socket != null && clientProxy.Socket.Connected)
            {
                ServerLog.PrintClientStates("客户端 " + clientProxy.ClientId + " 退出");
                ClientProxyClose(clientProxy);
            }
        }

        ClientsDict.Clear();
    }

    /// <summary>
    /// 所有的客户端提前异常退出、正常退出都走此方法
    /// </summary>
    /// <param name="clientProxy"></param>
    public void ClientProxyClose(ClientProxy clientProxy)
    {
        clientProxy.OnClose();
        ClientsDict.Remove(clientProxy.ClientId);
    }

    public void StartSeverSocket()
    {
        try
        {
            SeverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将服务器的ip捆绑
            SeverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
            //为服务器sokect添加监听
            SeverSocket.Listen(200);
            ServerLog.PrintServerStates("------------------ Server Start ------------------\n");
            //开始服务器时 一般接受一个服务就会被挂起所以要用多线程来解决
            Thread threadAccept = new Thread(Accept);
            threadAccept.IsBackground = true;
            threadAccept.Start();
        }
        catch
        {
            ServerLog.PrintError("Server start failed!");
        }
    }

    private int clientIdGenerator = 0;

    int GenerateClientId()
    {
        return clientIdGenerator++;
    }

    public void Accept()
    {
        Socket socket = SeverSocket.Accept();
        int clientId = GenerateClientId();
        ClientProxy clientProxy = new ClientProxy(socket, clientId, false);
        ClientsDict.Add(clientId, clientProxy);
        IPEndPoint point = socket.RemoteEndPoint as IPEndPoint;
        ServerLog.PrintClientStates("新的客户端连接 " + point.Address + ":" + point.Port + "  客户端总数: " + ClientsDict.Count);

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
                ClientProxyClose(clientProxy);
                break;
            }

            try
            {
                byte[] bytes = new byte[1024];
                int i = clientProxy.Socket.Receive(bytes);
                if (i <= 0)
                {
                    ServerLog.PrintClientStates("客户端关闭,ID: " + clientProxy.ClientId + " IP: " + clientProxy.Socket.RemoteEndPoint);
                    ClientProxyClose(clientProxy);
                    break;
                }

                clientProxy.DataHolder.PushData(bytes, i);
                while (clientProxy.DataHolder.IsFinished())
                {
                    ReceiveSocketData rsd = new ReceiveSocketData(clientProxy.Socket, clientProxy.DataHolder.mRecvData);
                    ReceiveDataQueue.Enqueue(rsd);
                    clientProxy.DataHolder.RemoveFromHead();
                }
            }
            catch
            {
                ServerLog.PrintError("Failed to ServerSocket error,ID: " + clientProxy.ClientId);
                ClientProxyClose(clientProxy);
                break;
            }
        }
    }

    void Response(Socket socket, RequestBase r)
    {
        //统一日志打出
        ServerLog.PrintReceive("GetFrom    " + socket.RemoteEndPoint + "    [" + r.GetProtocolName() + "]    " + r.DeserializeLog());

        if (r is ClientRequestBaseBase)
        {
            ClientRequestBaseBase request = (ClientRequestBaseBase) r;
            if (ClientsDict.ContainsKey(request.clientId))
            {
                ClientsDict[request.clientId].ReceiveMessage(request);
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
            bool success = asyncSend.AsyncWaitHandle.WaitOne(1000, true);
            if (!success)
            {
                ServerLog.PrintError("发送失败");
            }
        }
        catch (Exception e)
        {
            ServerLog.PrintError("Send Exception : " + e);
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
    public SendMsg(Socket client, RequestBase req)
    {
        Client = client;
        Req = req;
    }

    public Socket Client;
    public RequestBase Req;
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
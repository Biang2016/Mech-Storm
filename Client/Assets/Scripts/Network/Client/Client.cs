using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    private static Client _cs;

    public static Client CS
    {
        get
        {
            if (!_cs) _cs = FindObjectOfType<Client>();
            return _cs;
        }
    }

    public ProtoManager ClientProtoManager;
    private Socket ServerSocket;
    bool isStopReceive = true;

    public delegate void ConnectCallback();

    ConnectCallback connectDelegate = null;
    ConnectCallback connectFailedDelegate = null;

    private ClientData clientData;
    Queue<ClientData> receiveDataQueue = new Queue<ClientData>();
    Queue<Request> sendDataQueue = new Queue<Request>();

    void Awake()
    {
        OnRestartProtocols();
    }

    //接收到数据放入数据队列，按顺序取出
    void Update()
    {
        if (receiveDataQueue.Count > 0)
        {
            Request request = ClientProtoManager.TryDeserialize(receiveDataQueue.Dequeue());
        }

        if (sendDataQueue.Count > 0)
        {
            Send();
        }
    }

    void OnDestroy()
    {
        Closed();
    }

    public void OnRestartProtocols()
    {
        ClientProtoManager = new ProtoManager();
        ClientProtoManager.AddProtocol<TestConnectRequest>(NetProtocols.TEST_CONNECT);
        ClientProtoManager.AddProtocol<ClientIdRequest>(NetProtocols.SEND_CLIENT_ID);
        ClientProtoManager.AddProtocol<ServerInfoRequest>(NetProtocols.INFO_NUMBER);
        ClientProtoManager.AddProtocol<ServerWarningRequest>(NetProtocols.WARNING_NUMBER);
        ClientProtoManager.AddProtocol<GameStateRequest>(NetProtocols.GAME_BEGIN);
        ClientProtoManager.AddProtocol<PlayerRequest>(NetProtocols.PLAYER);
        ClientProtoManager.AddProtocol<PlayerCostRequest>(NetProtocols.PLAYER_COST_CHANGE);
        ClientProtoManager.AddProtocol<DrawCardRequest>(NetProtocols.DRAW_CARD);
        ClientProtoManager.AddProtocol<SummonRetinueRequest>(NetProtocols.SUMMON_RETINUE);
        ClientProtoManager.AddProtocol<PlayerTurnRequest>(NetProtocols.PLAYER_TURN);
        ClientProtoManager.AddProtocol<ClientEndRoundRequest>(NetProtocols.CLIENT_END_ROUND);
        ClientProtoManager.AddProtocol<CardDeckRequest>(NetProtocols.CARD_DECK_INFO);
        ClientProtoManager.AddRequestDelegate(NetProtocols.TEST_CONNECT, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.SEND_CLIENT_ID, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.INFO_NUMBER, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.WARNING_NUMBER, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.GAME_BEGIN, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER_COST_CHANGE, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.DRAW_CARD, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.SUMMON_RETINUE, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER_TURN, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.CLIENT_END_ROUND, Response);
        ClientProtoManager.AddRequestDelegate(NetProtocols.CARD_DECK_INFO, Response);
    }

    #region 连接

    public void Connect(string serverIp, int serverPort, ConnectCallback connectCallback, ConnectCallback connectFailedCallback)
    {
        connectDelegate = connectCallback;
        connectFailedDelegate = connectFailedCallback;

        //采用TCP方式连接  
        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //服务器IP地址  
        IPAddress address = IPAddress.Parse(serverIp);

        //服务器端口  
        IPEndPoint endpoint = new IPEndPoint(address, serverPort);

        //异步连接,连接成功调用connectCallback方法  
        IAsyncResult result = ServerSocket.BeginConnect(endpoint, new AsyncCallback(ConnectedCallback), ServerSocket);

        //这里做一个超时的监测，当连接超过5秒还没成功表示超时  
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success)
        {
            //超时  
            Closed();
            if (connectFailedDelegate != null)
            {
                connectFailedDelegate();
            }
        }
        else
        {
            //与socket建立连接成功，开启线程接受服务端数据。  
            isStopReceive = false;
            clientData = new ClientData(null, 0, new DataHolder(), isStopReceive);
            Thread thread = new Thread(new ThreadStart(ReceiveSocket));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void ConnectedCallback(IAsyncResult asyncConnect)
    {
        if (!ServerSocket.Connected)
        {
            if (connectFailedDelegate != null)
            {
                connectFailedDelegate();
            }

            return;
        }

        if (connectDelegate != null)
        {
            connectDelegate();
        }
    }

    public bool isConnect()
    {
        return ServerSocket != null && ServerSocket.Connected;
    }

    //关闭Socket  
    public void Closed()
    {
        isStopReceive = true;

        if (ServerSocket != null && ServerSocket.Connected)
        {
            ServerSocket.Shutdown(SocketShutdown.Both);
            ClientLog.CL.PrintError("[C]Socket close");
            ServerSocket.Close();
        }

        ServerSocket = null;
    }

    #endregion

    #region 接收

    enum ClientStates
    {
        Connected = 1 << 0,
        Registered = 1 << 1,
        SubmitCardDeck = 1 << 2,
        Matching = 1 << 3,
        Playing = 1 << 4,
    }

    private int clientStates = 0;

    private void ReceiveSocket()
    {
        clientData.DataHolder.Reset();
        while (!isStopReceive)
        {
            if (!ServerSocket.Connected)
            {
                //与服务器断开连接跳出循环  
                ClientLog.CL.PrintError("[C]连接服务器失败.");
                ServerSocket.Close();
                break;
            }

            try
            {
                //接受数据保存至bytes当中  
                byte[] bytes = new byte[4096];
                //Receive方法中会一直等待服务端回发消息  
                //如果没有回发会一直在这里等着。  

                int i = ServerSocket.Receive(bytes);

                if (i <= 0)
                {
                    ServerSocket.Close();
                    ClientLog.CL.PrintError("[C]Socket.Close();");
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
                ClientLog.CL.PrintError("[C]Failed to clientSocket error." + e);
                ServerSocket.Close();
                break;
            }
        }
    }

    void Response(Socket socket, Request r)
    {
        string Log = "";
        Log += "Server：[" + r.GetProtocolName() + "]    ";
        Log += r.DeserializeLog();
        ClientLog.CL.PrintReceive(Log);
        if (r is ServerInfoRequest) //接收服务器回执信息
        {
            ServerInfoRequest request = (ServerInfoRequest) r;
            if (request.infoNumber == InfoNumbers.INFO_IS_CONNECT) clientStates |= (int) ClientStates.Connected;
            if (request.infoNumber == InfoNumbers.INFO_SEND_CLIENT_ID_SUC) clientStates |= (int) ClientStates.Registered;
            if (request.infoNumber == InfoNumbers.INFO_SEND_CLIENT_CARDDECK_SUC) clientStates |= (int) ClientStates.SubmitCardDeck;
            if (request.infoNumber == InfoNumbers.INFO_IS_MATCHING) clientStates |= (int) ClientStates.Matching;
            int tmp = clientStates;
            StringBuilder sb = new StringBuilder();
            while (tmp > 0)
            {
                sb.Append(tmp % 2);
                tmp >>= 1;
            }

            ClientLog.CL.PrintClientStates("Client states: " + sb.ToString());
        }
        else if (r is ServerWarningRequest) //接收服务器回执警告
        {
        }
        else if (r is GameStateRequest)
        {
            clientStates |= (int) ClientStates.Playing;
            RoundManager.RM.Initialize();
            ClientInfoRequest request = new ClientInfoRequest(NetworkManager.NM.SelfClientId, InfoNumbers.INFO_BEGIN_GAME);
            SendMessage(request);
        }
        else if (r is PlayerRequest)
        {
            PlayerRequest request = (PlayerRequest) r;
            RoundManager.RM.InitializePlayers(request);
        }
        else if (r is PlayerCostRequest)
        {
            PlayerCostRequest request = (PlayerCostRequest) r;
            RoundManager.RM.SetPlayersCost(request);
        }
        else if (r is PlayerTurnRequest)
        {
            PlayerTurnRequest request = (PlayerTurnRequest) r;
            RoundManager.RM.SetPlayerTurn(request);
        }
        else if (r is DrawCardRequest)
        {
            DrawCardRequest request = (DrawCardRequest) r;
            RoundManager.RM.OnPlayerDrawCard(request);
        }
        else if (r is SummonRetinueRequest)
        {
            SummonRetinueRequest request = (SummonRetinueRequest) r;
            RoundManager.RM.OnPlayerSummonRetinue(request);
        }
    }

    #endregion

    #region 发送

    public void SendMessage(Request req)
    {
        sendDataQueue.Enqueue(req);
    }

    private void Send()
    {
        if (ServerSocket == null)
        {
            ClientLog.CL.PrintError("[C]Server socket is null");
            return;
        }

        if (!ServerSocket.Connected)
        {
            ClientLog.CL.PrintError("[C]Not connected to server socket");
            Closed();
            return;
        }

        try
        {
            Request req = sendDataQueue.Dequeue();

            DataStream bufferWriter = new DataStream(true);
            req.Serialize(bufferWriter);
            byte[] msg = bufferWriter.ToByteArray();

            byte[] buffer = new byte[msg.Length + 4];
            DataStream writer = new DataStream(buffer, true);

            writer.WriteInt32((uint) msg.Length); //增加数据长度
            writer.WriteRaw(msg);

            byte[] data = writer.ToByteArray();

            IAsyncResult asyncSend = ServerSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), ServerSocket);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                Closed();
            }
        }
        catch (Exception e)
        {
            ClientLog.CL.PrintError("[C]Send error : " + e.ToString());
        }
    }

    private void SendCallback(IAsyncResult asyncConnect)
    {
        //ClientLog.CL.Print("[C]Send success");
    }

    #endregion
}
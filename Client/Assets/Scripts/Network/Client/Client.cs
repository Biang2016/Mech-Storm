﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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

    private DataHolder mDataHolder = new DataHolder();
    Queue<byte[]> receiveDataQueue = new Queue<byte[]>();
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
            Response response = ClientProtoManager.TryDeserialize(receiveDataQueue.Dequeue());
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
        ClientProtoManager.AddProtocol<TestConnectResponse>(NetProtocols.TEST_CONNECT);
        ClientProtoManager.AddProtocol<ClientIdResponse>(NetProtocols.SEND_CLIENT_ID);
        ClientProtoManager.AddProtocol<ServerInfoResponse>(NetProtocols.INFO_NUMBER);
        ClientProtoManager.AddProtocol<ServerWarningResponse>(NetProtocols.WARNING_NUMBER);
        ClientProtoManager.AddProtocol<GameBeginResponse>(NetProtocols.GAME_BEGIN);
        ClientProtoManager.AddProtocol<PlayerResponse>(NetProtocols.PLAYER);
        ClientProtoManager.AddProtocol<PlayerCostResponse>(NetProtocols.PLAYER_COST_CHANGE);
        ClientProtoManager.AddProtocol<DrawCardResponse>(NetProtocols.DRAW_CARD);
        ClientProtoManager.AddProtocol<SummonRetinueResponse>(NetProtocols.SUMMON_RETINUE);
        ClientProtoManager.AddProtocol<PlayerTurnResponse>(NetProtocols.PLAYER_TURN);
        ClientProtoManager.AddProtocol<ClientEndRoundResponse>(NetProtocols.CLIENT_END_ROUND);
        ClientProtoManager.AddProtocol<CardDeckResponse>(NetProtocols.CARD_DECK_INFO);
        ClientProtoManager.AddRespDelegate(NetProtocols.TEST_CONNECT, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.SEND_CLIENT_ID, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.INFO_NUMBER, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.WARNING_NUMBER, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.GAME_BEGIN, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.PLAYER, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.PLAYER_COST_CHANGE, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.DRAW_CARD, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.SUMMON_RETINUE, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.PLAYER_TURN, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.CLIENT_END_ROUND, Response);
        ClientProtoManager.AddRespDelegate(NetProtocols.CARD_DECK_INFO, Response);
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
            Debug.Log("[C]Socket close");
            ServerSocket.Close();
        }

        ServerSocket = null;
    }

    #endregion

    #region 接收

    private void ReceiveSocket()
    {
        mDataHolder.Reset();
        while (!isStopReceive)
        {
            if (!ServerSocket.Connected)
            {
                //与服务器断开连接跳出循环  
                Debug.Log("[C]Failed to clientSocket server.");
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
                    Debug.Log("[C]Socket.Close();");
                    break;
                }

                mDataHolder.PushData(bytes, i);

                while (mDataHolder.IsFinished())
                {
                    receiveDataQueue.Enqueue(mDataHolder.mRecvData);
                    mDataHolder.RemoveFromHead();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[C]Failed to clientSocket error." + e);
                ServerSocket.Close();
                break;
            }
        }
    }

    void Response(Response r)
    {
        string Log = "";
        Log += "收到服务器信息：[协议]" + r.GetProtocolName() + "[内容]";
        Log += r.DeserializeLog();
        Debug.Log(Log);
        if (r is GameBeginResponse)
        {
            RoundManager.RM.InitializeGame();
        }
        else if (r is PlayerResponse)
        {
            PlayerResponse resp = (PlayerResponse) r;
            RoundManager.RM.InitializePlayers(resp);
        }
        else if (r is PlayerCostResponse)
        {
            PlayerCostResponse resp = (PlayerCostResponse) r;
            RoundManager.RM.SetPlayersCost(resp);
        }
        else if (r is PlayerTurnResponse)
        {
            PlayerTurnResponse resp = (PlayerTurnResponse) r;
            RoundManager.RM.SetPlayerTurn(resp);
        }
        else if (r is DrawCardResponse)
        {
            DrawCardResponse resp = (DrawCardResponse) r;
            RoundManager.RM.OnPlayerDrawCard(resp);
        }
        else if (r is SummonRetinueResponse)
        {
            SummonRetinueResponse resp = (SummonRetinueResponse) r;
            RoundManager.RM.OnPlayerSummonRetinue(resp);
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
            Debug.Log("[C]Server socket is null");
            return;
        }

        if (!ServerSocket.Connected)
        {
            Debug.Log("[C]Not connected to server socket");
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
            Debug.Log("[C]Send error : " + e.ToString());
        }
    }

    private void SendCallback(IAsyncResult asyncConnect)
    {
        Debug.Log("[C]Send success");
    }

    #endregion
}
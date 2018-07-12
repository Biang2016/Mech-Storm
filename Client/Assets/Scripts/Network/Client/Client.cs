﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

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

    public Proxy Proxy;
    Queue<ReceiveSocketData> receiveDataQueue = new Queue<ReceiveSocketData>();

    void Awake()
    {
        OnRestartProtocols();
    }

    //接收到数据放入数据队列，按顺序取出
    void Update()
    {
        if (Proxy != null)
        {
            if (receiveDataQueue.Count > 0)
            {
                ReceiveSocketData rsd = receiveDataQueue.Dequeue();
                ClientProtoManager.TryDeserialize(rsd.Data, rsd.Socket);
            }

            if (Proxy.SendRequestsQueue.Count > 0)
            {
                ClientRequestBase request = Proxy.SendRequestsQueue.Dequeue();
                Thread thread =new Thread(Send);
                thread.IsBackground = true;
                thread.Start(request);
            }
        }
    }

    void OnDestroy()
    {
        Closed();
    }

    public void OnRestartProtocols()
    {
        ClientProtoManager = new ProtoManager();
        foreach (System.Reflection.FieldInfo fi in typeof(NetProtocols).GetFields())
        {
            ClientProtoManager.AddRequestDelegate((int) fi.GetRawConstantValue(), Response);
        }
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
            Proxy = new Proxy(ServerSocket, 0, false);
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

    private int clientStates = 0;

    private void ReceiveSocket()
    {
        Proxy.DataHolder.Reset();
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

                Proxy.DataHolder.PushData(bytes, i);

                while (Proxy.DataHolder.IsFinished())
                {
                    ReceiveSocketData rsd = new ReceiveSocketData(Proxy.Socket, Proxy.DataHolder.mRecvData);
                    receiveDataQueue.Enqueue(rsd);
                    Proxy.DataHolder.RemoveFromHead();
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
        string Log = "Server：[" + r.GetProtocolName() + "]    " + r.DeserializeLog();
        ClientLog.CL.PrintReceive(Log);

        if (r is ServerRequestBase)
        {
            Proxy.ReceiveRequestsQueue.Enqueue((ServerRequestBase) r);
            Proxy.Response();
        }
    }

    #endregion

    #region 发送

    public void SendMessage(ClientRequestBase req)
    {
        Proxy.SendRequestsQueue.Enqueue(req);
    }

    private void Send(object obj)
    {
        ClientRequestBase request = (ClientRequestBase) obj;

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
            DataStream bufferWriter = new DataStream(true);
            request.Serialize(bufferWriter);
            byte[] msg = bufferWriter.ToByteArray();

            byte[] buffer = new byte[msg.Length + 4];
            DataStream writer = new DataStream(buffer, true);

            writer.WriteSInt32(msg.Length); //增加数据长度
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
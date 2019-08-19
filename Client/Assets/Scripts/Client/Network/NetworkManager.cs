using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoSingleton<NetworkManager>
{
    static int mainThreadId;

    public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId;

    void Awake()
    {
        mainThreadId = Thread.CurrentThread.ManagedThreadId;
        OnRestartProtocols();
        OnRestartSideEffects();
    }

    private void OnRestartProtocols()
    {
        foreach (NetProtocols num in Enum.GetValues(typeof(NetProtocols)))
        {
            ProtoManager.AddRequestDelegate((int) num, Response);
        }
    }

    private void OnRestartSideEffects() //所有的副作用在此注册
    {
        List<Type> types = Utils.GetClassesByNameSpace("SideEffects", Assembly.GetAssembly(typeof(Battle)));
        MethodInfo mi = typeof(SideEffectManager).GetMethod("AddSideEffectTypes");
        foreach (Type type in types)
        {
            if (Utils.IsBaseType(type, typeof(SideEffectBase)))
            {
                MethodInfo mi_temp = mi.MakeGenericMethod(type);
                mi_temp.Invoke(null, null);
            }
        }
    }

    private Socket ServerSocket;
    bool isStopReceive = true;

    public bool IsConnect() //是否已连接
    {
        return ServerSocket != null && ServerSocket.Connected && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.GetId;
    }

    public delegate void ConnectCallback();

    ConnectCallback connectDelegate = null;
    ConnectCallback connectFailedDelegate = null;

    Queue<ReceiveSocketData> receiveDataQueue = new Queue<ReceiveSocketData>();

    public static string ClientVersion = "1.0.1";
    public static string ServerVersion = "1.0.1";

    public bool ClientInvalid => ServerSocket != null && ServerSocket.Connected && ClientVersion != ServerVersion;

    //接收到数据放入数据队列，按顺序取出
    void Update()
    {
        if (Client.Instance.IsStandalone) return;
        if (!SocketErrorFlag)
        {
            if (receiveDataQueue.Count > 0)
            {
                ReceiveSocketData rsd = receiveDataQueue.Dequeue();
                DataStream stream = new DataStream(rsd.Data, true);
                ProtoManager.TryDeserialize(stream, rsd.Socket);
            }

            Client.Instance.Proxy.Send();
        }
        else
        {
            Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Offline;
            RoundManager.Instance.StopGame();
            SocketErrorFlag = false;
        }
    }

    void OnDestroy()
    {
        Closed();
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

        Thread getResult = new Thread(ConnectResult);
        getResult.IsBackground = true;
        getResult.Start(result);
    }

    private void ConnectResult(object obj)
    {
        IAsyncResult result = (IAsyncResult) obj;
        //这里做一个超时的监测，当连接超过5秒还没成功表示超时  
        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
        if (!success)
        {
            //超时  
            Closed();
            connectFailedDelegate?.Invoke();
        }
        else
        {
            //与socket建立连接成功，开启线程接受服务端数据。  
            isStopReceive = false;
            Client.Instance.Proxy.Socket = ServerSocket;
            Thread thread = new Thread(ReceiveSocket);
            thread.IsBackground = true;
            thread.Start();
        }
    }

    private void ConnectedCallback(IAsyncResult asyncConnect)
    {
        if (ServerSocket == null || !ServerSocket.Connected)
        {
            connectFailedDelegate?.Invoke();

            return;
        }

        connectDelegate?.Invoke();
    }

    //关闭Socket  
    public void Closed()
    {
        isStopReceive = true;

        if (ServerSocket != null && ServerSocket.Connected)
        {
            ServerSocket.Shutdown(SocketShutdown.Both);
            ClientLog.Instance.PrintError("[C]Socket close");
            ServerSocket.Close();
            if (NoticeManager.Instance != null & IsMainThread) NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnected"), 0f, 1.5f);
            RoundManager.Instance.StopGame();
        }

        ServerSocket = null;
    }

    #endregion

    #region 接收

    private void Response(RequestBase requestBase)
    {
        Client.Instance.Proxy.Response(requestBase);
    }

    private bool SocketErrorFlag = false;

    private void ReceiveSocket()
    {
        Client.Instance.Proxy.DataHolder.Reset();
        while (!isStopReceive)
        {
            if (ServerSocket != null)
            {
                if (!ServerSocket.Connected)
                {
                    //与服务器断开连接跳出循环  
                    ClientLog.Instance.PrintError("Connect Server Failed.");
                    ServerSocket.Close();
                    SocketErrorFlag = true;
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
                        RoundManager.Instance.StopGame();
                        ClientLog.Instance.PrintError("[C]Socket.Close();");
                        break;
                    }

                    Client.Instance.Proxy.DataHolder.PushData(bytes, i);

                    while (Client.Instance.Proxy.DataHolder.IsFinished())
                    {
                        ReceiveSocketData rsd = new ReceiveSocketData(Client.Instance.Proxy.Socket, Client.Instance.Proxy.DataHolder.mRecvData);
                        receiveDataQueue.Enqueue(rsd);
                        Client.Instance.Proxy.DataHolder.RemoveFromHead();
                    }
                }
                catch (Exception e)
                {
                    ClientLog.Instance.PrintError("[C]Failed by clientSocket error. " + e);
                    if (ServerSocket != null) ServerSocket.Close();
                    SocketErrorFlag = true;
                    break;
                }
            }
        }
    }

    #endregion

    #region 发送

    public void Send(object obj)
    {
        ClientRequestBase request = (ClientRequestBase) obj;

        if (ServerSocket == null)
        {
            ClientLog.Instance.PrintError("[C]Server socket is null");
            return;
        }

        if (!ServerSocket.Connected)
        {
            ClientLog.Instance.PrintError("[C]Not connected to server socket");
            Closed();
            return;
        }

        try
        {
            byte[] data = SerializeRequest(request);

            IAsyncResult asyncSend = ServerSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), ServerSocket);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
            {
                Closed();
            }

            string log = "SendToServer: <" + request.GetProtocol() + "> " + request.DeserializeLog();
            ClientLog.Instance.Print(log);
        }
        catch (Exception e)
        {
            ClientLog.Instance.PrintError("[C]Send error : " + e.ToString());
            RoundManager.Instance.StopGame();
        }
    }

    public static byte[] SerializeRequest(RequestBase request)
    {
        DataStream bufferWriter = new DataStream(true);
        request.Serialize(bufferWriter);
        byte[] msg = bufferWriter.ToByteArray();

        byte[] buffer = new byte[msg.Length + 4];
        DataStream writer = new DataStream(buffer, true);

        writer.WriteSInt32(msg.Length); //增加数据长度
        writer.WriteRaw(msg);

        byte[] data = writer.ToByteArray();
        return data;
    }

    private void SendCallback(IAsyncResult asyncConnect)
    {
        //ClientLog.Instance.Print("[C]Send success");
    }

    #endregion

    bool isReconnecting = false;
    private Coroutine CurrentTryConnectServer;

    private static Dictionary<LoginPanel.ServerTypes, string> ServerIPDict = new Dictionary<LoginPanel.ServerTypes, string>
    {
        {LoginPanel.ServerTypes.FormalServer, "95.169.26.10"},
        {LoginPanel.ServerTypes.TestServer, "127.0.0.1"},
    };

    IEnumerator TryConnectToServer(LoginPanel.ServerTypes serverType)
    {
        while (true)
        {
            if (Client.Instance.Proxy.Socket == null || !Client.Instance.Proxy.Socket.Connected)
            {
                Connect(ServerIPDict[serverType], 9999, ConnectCallBack, null);
            }

            CheckConnectState();
            yield return new WaitForSeconds(2f);
        }
    }

    public void ConnectToServer(LoginPanel.ServerTypes serverType)
    {
        TerminateConnection();
        CurrentTryConnectServer = StartCoroutine(TryConnectToServer(serverType));
    }

    public void TerminateConnection()
    {
        isReconnecting = false;
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnecting"), 0f, float.PositiveInfinity);
        UIManager.Instance.GetBaseUIForm<LoginPanel>()?.ShowTipText(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnecting"), 0f, float.PositiveInfinity, true);
        try
        {
            if (CurrentTryConnectServer != null) StopCoroutine(CurrentTryConnectServer);
            if (Client.Instance.Proxy.Socket != null)
            {
                Client.Instance.Proxy.Socket.Close();
            }

            Client.Instance.Proxy.Socket = null;
        }
        catch (Exception e)
        {
            ClientLog.Instance.PrintClientStates(e.ToString());
        }

        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnected"), 0f, 1f);
        UIManager.Instance.GetBaseUIForm<LoginPanel>()?.ShowTipText(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnected"), 0f, 1f, false);
    }

    private void CheckConnectState()
    {
        if (Client.Instance.Proxy.Socket != null && Client.Instance.Proxy.Socket.Connected)
        {
            if (isReconnecting)
            {
                if (Client.Instance.IsPlaying()) RoundManager.Instance.HasShowLostConnectNotice = false;
                NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_Connected"), 0f, 2f);
                isReconnecting = false;
            }
        }
        else
        {
            if (!isReconnecting)
            {
                UIManager.Instance.GetBaseUIForm<LoginPanel>().ShowTipText(LanguageManager.Instance.GetText("Notice_NetworkManager_Connecting"), 0f, float.PositiveInfinity, true);
                isReconnecting = true;
            }
        }
    }

    void ConnectCallBack()
    {
        ClientLog.Instance.Print("Connect success.");
    }

    public void SuccessMatched()
    {
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_MatchSuccess"), 0, 1f);
    }
}
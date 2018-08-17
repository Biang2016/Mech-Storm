using System;
using UnityEngine;
using System.Collections;

internal class NetworkManager : MonoSingletion<NetworkManager>
{
    void Start()
    {
    }

    bool isReconnecting = false;
    private IEnumerator CurrentTryConnectServer;

    IEnumerator TryConnectToServer(bool isTest)
    {
        while (true)
        {
            if (Client.Instance.Proxy == null || !Client.Instance.Proxy.Socket.Connected)
            {
                if (isTest) Client.Instance.Connect("127.0.0.1", 9999, ConnectCallBack, null);
                else Client.Instance.Connect("95.169.26.10", 9999, ConnectCallBack, null);
            }

            CheckConnectState();
            yield return new WaitForSeconds(3f);
        }
    }

    public void ConnectToTestServer()
    {
        TerminateConnection();
        CurrentTryConnectServer = TryConnectToServer(true);
        StartCoroutine(CurrentTryConnectServer);
    }

    public void ConnectToFormalServer()
    {
        TerminateConnection();
        CurrentTryConnectServer = TryConnectToServer(false);
        StartCoroutine(CurrentTryConnectServer);
    }

    public void TerminateConnection()
    {
        NoticeManager.Instance.ShowInfoPanel("正在断开连接", 0f, float.PositiveInfinity);
        try
        {
            if (CurrentTryConnectServer != null) StopCoroutine(CurrentTryConnectServer);
            if (Client.Instance.Proxy != null)
            {
                if (Client.Instance.Proxy.Socket != null)
                {
                    Client.Instance.Proxy.Socket.Close();
                }
                else
                {
                    Client.Instance.Proxy = null;
                }
            }
        }
        catch (Exception e)
        {
            ClientLog.Instance.PrintClientStates(e.ToString());
        }

        NoticeManager.Instance.ShowInfoPanel("已断开连接", 0f, 1f);
    }

    private void CheckConnectState()
    {
        if (Client.Instance.Proxy != null && Client.Instance.Proxy.Socket.Connected)
        {
            if (isReconnecting)
            {
                NoticeManager.Instance.ShowInfoPanel("连接服务器成功", 0f, 2f);
                isReconnecting = false;
            }
        }
        else
        {
            if (!isReconnecting)
            {
                NoticeManager.Instance.ShowInfoPanel("正在连接服务器", 0f, float.PositiveInfinity);
                isReconnecting = true;
            }
        }
    }

    void ConnectCallBack()
    {
        ClientLog.Instance.Print("连接服务器成功!");
    }

    public void SuccessMatched()
    {
        NoticeManager.Instance.ShowInfoPanel("匹配成功，开始比赛", 0, 1f);
    }

    private int high;

    void OnGUI()
    {
        high = 10;

        if (Client.Instance.Proxy == null) return;
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck)
        {
            if (CreateBtn("开始匹配"))
            {
                Client.Instance.Proxy.OnBeginMatch();
                ClientLog.Instance.Print("开始匹配");
                NoticeManager.Instance.ShowInfoPanel("匹配中", 0, float.PositiveInfinity);
            }
        }

        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            if (CreateBtn("取消匹配"))
            {
                Client.Instance.Proxy.CancelMatch();
                ClientLog.Instance.Print("取消匹配");
                NoticeManager.Instance.ShowInfoPanel("取消匹配", 0, 1f);
            }
        }

        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            if (CreateBtn("退出比赛"))
            {
                Client.Instance.Proxy.LeaveGame();
                RoundManager.Instance.StopGame();
                ClientLog.Instance.Print("您已退出比赛");
                NoticeManager.Instance.ShowInfoPanel("您已退出比赛", 0, 1f);
            }
        }
    }

    private bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(20, high, 150, 30), btnname);
        high += 35;
        return b;
    }
}
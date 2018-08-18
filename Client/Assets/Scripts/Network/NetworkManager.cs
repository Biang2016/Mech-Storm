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
        isReconnecting = false;
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
}
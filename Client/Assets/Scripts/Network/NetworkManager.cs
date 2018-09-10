using System;
using System.Collections;
using UnityEngine;

public class NetworkManager : MonoSingletion<NetworkManager>
{
    void Start()
    {
    }

    bool isReconnecting = false;
    private Coroutine CurrentTryConnectServer;

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
            yield return new WaitForSeconds(2f);
        }
    }

    public void ConnectToTestServer()
    {
        TerminateConnection();
        CurrentTryConnectServer = StartCoroutine(TryConnectToServer(true));
    }

    public void ConnectToFormalServer()
    {
        TerminateConnection();
        CurrentTryConnectServer = StartCoroutine(TryConnectToServer(false));
    }

    public void TerminateConnection()
    {
        isReconnecting = false;
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Disconnecting" : "正在断开连接", 0f, float.PositiveInfinity);
        LoginManager.Instance.ShowTipText(GameManager.Instance.isEnglish ? "Disconnecting" : "正在断开连接", 0f, float.PositiveInfinity, true);
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

        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Disconnected" : "已断开连接", 0f, 1f);
        LoginManager.Instance.ShowTipText(GameManager.Instance.isEnglish ? "Disconnected" : "已断开连接", 0f, 1f, false);
    }

    private void CheckConnectState()
    {
        if (Client.Instance.Proxy != null && Client.Instance.Proxy.Socket.Connected)
        {
            if (isReconnecting)
            {
                RoundManager.Instance.HasShowLostConnectNotice = false;
                NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Connected" : "连接服务器成功", 0f, 2f);
                isReconnecting = false;
            }
        }
        else
        {
            if (!isReconnecting)
            {
                LoginManager.Instance.ShowTipText(GameManager.Instance.isEnglish ? "Connecting" : "正在连接服务器", 0f, float.PositiveInfinity, true);
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
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Match success! Game begin!" : "匹配成功，开始比赛", 0, 1f);
    }
}
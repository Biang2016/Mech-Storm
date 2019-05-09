using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoSingleton<NetworkManager>
{
    void Start()
    {
    }

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
            if (Client.Instance.Proxy == null || !Client.Instance.Proxy.Socket.Connected)
            {
                Client.Instance.Connect(ServerIPDict[serverType], 9999, ConnectCallBack, null);
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
        UIManager.Instance.GetBaseUIForm<LoginPanel>().ShowTipText(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnecting"), 0f, float.PositiveInfinity, true);
        try
        {
            if (CurrentTryConnectServer != null) StopCoroutine(CurrentTryConnectServer);
            if (Client.Instance.Proxy != null)
            {
                if (Client.Instance.Proxy.Socket != null)
                {
                    Client.Instance.Proxy.Socket.Close();
                }

                Client.Instance.Proxy = null;
            }
        }
        catch (Exception e)
        {
            ClientLog.Instance.PrintClientStates(e.ToString());
        }

        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnected"), 0f, 1f);
        UIManager.Instance.GetBaseUIForm<LoginPanel>().ShowTipText(LanguageManager.Instance.GetText("Notice_NetworkManager_Disconnected"), 0f, 1f, false);
    }

    private void CheckConnectState()
    {
        if (Client.Instance.Proxy != null && Client.Instance.Proxy.Socket.Connected)
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
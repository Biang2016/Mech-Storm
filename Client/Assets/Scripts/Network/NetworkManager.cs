using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

internal class NetworkManager : MonoSingletion<NetworkManager>
{
    void Start()
    {
        StartCoroutine(TryConnectToServer());
    }

    bool isReconnecting = false;

    IEnumerator TryConnectToServer()
    {
        while (true)
        {
            if (Client.CS.Proxy == null || !Client.CS.Proxy.Socket.Connected)
            {
                Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
            }

            CheckConnectState();
            yield return new WaitForSeconds(3f);
        }
    }

    private void CheckConnectState()
    {
        if (Client.CS.Proxy != null && Client.CS.Proxy.Socket.Connected)
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
        ClientLog.CL.Print("连接服务器成功!");
    }

    public void SuccessMatched()
    {
        NoticeManager.Instance.ShowInfoPanel("匹配成功，开始比赛", 0, 1f);
    }

    private int high;

    void OnGUI()
    {
        high = 10;

        if (Client.CS.Proxy == null) return;
        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck)
        {
            if (CreateBtn("开始匹配"))
            {
                Client.CS.Proxy.OnBeginMatch();
                ClientLog.CL.Print("开始匹配");
                NoticeManager.Instance.ShowInfoPanel("匹配中", 0, float.PositiveInfinity);
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            if (CreateBtn("取消匹配"))
            {
                Client.CS.Proxy.CancelMatch();
                ClientLog.CL.Print("取消匹配");
                NoticeManager.Instance.ShowInfoPanel("取消匹配", 0, 1f);
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            if (CreateBtn("退出比赛"))
            {
                Client.CS.Proxy.LeaveGame();
                RoundManager.Instance.StopGame();
                ClientLog.CL.Print("您已退出比赛");
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

internal class NetworkManager : MonoBehaviour
{
    private static NetworkManager _nm;

    public static NetworkManager NM
    {
        get
        {
            if (!_nm)
            {
                _nm = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
            }

            return _nm;
        }
    }

    public Animator InfoPanelAnimator;
    public Text InfoText;

    List<int> SelfCardDeckInfo = new List<int>();

    public void Awake()
    {
        AllCards.AddAllCards();
    }

    public void Start()
    {
        TryConnectToServer();
        StartCoroutine(CheckConnection());
    }

    void TryConnectToServer()
    {
        if (Client.CS.Proxy == null || Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Nothing)
        {
            Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
            if (Client.CS.Proxy.Socket.Connected)
            {
                StartCoroutine(ShowInfoPanel("连接服务器成功", 1f, 1f));
            }
        }
    }

    bool isReconnecting = false;

    IEnumerator CheckConnection()
    {
        while (true)
        {
            if (!Client.CS.Proxy.Socket.Connected)
            {
                Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
                yield return new WaitForSeconds(1f);
                if (Client.CS.Proxy.Socket.Connected)
                {
                    StartCoroutine(ShowInfoPanel("重连服务器成功", 0f, 1f));
                }
                else
                {
                    if (!isReconnecting) StartCoroutine(ShowInfoPanel("正在连接服务器", 0f, float.PositiveInfinity));
                    isReconnecting = true;
                }
            }

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator ShowInfoPanel(string text, float delay, float last)
    {
        yield return new WaitForSeconds(delay);
        InfoText.text = text;
        if (InfoPanelAnimator.GetBool("isShow")) InfoPanelAnimator.SetTrigger("Shut");
        InfoPanelAnimator.SetBool("isShow", true);
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            InfoPanelAnimator.SetBool("isShow", false);
        }
        else
        {
            int dotCount = 0;
            while (!Client.CS.Proxy.Socket.Connected)
            {
                InfoText.text += ".";
                yield return new WaitForSeconds(0.5f);
                dotCount++;
                if (dotCount == 3)
                {
                    dotCount = 0;
                    InfoText.text = text;
                }
            }
        }
    }

    private int high;
    private string clientIdStr = "";
    private string cardDeckInfo = "";

    void OnGUI()
    {
        high = 10;

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.GetId || Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck)
        {
            cardDeckInfo = CreateTextField("卡组信息", cardDeckInfo);

            if (CreateBtn("确认卡组"))
            {
                List<string> tmp = cardDeckInfo.Split(',').ToList();
                SelfCardDeckInfo.Clear();
                foreach (string s in tmp)
                {
                    SelfCardDeckInfo.Add(int.Parse(s));
                }

                Client.CS.Proxy.OnSendCardDeck(new CardDeckInfo(SelfCardDeckInfo.ToArray()));
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck)
        {
            if (CreateBtn("开始匹配"))
            {
                Client.CS.Proxy.OnBeginMatch();
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            if (CreateBtn("取消匹配"))
            {
                Client.CS.Proxy.CancelMatch();
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            if (CreateBtn("退出比赛"))
            {
                Client.CS.Proxy.LeaveGame();
                RoundManager.RM.OnGameStop();
            }
        }
    }

    private string CreateTextField(string textTitle, string contentStr)
    {
        GUI.Label(new Rect(20, high + 5, 60, 30), textTitle);
        contentStr = GUI.TextField(new Rect(80, high, 90, 30), contentStr);
        high += 35;
        return contentStr;
    }

    public bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(20, high, 150, 30), btnname);
        high += 35;
        return b;
    }


    public void ConnectCallBack()
    {
        ClientLog.CL.Print("连接服务器成功!");
    }
}
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

    void Awake()
    {
        AllCards.AddAllCards();
    }

    void Start()
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
                ShowInfoPanel("连接服务器成功", 0f, 1f);
                isReconnecting = false;
            }
        }
    }

    bool isReconnecting = false;

    IEnumerator CheckConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (!Client.CS.Proxy.Socket.Connected)
            {
                Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
                yield return new WaitForSeconds(1f);
                if (Client.CS.Proxy.Socket.Connected)
                {
                    ShowInfoPanel("连接服务器成功", 0f, 1f);
                    isReconnecting = false;
                }
                else
                {
                    if (!isReconnecting)
                    {
                        ShowInfoPanel("正在连接服务器", 0f, float.PositiveInfinity);
                        isReconnecting = true;
                    }
                }
            }
        }
    }

    void ConnectCallBack()
    {
        ClientLog.CL.Print("连接服务器成功!");
    }

    IEnumerator ShowInfoPanelCoroutine;
    void ShowInfoPanel(string text, float delay, float last)
    {
        if (ShowInfoPanelCoroutine != null)
        {
            StopCoroutine(ShowInfoPanelCoroutine);
        }
        ShowInfoPanelCoroutine = Co_ShowInfoPanel(text, delay, last);
        StartCoroutine(ShowInfoPanelCoroutine);
    }

    IEnumerator Co_ShowInfoPanel(string text, float delay, float last)
    {
        yield return new WaitForSeconds(delay);
        InfoText.text = text;
        if (InfoPanelAnimator.GetBool("isShow"))
        {
            InfoPanelAnimator.SetTrigger("Shut");
        }

        InfoPanelAnimator.SetBool("isShow", true);
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            InfoPanelAnimator.SetBool("isShow", false);
        }
        else
        {
            int dotCount = 0;
            while (true)
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

    public void SuccessMatched(){
        ShowInfoPanel("匹配成功，开始比赛", 0, 1f);
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
                ShowInfoPanel("更新卡组成功", 0, 1f);
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck)
        {
            if (CreateBtn("开始匹配"))
            {
                Client.CS.Proxy.OnBeginMatch();
                ClientLog.CL.Print("开始匹配");
                ShowInfoPanel("匹配中", 0, float.PositiveInfinity);
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            if (CreateBtn("取消匹配"))
            {
                Client.CS.Proxy.CancelMatch();
                ClientLog.CL.Print("取消匹配");
                ShowInfoPanel("取消匹配", 0, 1f);
            }
        }

        if (Client.CS.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            if (CreateBtn("退出比赛"))
            {
                Client.CS.Proxy.LeaveGame();
                RoundManager.RM.StopGame();
                ClientLog.CL.Print("您已退出比赛");
                ShowInfoPanel("您已退出比赛", 0, 1f);
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

    private bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(20, high, 150, 30), btnname);
        high += 35;
        return b;
    }
}
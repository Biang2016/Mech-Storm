using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public List<int> SelfCardDeckInfo = new List<int>();

    public void Awake()
    {
        AllCards.AddAllCards();
    }

    private int high;
    private string clientIdStr = "";
    private string cardDeckInfo = "";

    void OnGUI()
    {
        high = 10;
        if (CreateBtn("连接到服务器"))
        {
            Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
        }

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

        if (CreateBtn("开始匹配"))
        {
            Client.CS.Proxy.OnBeginMatch();
        }

        if (CreateBtn("重置游戏"))
        {
            Client.CS.Proxy.ReSetClient();
        }
    }

    private string CreateTextField(string textTitle, string contentStr)
    {
        GUI.Label(new Rect(20, high + 5, 50, 30), textTitle);
        contentStr = GUI.TextField(new Rect(70, high, 100, 30), contentStr);
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
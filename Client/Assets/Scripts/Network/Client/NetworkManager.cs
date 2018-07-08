using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkManager : MonoBehaviour
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

    public int SelfClientId;
    public List<int> SelfCardDeckInfo = new List<int>();

    public void Awake()
    {
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

        //if (CreateBtn("发送测试请求"))
        //{
        //    TestConnectRequest req = new TestConnectRequest(123);
        //    Client.CS.SendMessage(req);
        //}

        //if (CreateBtn("服务器测试广播"))
        //{
        //    TestConnectRequest req = new TestConnectRequest(456);
        //    Server.SV.SendMessage(req);
        //}

        clientIdStr = CreateTextField("客户ID", clientIdStr);

        if (CreateBtn("注册客户ID"))
        {
            SelfClientId = int.Parse(clientIdStr);
            ClientIdRequest req = new ClientIdRequest(SelfClientId, ClientIdPurpose.RegisterClientId);
            Client.CS.SendMessage(req);
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

            CardDeckRequest req = new CardDeckRequest(SelfClientId, new CardDeckInfo(SelfCardDeckInfo.ToArray()));
            Client.CS.SendMessage(req);
        }

        if (CreateBtn("客户端请求开始匹配"))
        {
            ClientIdRequest req = new ClientIdRequest(SelfClientId, ClientIdPurpose.MatchGames);
            Client.CS.SendMessage(req);
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
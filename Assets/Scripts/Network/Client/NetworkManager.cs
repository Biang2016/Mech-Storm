using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public void Awake()
    {
    }

    int high;

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

        string clientIdStr = CreateTextField("客户ID");
        SelfClientId = int.Parse(clientIdStr);

        if (CreateBtn("客户端请求开始匹配"))
        {
            EntryGameRequest req = new EntryGameRequest(SelfClientId);
            Client.CS.SendMessage(req);
        }
    }

    private string CreateTextField(string textTitle)
    {
        GUI.Label(new Rect(20, high + 5, 50, 30), textTitle);
        string str = "";
        str = GUI.TextField(new Rect(70, high, 100, 30), str);
        high += 35;
        return str;
    }

    public bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(20, high, 150, 30), btnname);
        high += 35;
        return b;
    }


    public void ConnectCallBack()
    {
        Debug.Log("Connect success!");
    }
}
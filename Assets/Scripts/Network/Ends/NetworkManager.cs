using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    void Awake()
    {
    }


    void OnGUI()
    {
        high = 10;
        if (CreateBtn("连接到服务器"))
        {
            Client.CS.Connect("127.0.0.1", 9999, ConnectCallBack, null);
        }

        if (CreateBtn("发送测试请求"))
        {
            TestConnectRequest req = new TestConnectRequest(123);
            Client.CS.SendMessage(req);
        }

        if (CreateBtn("服务器测试广播"))
        {
            TestConnectRequest req = new TestConnectRequest(456);
            Server.SV.SendMessage(req);
        }

        if (CreateBtn("客户端请求开始游戏"))
        {
            EntryGameRequest req = new EntryGameRequest(100001);
            Client.CS.SendMessage(req);
        }
    }

    int high;

    public bool CreateBtn(string btnname)
    {
        bool b = GUI.Button(new Rect(20, high, 150, 40), btnname);
        high += 45;
        return b;
    }

    public void ConnectCallBack()
    {
        Debug.Log("Connect success!");
    }
}
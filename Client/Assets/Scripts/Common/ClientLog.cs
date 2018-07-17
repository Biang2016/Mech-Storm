using System;
using System.Collections.Generic;
using UnityEngine;

internal class ClientLog : MonoBehaviour
{
    private static ClientLog _cl;

    public static ClientLog CL
    {
        get { return _cl; }
    }

    private ClientLog()
    {
    }

    void Awake()
    {
        _cl = FindObjectOfType<ClientLog>();
        LogMessages = new Queue<Log>();
    }

    void Update()
    {
        DoPrint();
    }

    Queue<Log> LogMessages;

    public void Print(string logStr)
    {
        Print(logStr, Color.white);
    }

    public void Print(string logStr, Color color)
    {
        Log log = new Log(logStr, color);
        LogMessages.Enqueue(log);
    }

    public void DoPrint()
    {
        if (LogMessages.Count > 0)
        {
            Log log = LogMessages.Dequeue();
            if (log != null)
            {
                Debug.Log("<color=#" + log.Color + ">" + log.LogStr + "</color>");
            }
        }
    }

    public void PrintWarning(string logStr)
    {
        Print(logStr, Color.yellow);
    }

    public void PrintError(string logStr)
    {
        Print(logStr, Color.red);
    }

    public void PrintClientStates(string logStr)
    {
        Print(logStr, Color.green);
    }

    public void PrintServerStates(string logStr)
    {
        Print(logStr, Color.gray);
    }

    public void PrintReceive(string logStr)
    {
        Print(logStr, new Color(0.5f,0.5f,1f));
    }

    public void PrintSend(string logStr)
    {
        Print(logStr, Color.magenta);
    }
}

class Log
{
    public string LogStr;
    public string Color;
    public string Time;

    public Log(string logStr, Color color)
    {
        Color = ColorUtility.ToHtmlStringRGB(color);
        Time = DateTime.Now.ToLongTimeString();
        LogStr = Time + "  " + logStr;
    }
}
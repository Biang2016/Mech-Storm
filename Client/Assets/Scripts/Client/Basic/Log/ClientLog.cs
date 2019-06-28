using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientLog : MonoSingleton<ClientLog>, ILog
{
    private Queue<Log> LogQueue = new Queue<Log>();

    void Awake()
    {
        //sw = new StreamWriter(Application.streamingAssetsPath + "/RequestLog.txt", false);
        //sw.Close();
    }

    void Update()
    {
        DoPrint();
    }

    public void Print(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Normal) == LogVerbosity.Normal)
        {
            Print(logStr, Color.white);
        }
    }

    private void Print(string logStr, Color color)
    {
        if (RootManager.Instance.ShowClientLogs)
        {
            Log log = new Log(logStr, color);
            LogQueue.Enqueue(log);
        }
    }

    //private static StreamWriter sw;

    public void DoPrint()
    {
#if DEBUG
        //sw = new StreamWriter(Application.streamingAssetsPath + "/RequestLog.txt", true);
#endif

        if (LogQueue.Count > 0)
        {
            Log log = LogQueue.Dequeue();
            if (log != null)
            {
                Debug.Log("<color=#" + log.Color + ">" + log.LogStr + "</color>");
#if DEBUG
                //sw.WriteLine(log.LogStr);
#endif
            }
        }
#if DEBUG
        //sw.Close();
#endif
    }

    [SerializeField] private LogVerbosity logVerbosity;

    public LogVerbosity LogVerbosity
    {
        get { return logVerbosity; }
        set { logVerbosity = value; }
    }

    public void PrintWarning(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Warning) == LogVerbosity.Warning)
        {
            Print(logStr, Color.yellow);
        }
    }

    public void PrintError(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Error) == LogVerbosity.Error)
        {
            Print(logStr, Color.red);
        }
    }

    public void PrintClientStates(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.ClientState) == LogVerbosity.ClientState)
        {
            Print(logStr, Color.green);
        }
    }

    public void PrintServerStates(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.ServerState) == LogVerbosity.ServerState)
        {
            Print(logStr, Color.grey);
        }
    }

    public void PrintReceive(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Receive) == LogVerbosity.Receive)
        {
            Print(logStr, Color.blue);
        }
    }

    public void PrintSend(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Send) == LogVerbosity.Send)
        {
            Print(logStr, Color.magenta);
        }
    }

    public void PrintBattleEffectsStart(string logStr)
    {
        Print(logStr, ClientUtils.HTMLColorToColor("#00FFDB"));
    }

    public void PrintBattleEffectsEnd(string logStr)
    {
        Print(logStr, ClientUtils.HTMLColorToColor("#007464"));
    }
}

public class Log : LogBase
{
    public string Color;

    public Log(string logStr, Color color)
    {
        Color = ColorUtility.ToHtmlStringRGB(color);
        Time = DateTime.Now.ToLongTimeString();
        LogStr = Time + "  " + logStr;
    }
}
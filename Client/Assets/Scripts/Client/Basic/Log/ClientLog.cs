using System;
using System.Collections.Generic;
using UnityEngine;

public class ClientLog : MonoSingleton<ClientLog>, ILog
{
    private Queue<Log> LogQueue { get; set; }

    void Awake()
    {
        LogQueue = new Queue<Log>();
        //sw = new StreamWriter(Application.streamingAssetsPath + "/RequestLog.txt", false);
        //sw.Close();
    }

    void Update()
    {
        DoPrint();
    }

    public void Print(string logStr)
    {
        Print(logStr, Color.white);
    }

    private void Print(string logStr, Color color)
    {
        Log log = new Log(logStr, color);
        LogQueue.Enqueue(log);
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
                if (RootManager.Instance.ShowClientLogs)
                {
                    Debug.Log("<color=#" + log.Color + ">" + log.LogStr + "</color>");
#if DEBUG
                    //sw.WriteLine(log.LogStr);
#endif
                }
            }
        }
#if DEBUG
        //sw.Close();
#endif
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
        Print(logStr, new Color(0.5f, 0.5f, 1f));
    }

    public void PrintSend(string logStr)
    {
        Print(logStr, Color.magenta);
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
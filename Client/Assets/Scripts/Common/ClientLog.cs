using System;
using System.Collections.Generic;
using UnityEngine;

class ClientLog : MonoBehaviour
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

    Queue<Log> LogMessages;

    private void Update()
    {
        while (LogMessages.Count > 0)
        {
            Log tmp = LogMessages.Dequeue();
            Debug.Log(tmp.LogStr);
            Console.ForegroundColor = tmp.ConsoleColor;
            Console.WriteLine(tmp.LogStr, tmp.ConsoleColor);
        }
    }

    public void Print(string logStr)
    {
        Print(logStr, ConsoleColor.White);
    }

    public void Print(string logStr, ConsoleColor consoleColor)
    {
        Log log = new Log(logStr, consoleColor);
        LogMessages.Enqueue(log);
    }

    public void PrintWarning(string logStr)
    {
        Print(logStr, ConsoleColor.Yellow);
    }

    public void PrintError(string logStr)
    {
        Print(logStr, ConsoleColor.Red);
    }

    public void PrintClientStates(string logStr)
    {
        Print(logStr, ConsoleColor.Green);
    }

    public void PrintServerStates(string logStr)
    {
        Print(logStr, ConsoleColor.DarkGray);
    }

    public void PrintReceive(string logStr)
    {
        Print(logStr, ConsoleColor.Blue);
    }

    public void PrintSend(string logStr)
    {
        Print(logStr, ConsoleColor.Magenta);
    }
}

class Log
{
    public string LogStr;
    public ConsoleColor ConsoleColor;
    public string Time;

    public Log(string logStr, ConsoleColor consoleColor)
    {
        ConsoleColor = consoleColor;
        Time = System.DateTime.Now.ToLongTimeString();
        LogStr = Time + "  " + logStr;
    }
}
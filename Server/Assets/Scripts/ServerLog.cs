using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class ServerLog
{
    public static void Print(string logStr)
    {
        Print(logStr, ConsoleColor.White);
    }

    public static void Print(string logStr, ConsoleColor consoleColor)
    {
        Log log = new Log(logStr, consoleColor);
        LogQueue.Enqueue(log);
    }

    public static void PrintWarning(string logStr)
    {
        Print(logStr, ConsoleColor.Yellow);
    }

    public static void PrintError(string logStr)
    {
        Print(logStr, ConsoleColor.Red);
    }

    public static void PrintClientStates(string logStr)
    {
        Print(logStr, ConsoleColor.Green);
    }

    public static void PrintServerStates(string logStr)
    {
        Print(logStr, ConsoleColor.DarkGray);
    }

    public static void PrintReceive(string logStr)
    {
        Print(logStr, ConsoleColor.Blue);
    }

    public static void PrintSend(string logStr)
    {
        Print(logStr, ConsoleColor.Magenta);
    }


    static Queue<Log> LogQueue = new Queue<Log>();

    public static void Update()
    {
        if (LogQueue.Count > 0)
        {
            lock (LogQueue)
            {
                Log tmp = LogQueue.Dequeue();
                Console.ForegroundColor = tmp.ConsoleColor;
                Console.WriteLine(tmp.LogStr);
            }
        }
    }
}

class Log
{
    public string LogStr;
    public ConsoleColor ConsoleColor;

    public Log(string logStr, ConsoleColor consoleColor)
    {
        LogStr = logStr;
        ConsoleColor = consoleColor;
    }
}
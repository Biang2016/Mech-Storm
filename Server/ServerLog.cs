using System;
using System.Collections.Generic;

internal static class ServerLog
{
    static Queue<Log> LogQueue = new Queue<Log>();

    public static void Print(string logStr, ConsoleColor consoleColor)
    {
        Log log = new Log(logStr, consoleColor);
        LogQueue.Enqueue(log);
        DoPrint();
    }

    private static void DoPrint()
    {
        if (LogQueue.Count > 0)
        {
            Log tmp = LogQueue.Dequeue();
            if (tmp != null)
            {
                Console.ForegroundColor = tmp.ConsoleColor;
                Console.WriteLine(tmp.Time + "  " + tmp.LogStr);
            }
        }
    }

    public static void Print(string logStr)
    {
        Print(logStr, ConsoleColor.White);
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
}

class Log
{
    public string LogStr;
    public ConsoleColor ConsoleColor;
    public string Time;

    public Log(string logStr, ConsoleColor consoleColor)
    {
        LogStr = logStr;
        ConsoleColor = consoleColor;
        Time = System.DateTime.Now.ToLongTimeString();
    }
}
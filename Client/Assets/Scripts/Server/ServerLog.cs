using System;
using System.Collections.Generic;

public class ServerLog : ILog
{
    public static ServerLog Instance = new ServerLog();

    private ServerLog()
    {
        LogQueue = new Queue<Log>();
    }

    private Queue<Log> LogQueue { get; set; }

    private void Print(string logStr, ConsoleColor consoleColor)
    {
        Log log = new Log(logStr, consoleColor);
        LogQueue.Enqueue(log);
        DoPrint();
    }

    public void DoPrint()
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

    public LogVerbosity LogVerbosity { get; set; }

    public void Print(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Normal) == LogVerbosity.Normal)
        {
            Print(logStr, ConsoleColor.White);
        }
    }

    public void PrintWarning(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Warning) == LogVerbosity.Warning)
        {
            Print(logStr, ConsoleColor.Yellow);
        }
    }

    public void PrintError(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Error) == LogVerbosity.Error)
        {
            Print(logStr, ConsoleColor.Red);
        }
    }

    public void PrintClientStates(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.ClientState) == LogVerbosity.ClientState)
        {
            Print(logStr, ConsoleColor.Green);
        }
    }

    public void PrintServerStates(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.ServerState) == LogVerbosity.ServerState)
        {
            Print(logStr, ConsoleColor.DarkGray);
        }
    }

    public void PrintReceive(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Receive) == LogVerbosity.Receive)
        {
            Print(logStr, ConsoleColor.Blue);
        }
    }

    public void PrintSend(string logStr)
    {
        if ((LogVerbosity & LogVerbosity.Send) == LogVerbosity.Send)
        {
            Print(logStr, ConsoleColor.Magenta);
        }
    }
}

public class Log : LogBase
{
    public ConsoleColor ConsoleColor;

    public Log(string logStr, ConsoleColor consoleColor)
    {
        LogStr = logStr;
        ConsoleColor = consoleColor;
        Time = DateTime.Now.ToLongTimeString();
    }
}
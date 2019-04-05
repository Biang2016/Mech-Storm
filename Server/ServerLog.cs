using System;
using System.Collections.Generic;

internal static class ServerLog
{
    static Queue<Log> LogQueue = new Queue<Log>();

    internal static ServerLogVerbosity ServerLogVerbosity;

    private static void Print(string logStr, ConsoleColor consoleColor)
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
        if ((ServerLogVerbosity & ServerLogVerbosity.Normal) == ServerLogVerbosity.Normal)
        {
            Print(logStr, ConsoleColor.White);
        }
    }

    public static void PrintWarning(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Warning) == ServerLogVerbosity.Warning)
        {
            Print(logStr, ConsoleColor.Yellow);
        }
    }

    public static void PrintError(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Error) == ServerLogVerbosity.Error)
        {
            Print(logStr, ConsoleColor.Red);
        }
    }

    public static void PrintClientStates(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.ClientState) == ServerLogVerbosity.ClientState)
        {
            Print(logStr, ConsoleColor.Green);
        }
    }

    public static void PrintServerStates(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.ServerState) == ServerLogVerbosity.ServerState)
        {
            Print(logStr, ConsoleColor.DarkGray);
        }
    }

    public static void PrintReceive(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Receive) == ServerLogVerbosity.Receive)
        {
            Print(logStr, ConsoleColor.Blue);
        }
    }

    public static void PrintSend(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Send) == ServerLogVerbosity.Send)
        {
            Print(logStr, ConsoleColor.Magenta);
        }
    }
}

internal class Log
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

[Flags]
internal enum ServerLogVerbosity
{
    Normal,
    Warning,
    Error,
    ClientState,
    ServerState,
    Send,
    Receive,
    All = Normal | Warning | Error | ClientState | ServerState | Send | Receive,
    States = ClientState | ServerState,
    StatesAndLog = Normal | Warning | Error | ClientState | ServerState,
}
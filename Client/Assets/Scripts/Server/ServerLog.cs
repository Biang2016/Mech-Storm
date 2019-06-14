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

    public ServerLogVerbosity ServerLogVerbosity;

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

    public void Print(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Normal) == ServerLogVerbosity.Normal)
        {
            Print(logStr, ConsoleColor.White);
        }
    }

    public void PrintWarning(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Warning) == ServerLogVerbosity.Warning)
        {
            Print(logStr, ConsoleColor.Yellow);
        }
    }

    public void PrintError(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Error) == ServerLogVerbosity.Error)
        {
            Print(logStr, ConsoleColor.Red);
        }
    }

    public void PrintClientStates(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.ClientState) == ServerLogVerbosity.ClientState)
        {
            Print(logStr, ConsoleColor.Green);
        }
    }

    public void PrintServerStates(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.ServerState) == ServerLogVerbosity.ServerState)
        {
            Print(logStr, ConsoleColor.DarkGray);
        }
    }

    public void PrintReceive(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Receive) == ServerLogVerbosity.Receive)
        {
            Print(logStr, ConsoleColor.Blue);
        }
    }

    public void PrintSend(string logStr)
    {
        if ((ServerLogVerbosity & ServerLogVerbosity.Send) == ServerLogVerbosity.Send)
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

[Flags]
public enum ServerLogVerbosity
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
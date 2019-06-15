using System;

internal class ServerConsole
{
    static void Main(string[] args)
    {
        ServerLog.Instance.ServerLogVerbosity = ServerLogVerbosity.All;
        ServerLog.Instance.Print("SERVER START");
        Server.SV = new Server("95.169.26.10", 9999);

        ServerLog.Instance.Print("ServerVersion: " + Server.ServerVersion);
        Server.SV.Start();
        while (Console.ReadLine() != "Exit")
        {
        }

        Server.SV.Stop();
    }
}
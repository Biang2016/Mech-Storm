using System;

class ServerConsole
{
    static void Main(string[] args)
    {
        Server.SV.Start();

        while (true)
        {
            ServerLog.Update();
        }

        Server.SV.Stop();
    }
}
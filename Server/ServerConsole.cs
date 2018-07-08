using System;


class ServerConsole
{
    static void Main(string[] args)
    {
        Server.SV.Start();

        while (true)
        {
            if (Console.ReadLine()=="Exit")
            {
                break;
            }
            ServerLog.Update();
        }

        Server.SV.Stop();
    }
}
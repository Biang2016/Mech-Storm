using System;

class ServerConsole
{
    static void Main(string[] args)
    {
        Server.SV.Start();
        while (Console.ReadLine() != "Exit")
        {
            
        }
        Server.SV.Stop();
    }
}
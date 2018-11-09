using System;
using System.Diagnostics;
using System.IO;

class ServerConsole
{
    public static DEVELOP Platform = DEVELOP.TEST;

    public enum DEVELOP
    {
        DEVELOP,
        TEST,
        FORMAL
    }

    static void Main(string[] args)
    {
        switch (Platform)
        {
            case DEVELOP.DEVELOP:
            {
                Server.SV = new Server("127.0.0.1", 9999);

                if (Directory.Exists("./Config"))
                {
                    Directory.Delete("./Config", true);
                }

                Directory.CreateDirectory("./Config");

                Utils.CopyDirectory("../../../Client/Assets/StreamingAssets/Config", "./Config/");
                Utils.CopyDirectory("../../Config", "./Config/");
               
                ServerLog.Print("DEVELOP");
                break;
            }
            case DEVELOP.TEST:
            {
                ServerLog.Print("TEST");
                Server.SV = new Server("127.0.0.1", 9999);
                break;
            }
            case DEVELOP.FORMAL:
            {
                ServerLog.Print("FORMAL");
                Server.SV = new Server("95.169.26.10", 9999);
                break;
            }
        }

        Server.SV.Start();
        while (Console.ReadLine() != "Exit")
        {
        }

        Server.SV.Stop();
    }
}
using System;
using System.IO;

class ServerConsole
{
    public static DEVELOP Platform = DEVELOP.FORMAL;

    public enum DEVELOP
    {
        DEVELOP,
        TEST,
        FORMAL
    }

    static void Main(string[] args)
    {
        StreamReader sr = new StreamReader("./Config/ServerSwitch.txt");
        string platform = sr.ReadToEnd().TrimEnd("\n ".ToCharArray());
        sr.Close();
        Platform = (DEVELOP) Enum.Parse(typeof(DEVELOP), platform);

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

                ServerLog.Print("DEVELOP SERVER");
                break;
            }
            case DEVELOP.TEST:
            {
                ServerLog.Print("TEST SERVER");
                Server.SV = new Server("127.0.0.1", 9999);
                break;
            }
            case DEVELOP.FORMAL:
            {
                ServerLog.Print("FORMAL SERVER");
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

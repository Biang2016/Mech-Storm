using System;
using System.Diagnostics;
using System.IO;

class ServerConsole
{
    static void Main(string[] args)
    {
        DoItSys();




#if DEBUG
        Server.SV= new Server("127.0.0.1", 9999);

        if (Directory.Exists("./Config"))
        {
            Directory.Delete("./Config", true);
        }

        Directory.CreateDirectory("./Config");

        DirectoryInfo di = new DirectoryInfo("../../../Client/Assets/StreamingAssets/Config");
        foreach (FileInfo fileInfo in di.GetFiles())
        {
            File.Copy(fileInfo.FullName, "./Config/" + fileInfo.Name, true);
        }
#else
        Server.SV = new Server("95.169.26.10", 9999);
#endif
        Server.SV.Start();
        while (Console.ReadLine() != "Exit")
        {
        }

        Server.SV.Stop();
    }

    static void DoItSys()
    {
        rm = new System.Random(255);

        PrintSysRandom("Step 1");
        PrintSysRandom("Step 2");

        rm = new System.Random(100);

        PrintSysRandom("Step 3");
        PrintSysRandom("Step 4");

        rm = new System.Random(255);

        PrintSysRandom("Step 5");
        PrintSysRandom("Step 6");

        rm = new System.Random(100);

        PrintSysRandom("Step 7");
        PrintSysRandom("Step 8");
    }

    private static System.Random rm = null;

    static void PrintSysRandom(string label)
    {
        ServerLog.Print(string.Format("{0} - RandomValue {1}", label, rm.Next(0, 100)));
    }

}
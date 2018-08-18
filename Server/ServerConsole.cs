using System;
using System.IO;

class ServerConsole
{
    static void Main(string[] args)
    {
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
}
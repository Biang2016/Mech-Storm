using System;
using System.IO;

class ServerConsole
{
    static void Main(string[] args)
    {
#if DEBUG
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
#endif
        Server.SV.Start();
        while (Console.ReadLine() != "Exit")
        {
        }

        Server.SV.Stop();
    }
}
using System;
using System.IO;

public class FileUtils
{
    public static void CopyDirectory(string src, string dest)
    {
        DirectoryInfo dir = new DirectoryInfo(src);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
        foreach (var info in fileInfo)
        {
            if (info is DirectoryInfo)
            {
                if (!Directory.Exists(dest + "\\" + info.Name))
                {
                    //目标目录下不存在此文件夹即创建子文件夹
                    Directory.CreateDirectory(dest + "\\" + info.Name);
                }

                //递归调用复制子文件夹
                CopyDirectory(info.FullName, dest + "\\" + info.Name);
            }
            else
            {
                File.Copy(info.FullName, dest + "\\" + info.Name, true);
            }
        }
    }

    public static String ByteToReadableFileSize(long size)
    {
        long param = 1024;
        int count = 0;
        float fileSize = size;
        while (fileSize > param && count < 5)
        {
            fileSize = fileSize / param;
            count++;
        }

        String result = String.Format("{0:F}", fileSize);
        switch (count)
        {
            case 0:
                result += "B";
                break;
            case 1:
                result += "KB";
                break;
            case 2:
                result += "MB";
                break;
            case 3:
                result += "GB";
                break;
            case 4:
                result += "TB";
                break;
            case 5:
                result += "PB";
                break;
        }

        return result;
    }
}
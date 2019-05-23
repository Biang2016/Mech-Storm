using System;
using System.IO;
using System.Security.Cryptography;

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

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="filePath">需要创建的目录路径</param>
    public static void CreateDirectory(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            string dirName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="bytes">文件内容</param>
    public static void CreatFile(string filePath, byte[] bytes)
    {
        FileInfo file = new FileInfo(filePath);
        Stream stream = file.Create();

        stream.Write(bytes, 0, bytes.Length);

        stream.Close();
        stream.Dispose();
    }

    public static string GetMD5WithFilePath(string filePath)
    {
        FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hash_byte = md5.ComputeHash(file);
        string str = BitConverter.ToString(hash_byte);
        str = str.Replace("-", "");
        file.Close();
        return str;
    }

    public static string GetAssetsRelativePath(string full_path)
    {
        string normalize_path = NormalizePath(full_path);
        if (normalize_path.IndexOf("/Assets/") < 0)
        {
            return normalize_path;
        }
        else
        {
            return normalize_path.Substring(normalize_path.IndexOf("/Assets/") + 1);
        }
    }

    public static string NormalizePath(string path)
    {
        return path.Replace("\\", "/");
    }


}
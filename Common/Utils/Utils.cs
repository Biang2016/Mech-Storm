using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

public partial class Utils
{
    public static List<Type> GetClassesByNameSpace(string nameSpace)
    {
        Assembly asm = Assembly.GetCallingAssembly();

        List<Type> res = new List<Type>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.Namespace == nameSpace)
                res.Add(type);
        }

        return res;
    }

    public static List<Type> GetClassesByBaseClass(Type baseType)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        List<Type> res = new List<Type>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.IsAbstract) continue;
            if (type.BaseType == baseType)
            {
                res.Add(type);
            }
        }

        return res;
    }

    public static List<Type> GetClassesByGenericClass(Type baseType)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        List<Type> res = new List<Type>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.IsAbstract) continue;
            if (IsParent(type, baseType))
            {
                res.Add(type);
            }
        }

        return res;
    }

    public static bool IsParent(Type test, Type parent)
    {
        if (test == null || parent == null || test == parent || test.BaseType == null)
        {
            return false;
        }

        if (parent.IsInterface)
        {
            foreach (var t in test.GetInterfaces())
            {
                if (t == parent)
                {
                    return true;
                }
            }
        }
        else
        {
            do
            {
                if (test.BaseType == parent)
                {
                    return true;
                }

                test = test.BaseType;
            } while (test != null);
        }

        return false;
    }

    public static string TextToVertical(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char ch in text)
        {
            sb.Append(ch);
            sb.Append("\n");
        }

        return sb.ToString().Trim('\n');
    }

    public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    public static List<T> GetRandomFromList<T>(List<T> OriList, int number)
    {
        if (OriList == null) return new List<T>();
        if (number > OriList.Count) number = OriList.Count;

        HashSet<int> indice = new HashSet<int>();
        Random rd = new Random(DateTime.Now.Millisecond * number);
        while (indice.Count < number)
        {
            int index = rd.Next(0, OriList.Count);
            if (!indice.Contains(index))
            {
                indice.Add(index);
            }
        }

        List<T> res = new List<T>();
        foreach (int i in indice)
        {
            res.Add(OriList[i]);
        }

        return res;
    }

    public static List<T> GetRandomWithProbabilityFromList<T>(List<T> OriList, int number) where T : Probability
    {
        if (OriList == null) return new List<T>();

        int accu = 0;
        SortedDictionary<int, T> resDict = new SortedDictionary<int, T>();
        foreach (T probability in OriList)
        {
            if (probability.Probability > 0)
            {
                accu += probability.Probability;
                resDict.Add(accu, probability);
            }
        }

        System.Random rd = new System.Random(DateTime.Now.Millisecond * number);
        HashSet<T> res = new HashSet<T>();
        while (res.Count < number)
        {
            int index = rd.Next(0, accu);
            foreach (int key in resDict.Keys)
            {
                if (key >= index)
                {
                    T pr = resDict[key];
                    if (!res.Contains(pr))
                    {
                        res.Add(pr);
                    }
                    else
                    {
                        if (!pr.Singleton)
                        {
                            res.Add((T) pr.ProbabilityClone());
                        }
                    }


                    break;
                }
            }
        }

        return res.ToList();
    }

    public static string NormalizeFileSperator(string fullFilename)
    {
        return fullFilename.Replace("\\", "/");
    }

    public static void CopyDirectory(string srcPath, string destPath)
    {
        DirectoryInfo dir = new DirectoryInfo(srcPath);
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i is DirectoryInfo) //判断是否文件夹
            {
                if (!Directory.Exists(destPath + "/" + i.Name))
                {
                    Directory.CreateDirectory(destPath + "/" + i.Name); //目标目录下不存在此文件夹即创建子文件夹
                }

                CopyDirectory(i.FullName, destPath + "/" + i.Name); //递归调用复制子文件夹
            }
            else
            {
                File.Copy(i.FullName, destPath + "/" + i.Name, true); //不是文件夹即复制文件，true表示可以覆盖同名文件
            }
        }
    }

    public static HashSet<T> CloneHashSet<T>(HashSet<T> src)
    {
        HashSet<T> res = new HashSet<T>();
        foreach (T t in src)
        {
            res.Add(t);
        }

        return res;
    }
}
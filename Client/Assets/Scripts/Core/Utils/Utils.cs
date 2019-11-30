using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public partial class Utils
{
    public delegate void DebugLogDelegate(string log);

    public delegate void NoticeCenterMsgDelegate(string noticeStr);

    public static DebugLogDelegate DebugLog;
    public static NoticeCenterMsgDelegate NoticeCenterMsg;

    public static List<Type> GetClassesByNameSpace(string nameSpace, Assembly assembly)
    {
        List<Type> res = new List<Type>();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.Namespace == nameSpace)
                res.Add(type);
        }

        return res;
    }

    public static List<Type> GetClassesByBaseClass(Type baseType, Assembly assembly)
    {
        List<Type> res = new List<Type>();

        foreach (Type type in assembly.GetTypes())
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
            if (IsBaseType(type, baseType))
            {
                res.Add(type);
            }
        }

        return res;
    }

    public static bool IsBaseType(Type type, Type baseType)
    {
        if (type == null || baseType == null || type == baseType || type.BaseType == null)
        {
            return false;
        }

        if (baseType.IsInterface)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t == baseType)
                {
                    return true;
                }
            }
        }
        else
        {
            do
            {
                if (type.BaseType == baseType)
                {
                    return true;
                }

                type = type.BaseType;
            } while (type != null);
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

    public static List<T> GetRandomFromList<T>(List<T> OriList, int number, List<T> exceptList = null)
    {
        if (OriList == null || OriList.Count == 0) return new List<T>();

        List<T> ori = OriList.ToArray().ToList();
        if (exceptList != null)
        {
            List<T> remove = new List<T>();
            foreach (T t in ori)
            {
                if (exceptList.Contains(t))
                {
                    remove.Add(t);
                }
            }

            foreach (T removeT in remove)
            {
                ori.Remove(removeT);
            }
        }

        if (number > ori.Count) number = ori.Count;

        HashSet<int> indices = new HashSet<int>();
        Random rd = new Random(DateTime.Now.Millisecond * number);
        while (indices.Count < number)
        {
            int index = rd.Next(0, ori.Count);
            if (!indices.Contains(index))
            {
                indices.Add(index);
            }
        }

        List<T> res = new List<T>();
        foreach (int i in indices)
        {
            res.Add(ori[i]);
        }

        return res;
    }

    public static List<T> GetRandomWithProbabilityFromList<T>(List<T> OriList, int number) where T : Probability
    {
        if (OriList == null || OriList.Count == 0) return new List<T>();

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
                        if (!pr.IsSingleton)
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

    public static string HighlightStringFormat(string src, string color, params object[] args)
    {
        string[] coloredStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            coloredStrings[i] = "<" + color + ">" + args[i] + "</color>";
        }

        return string.Format(src, coloredStrings);
    }

    public static string HighlightStringFormat(string src, string color, bool[] needTint, params object[] args)
    {
        string[] coloredStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            if (needTint[i])
            {
                coloredStrings[i] = "<" + color + ">" + args[i] + "</color>";
            }
            else
            {
                coloredStrings[i] = args[i].ToString();
            }
        }

        return string.Format(src, coloredStrings);
    }

    public static string AddHighLightColorToText(string highLightText, string color)
    {
        return "<" + color + ">" + highLightText + "</color>";
    }

    private static string colorStringPattern = @"(.*)(<#)([0-9a-fA-F]{6,8}>.*</color>)(.*)";

    public static string TextMeshProColorStringConvertToText(string colorString)
    {
        Regex rg = new Regex(colorStringPattern);
        if (rg.IsMatch(colorString))
        {
            string replace = colorString;
            while (rg.IsMatch(replace))
            {
                replace = rg.Replace(replace, "$1<color=#$3$4");
            }

            return replace;
        }

        return colorString;
    }
}
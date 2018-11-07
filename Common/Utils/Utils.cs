using System;
using System.Collections.Generic;
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
        Random rd = new Random();
        while (indice.Count < number)
        {
            int index = rd.Next(0, number);
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
}
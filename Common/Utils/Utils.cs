using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class Utils
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
}
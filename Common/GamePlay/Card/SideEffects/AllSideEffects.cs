using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllSideEffects
{
    public static Dictionary<string, SideEffectBase> SideEffectsNameDict = new Dictionary<string, SideEffectBase>();

    public static void Reset()
    {
        SideEffectsNameDict.Clear();
    }

    public static SideEffectBase GetSideEffect(string sideEffectName)
    {
        if (SideEffectsNameDict.ContainsKey(sideEffectName))
        {
            return SideEffectsNameDict[sideEffectName];
        }
        else
        {
            return null;
        }
    }

    private static void addSideEffect(SideEffectBase sideEffectBase)
    {
        if (!SideEffectsNameDict.ContainsKey(sideEffectBase.Name)) SideEffectsNameDict.Add(sideEffectBase.Name, sideEffectBase);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllSideEffects(string sideEffectsXMLPath)
    {
        Reset();
        SortedDictionary<string, string> descKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            descKeyDict[strName] = "desc_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(sideEffectsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        if (CurrentAssembly == null) CurrentAssembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allSideEffects = doc.DocumentElement;
        for (int i = 0; i < allSideEffects.ChildNodes.Count; i++)
        {
            XmlNode sideEffectNode = allSideEffects.ChildNodes.Item(i);

            string name = sideEffectNode.Attributes["name"].Value;
            SideEffectBase se = (SideEffectBase) CurrentAssembly.CreateInstance("SideEffects." + name);
            if (se == null)
            {
                Utils.DebugLog("SideEffects: " + name + " does not exist");
                continue;
            }

            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaws = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in descKeyDict)
            {
                se.DescRaws[kv.Key] = sideEffectNode.Attributes[kv.Value].Value;
            }

            addSideEffect(se);
        }
    }
}
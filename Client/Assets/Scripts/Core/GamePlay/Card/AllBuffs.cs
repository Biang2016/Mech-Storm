using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using SideEffects;

public static class AllBuffs
{
    public static string BuffsXMLFile => LoadAllBasicXMLFiles.ConfigFolderPath+ "/Basic/Buffs.xml";

    public static Dictionary<string, PlayerBuffSideEffects> BuffDict = new Dictionary<string, PlayerBuffSideEffects>();

    public static void Reset()
    {
        BuffDict.Clear();
    }

    private static void addBuff(PlayerBuffSideEffects se)
    {
        if (!BuffDict.ContainsKey(se.Name)) BuffDict.Add(se.Name, se);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllBuffs()
    {
        Reset();
        SortedDictionary<string, string> descKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            descKeyDict[strName] = "desc_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(BuffsXMLFile))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement node_AllBuffs = doc.DocumentElement;
        for (int i = 0; i < node_AllBuffs.ChildNodes.Count; i++)
        {
            XmlNode node_Buff = node_AllBuffs.ChildNodes.Item(i);

            string name = node_Buff.Attributes["name"].Value;
            PlayerBuffSideEffects se = (PlayerBuffSideEffects) CurrentAssembly.CreateInstance("SideEffects." + name);
            if (se == null)
            {
                Utils.DebugLog("Buffs: " + name + " does not exist");
                continue;
            }

            se.Name = node_Buff.Attributes["name"].Value;
            se.DescRaws = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in descKeyDict)
            {
                se.DescRaws[kv.Key] = node_Buff.Attributes[kv.Value].Value;
            }

            addBuff(se);
        }

        foreach (KeyValuePair<string, SideEffectBase> kv in AllSideEffects.SideEffectsNameDict)
        {
            if (kv.Value is AddPlayerBuff_Base addPlayerBuffBase)
            {
                addPlayerBuffBase.BuffName = "DoSthWhenTrigger_RemoveBySomeTime";
            }
        }
    }

    public static PlayerBuffSideEffects GetBuff(string buffName)
    {
        if (BuffDict.ContainsKey(buffName))
        {
            return (PlayerBuffSideEffects) BuffDict[buffName].Clone();
        }
        else
        {
            return null;
        }
    }
}
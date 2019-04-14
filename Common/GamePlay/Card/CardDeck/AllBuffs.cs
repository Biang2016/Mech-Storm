using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllBuffs
{
    private static Dictionary<string, SideEffectExecute> BuffDict = new Dictionary<string, SideEffectExecute>();

    public static void Reset()
    {
        BuffDict.Clear();
    }

    private static void addBuff(SideEffectExecute seb)
    {
        if (!BuffDict.ContainsKey(seb.SideEffectBase.Name)) BuffDict.Add(seb.SideEffectBase.Name, seb);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllBuffs(string buffsXMLPath)
    {
        Reset();
        SortedDictionary<string, string> descKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            descKeyDict[strName] = "desc_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(buffsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        if (CurrentAssembly == null) CurrentAssembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allBuffs = doc.DocumentElement;
        for (int i = 0; i < allBuffs.ChildNodes.Count; i++)
        {
            XmlNode sideEffectNode = allBuffs.ChildNodes.Item(i);

            string name = sideEffectNode.Attributes["name"].Value;
            PlayerBuffSideEffects se = (PlayerBuffSideEffects) CurrentAssembly.CreateInstance("SideEffects." + name);
            if (se == null)
            {
                Utils.DebugLog("Buffs: " + name + " does not exist");
                continue;
            }

            se.Name = sideEffectNode.Attributes["name"].Value;

            se.DescRaws = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in descKeyDict)
            {
                se.DescRaws[kv.Key] = sideEffectNode.Attributes[kv.Value].Value;
            }

            se.M_SideEffectParam.GetParamsFromXMLNode(sideEffectNode);
            SideEffectExecute see = new SideEffectExecute(
                SideEffectExecute.SideEffectFrom.Buff, se,
                (SideEffectBundle.TriggerTime) se.M_SideEffectParam.GetParam_ConstInt("TriggerTime"),
                (SideEffectBundle.TriggerRange) se.M_SideEffectParam.GetParam_ConstInt("TriggerRange"),
                se.M_SideEffectParam.GetParam_ConstInt("TriggerDelayTimes"),
                se.M_SideEffectParam.GetParam_ConstInt("TriggerTimes"),
                (SideEffectBundle.TriggerTime) se.M_SideEffectParam.GetParam_ConstInt("RemoveTriggerTime"),
                (SideEffectBundle.TriggerRange) se.M_SideEffectParam.GetParam_ConstInt("RemoveTriggerRange"),
                se.M_SideEffectParam.GetParam_ConstInt("RemoveTriggerTimes"));
            for (int k = 0; k < sideEffectNode.ChildNodes.Count; k++)
            {
                XmlNode buffInfo = sideEffectNode.ChildNodes[k];
                SideEffectBase buff = AllSideEffects.SideEffectsNameDict[buffInfo.Attributes["name"].Value].Clone();
                AllCards.GetInfoForSideEffect(buffInfo, buff);

                see.SideEffectBase.Sub_SideEffect.Add(buff);
            }

            addBuff(see);
        }
    }

    public static SideEffectExecute GetBuff(string buffName)
    {
        if (BuffDict.ContainsKey(buffName))
        {
            return BuffDict[buffName].Clone();
        }
        else
        {
            return null;
        }
    }
}
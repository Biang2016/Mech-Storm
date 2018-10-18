using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllBuffs
{
    public static Dictionary<string, SideEffectExecute> BuffDict = new Dictionary<string, SideEffectExecute>();

    private static void addBuff(SideEffectExecute seb)
    {
        BuffDict.Add(seb.SideEffectBase.Name, seb);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllBuffs(string buffsXMLPath)
    {
        string text;
        using (StreamReader sr = new StreamReader(buffsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        CurrentAssembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allBuffs = doc.DocumentElement;
        for (int i = 0; i < allBuffs.ChildNodes.Count; i++)
        {
            XmlNode sideEffectNode = allBuffs.ChildNodes.Item(i);

            PlayerBuffSideEffects se = (PlayerBuffSideEffects) CurrentAssembly.CreateInstance("SideEffects." + sideEffectNode.Attributes["name"].Value);
            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaw = sideEffectNode.Attributes["desc"].Value;
            se.DescRaw_en = sideEffectNode.Attributes["desc_en"].Value;

            SideEffectBundle.TriggerTime triggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), sideEffectNode.Attributes["triggerTime"].Value);
            SideEffectBundle.TriggerRange triggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), sideEffectNode.Attributes["triggerRange"].Value);
            int triggerDelayTimes = int.Parse(sideEffectNode.Attributes["triggerDelayTimes"].Value);
            int triggerTimes = int.Parse(sideEffectNode.Attributes["triggerTimes"].Value);
            SideEffectBundle.TriggerTime removeTriggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), sideEffectNode.Attributes["removeTriggerTime"].Value);
            SideEffectBundle.TriggerRange removeTriggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), sideEffectNode.Attributes["removeTriggerRange"].Value);
            int removeTriggerTimes = int.Parse(sideEffectNode.Attributes["removeTriggerTimes"].Value);

            SideEffectExecute see = new SideEffectExecute(se, triggerTime, triggerRange, triggerDelayTimes, triggerTimes, removeTriggerTime, removeTriggerRange, removeTriggerTimes);

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
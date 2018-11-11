using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllBuffs
{
    public static Dictionary<string, SideEffectExecute> BuffDict = new Dictionary<string, SideEffectExecute>();

    public delegate void DebugLog(string log);

    public static DebugLog DebugLogHandler;


    private static void addBuff(SideEffectExecute seb)
    {
        if (!BuffDict.ContainsKey(seb.SideEffectBase.Name)) BuffDict.Add(seb.SideEffectBase.Name, seb);
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

            string name = sideEffectNode.Attributes["name"].Value;
            PlayerBuffSideEffects se = (PlayerBuffSideEffects) CurrentAssembly.CreateInstance("SideEffects." + name);
            if (se == null)
            {
                DebugLogHandler("Buffs: " + name + " does not exist");
                continue;
            }

            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaw = sideEffectNode.Attributes["desc"].Value;
            se.DescRaw_en = sideEffectNode.Attributes["desc_en"].Value;

            se.BuffColor = sideEffectNode.Attributes["BuffColor"].Value;
            se.BuffPicId = int.Parse(sideEffectNode.Attributes["BuffPicId"].Value);
            se.HasNumberShow = sideEffectNode.Attributes["HasNumberShow"].Value == "True";
            se.CanPiled = sideEffectNode.Attributes["CanPiled"].Value == "True";
            se.Singleton = sideEffectNode.Attributes["Singleton"].Value == "True";
            se.PiledBy = (PlayerBuffSideEffects.BuffPiledBy) Enum.Parse(typeof(PlayerBuffSideEffects.BuffPiledBy), sideEffectNode.Attributes["PiledBy"].Value);

            se.TriggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), sideEffectNode.Attributes["TriggerTime"].Value);
            se.TriggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), sideEffectNode.Attributes["TriggerRange"].Value);
            se.TriggerDelayTimes = int.Parse(sideEffectNode.Attributes["TriggerDelayTimes"].Value);
            se.TriggerTimes = int.Parse(sideEffectNode.Attributes["TriggerTimes"].Value);
            se.RemoveTriggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), sideEffectNode.Attributes["RemoveTriggerTime"].Value);
            se.RemoveTriggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), sideEffectNode.Attributes["RemoveTriggerRange"].Value);
            se.RemoveTriggerTimes = int.Parse(sideEffectNode.Attributes["RemoveTriggerTimes"].Value);

            SideEffectExecute see = new SideEffectExecute(SideEffectExecute.SideEffectFrom.Buff, se, se.TriggerTime, se.TriggerRange, se.TriggerDelayTimes, se.TriggerTimes, se.RemoveTriggerTime, se.RemoveTriggerRange, se.RemoveTriggerTimes);

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
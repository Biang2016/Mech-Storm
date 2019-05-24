using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllSideEffects
{
    public static string SideEffectsXMLFile => LoadAllBasicXMLFiles.ConfigFolderPath+ "/Basic/SideEffects.xml";

    public static Dictionary<string, SideEffectBase> SideEffectsNameDict = new Dictionary<string, SideEffectBase>();

    public static void Reset()
    {
        SideEffectsNameDict.Clear();
    }

    public static SideEffectBase GetSideEffect(string sideEffectName)
    {
        if (SideEffectsNameDict.ContainsKey(sideEffectName))
        {
            return SideEffectsNameDict[sideEffectName].Clone();
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

    public static void AddAllSideEffects()
    {
        Reset();
        SortedDictionary<string, string> descKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            descKeyDict[strName] = "desc_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(SideEffectsXMLFile))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement node_AllSideEffects = doc.DocumentElement;
        for (int i = 0; i < node_AllSideEffects.ChildNodes.Count; i++)
        {
            XmlNode node_SideEffect = node_AllSideEffects.ChildNodes.Item(i);

            string name = node_SideEffect.Attributes["name"].Value;
            SideEffectBase se = (SideEffectBase) CurrentAssembly.CreateInstance("SideEffects." + name);
            
            if (se == null)
            {
                Utils.DebugLog("SideEffects: " + name + " does not exist");
                continue;
            }

            se.Name = node_SideEffect.Attributes["name"].Value;
            se.DescRaws = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in descKeyDict)
            {
                se.DescRaws[kv.Key] = node_SideEffect.Attributes[kv.Value].Value;
            }

            if (se is TargetSideEffect t_se)
            {
                t_se.M_SideEffectParam.GetParam("TargetSelect").SetValue(t_se.ValidTargetSelects[0].ToString());
                Dictionary<TargetSelect, List<TargetRange>> validTargetRangeDict = TargetSelector.TargetSelectorPresets[t_se.TargetSelectorType];
                TargetRange validTargetRange = validTargetRangeDict[t_se.ValidTargetSelects[0]][0];
                t_se.M_SideEffectParam.GetParam("TargetRange").SetValue(validTargetRange.ToString());
            }

            addSideEffect(se);
        }
    }
}
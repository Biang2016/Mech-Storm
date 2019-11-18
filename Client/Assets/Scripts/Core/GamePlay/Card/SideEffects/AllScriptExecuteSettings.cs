using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllScriptExecuteSettings
{
    public static string ScriptExecuteSettingsXMLFile => LoadAllBasicXMLFiles.ConfigFolderPath + "/Basic/ScriptExecuteSettings.xml";

    public static Dictionary<string, ScriptExecuteSettingBase> ScriptExecuteSettingsNameDict = new Dictionary<string, ScriptExecuteSettingBase>();

    public static void Reset()
    {
        ScriptExecuteSettingsNameDict.Clear();
    }

    public static ScriptExecuteSettingBase GetScriptExecuteSetting(string scriptExecuteSettingName)
    {
        if (ScriptExecuteSettingsNameDict.ContainsKey(scriptExecuteSettingName))
        {
            return (ScriptExecuteSettingBase) ScriptExecuteSettingsNameDict[scriptExecuteSettingName].Clone();
        }
        else
        {
            return null;
        }
    }

    private static void addScriptExecuteSetting(ScriptExecuteSettingBase scriptExecuteSettingBase)
    {
        if (!ScriptExecuteSettingsNameDict.ContainsKey(scriptExecuteSettingBase.Name)) ScriptExecuteSettingsNameDict.Add(scriptExecuteSettingBase.Name, scriptExecuteSettingBase);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllScriptExecuteSettings()
    {
        Reset();
        SortedDictionary<string, string> descKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            descKeyDict[strName] = "desc_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(ScriptExecuteSettingsXMLFile))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement node_AllScriptExecuteSettings = doc.DocumentElement;
        for (int i = 0; i < node_AllScriptExecuteSettings.ChildNodes.Count; i++)
        {
            XmlNode node_ScriptExecuteSetting = node_AllScriptExecuteSettings.ChildNodes.Item(i);

            string name = node_ScriptExecuteSetting.Attributes["name"].Value;
            ScriptExecuteSettingBase se = (ScriptExecuteSettingBase) CurrentAssembly.CreateInstance("ScriptExecuteSettings." + name);

            if (se == null)
            {
                Utils.DebugLog("ScriptExecuteSettings: " + name + " does not exist");
                continue;
            }

            se.Name = node_ScriptExecuteSetting.Attributes["name"].Value;
            se.DescRaws = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in descKeyDict)
            {
                se.DescRaws[kv.Key] = node_ScriptExecuteSetting.Attributes[kv.Value].Value;
            }

            addScriptExecuteSetting(se);
        }
    }
}
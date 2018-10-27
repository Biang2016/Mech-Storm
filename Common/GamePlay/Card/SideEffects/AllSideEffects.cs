using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllSideEffects
{
    public static Dictionary<string, SideEffectBase> SideEffectsNameDict = new Dictionary<string, SideEffectBase>();

    public delegate void DebugLog(string log);

    public static DebugLog DebugLogHandler;


    private static void addSideEffect(SideEffectBase sideEffectBase)
    {
        if (!SideEffectsNameDict.ContainsKey(sideEffectBase.Name)) SideEffectsNameDict.Add(sideEffectBase.Name, sideEffectBase);
    }

    public static Assembly CurrentAssembly;

    public static void AddAllSideEffects(string sideEffectsXMLPath)
    {
        string text;
        using (StreamReader sr = new StreamReader(sideEffectsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        CurrentAssembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
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
                DebugLogHandler("SideEffects: " + name + " does not exist");
                continue;
            }

            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaw = sideEffectNode.Attributes["desc"].Value;
            se.DescRaw_en = sideEffectNode.Attributes["desc_en"].Value;
            addSideEffect(se);
        }
    }
}
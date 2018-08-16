using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllSideEffects
{
    public static Dictionary<int, SideEffectBase> SideEffectsDict = new Dictionary<int, SideEffectBase>();
    public static Dictionary<string, SideEffectBase> SideEffectsNameDict = new Dictionary<string, SideEffectBase>();

    private static void addSideEffect(SideEffectBase sideEffectBase)
    {
        SideEffectsDict.Add(sideEffectBase.SideEffectID, sideEffectBase);
        SideEffectsNameDict.Add(sideEffectBase.Name, sideEffectBase);
    }

    public static void AddAllSideEffects(string sideEffectsXMLPath)
    {
        string text;
        using (StreamReader sr = new StreamReader(sideEffectsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        Assembly assembly = Assembly.GetCallingAssembly(); // 获取当前程序集 
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allSideEffects = doc.DocumentElement;
        for (int i = 0; i < allSideEffects.ChildNodes.Count; i++)
        {
            XmlNode sideEffectNode = allSideEffects.ChildNodes.Item(i);

            SideEffectBase se = (SideEffectBase) assembly.CreateInstance("SideEffects." + sideEffectNode.Attributes["name"].Value);
            se.SideEffectID = int.Parse(sideEffectNode.Attributes["id"].Value);
            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaw = sideEffectNode.Attributes["desc"].Value;
            addSideEffect(se);
        }
    }
}
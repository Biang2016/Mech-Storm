using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using MyCardGameCommon.Net.GamePlay.Card;

public static class AllSideEffects
{
    public static Dictionary<int, SideEffectBase> SideEffectsDict = new Dictionary<int, SideEffectBase>();
    public static Dictionary<string, SideEffectBase> SideEffectsNameDict = new Dictionary<string, SideEffectBase>();

    private static void addSideEffect(SideEffectBase sideEffectBase)
    {
        SideEffectsDict.Add(sideEffectBase.SideEffectID, sideEffectBase);
        SideEffectsNameDict.Add(sideEffectBase.Name, sideEffectBase);
    }

    public static void AddAllSideEffects()
    {
        Assembly assembly = Assembly.GetCallingAssembly(); // 获取当前程序集 

        XmlDocument doc = new XmlDocument();
        string text = CardResource.SideEffects;
        doc.LoadXml(text);
        XmlElement allSideEffects = doc.DocumentElement;
        for (int i = 0; i < allSideEffects.ChildNodes.Count; i++)
        {
            XmlNode sideEffectNode = allSideEffects.ChildNodes.Item(i);

            SideEffectBase se = (SideEffectBase) assembly.CreateInstance(sideEffectNode.Attributes["name"].Value);
            se.SideEffectID = int.Parse(sideEffectNode.Attributes["id"].Value);
            se.Name = sideEffectNode.Attributes["name"].Value;
            se.DescRaw = sideEffectNode.Attributes["desc"].Value;
            addSideEffect(se);
        }
    }
}
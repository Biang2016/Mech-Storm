using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyCardGameCommon;

public class AllSideEffects
{
    public static Dictionary<int, SideEffectBase> SideEffectsDict = new Dictionary<int, SideEffectBase>();

    private static void addSideEffect(SideEffectBase sideEffectBase)
    {
        SideEffectsDict.Add(sideEffectBase.SideEffectID, sideEffectBase);
    }

    public static void AddAllSideEffects()
    {
        XmlDocument doc = new XmlDocument();
        string text = CardResource.SideEffects;
        doc.LoadXml(text);
        XmlElement allSideEffects = doc.DocumentElement;
        for (int i = 0; i < allSideEffects.ChildNodes.Count; i++)
        {
            SideEffectBase sideEffectBase = new SideEffectBase();
            XmlNode sideEffect = allSideEffects.ChildNodes.Item(i);

            sideEffectBase.SideEffectID = int.Parse(sideEffect.Attributes["id"].Value);
            sideEffectBase.Name = sideEffect.Attributes["name"].Value;
            sideEffectBase.Desc = sideEffect.Attributes["desc"].Value;
        }
    }

    public class SideEffectBase
    {
        public int SideEffectID;
        public string Name;
        public string Desc;
    }
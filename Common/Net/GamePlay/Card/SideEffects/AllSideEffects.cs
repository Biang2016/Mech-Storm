using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MyCardGameCommon;

public class AllSideEffects
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
        XmlDocument doc = new XmlDocument();
        string text = CardResource.SideEffects;
        doc.LoadXml(text);
        XmlElement allSideEffects = doc.DocumentElement;
        for (int i = 0; i < allSideEffects.ChildNodes.Count; i++)
        {
            SideEffectBase sideEffect = new SideEffectBase();
            XmlNode sideEffectNode = allSideEffects.ChildNodes.Item(i);

            sideEffect.SideEffectID = int.Parse(sideEffectNode.Attributes["id"].Value);
            sideEffect.Name = sideEffectNode.Attributes["name"].Value;
            sideEffect.Desc = sideEffectNode.Attributes["desc"].Value;
            for (int j = 0; j < sideEffectNode.ChildNodes.Count; j++)
            {
                XmlNode param = sideEffectNode.ChildNodes[j];
                switch (param.Attributes["type"].Value)
                {
                    case "int":
                        sideEffect.Params.Add(new SideEffectBase.Param(param.Attributes["name"].Value, int.Parse(param.Attributes["value"].Value)));
                        break;
                    case "string":
                        sideEffect.Params.Add(new SideEffectBase.Param(param.Attributes["name"].Value, param.Attributes["value"].Value));
                        break;
                    default: break;
                }
            }

            addSideEffect(sideEffect);
        }
    }
}


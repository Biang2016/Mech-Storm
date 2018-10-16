using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllColors
{
    public static Dictionary<ColorType, string> ColorDict = new Dictionary<ColorType, string>();

    public enum ColorType
    {
        SoldierCardColor,
        HeroCardColor,
        EnergyCardColor,
        SpellCardColor,
        WeaponCardColor,
        ShieldCardColor,
        PackCardColor,
        MACardColor,
        CardHightLightColor,
        CardImportantColor,
    }

    public static void AddAllColors(string colorXMLPath)
    {
        string text;
        using (StreamReader sr = new StreamReader(colorXMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allColors = doc.DocumentElement;
        for (int i = 0; i < allColors.ChildNodes.Count; i++)
        {
            XmlNode colorNode = allColors.ChildNodes.Item(i);
            ColorType colorType = (ColorType) Enum.Parse(typeof(ColorType), colorNode.Attributes["name"].Value);
            string color = colorNode.Attributes["value"].Value;
            ColorDict.Add(colorType, color);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public static class AllColors
{
    public static Dictionary<ColorType, string> ColorDict = new Dictionary<ColorType, string>();
    public static Dictionary<ColorType, float> IntensityDict = new Dictionary<ColorType, float>();


    [JsonConverter(typeof(StringEnumConverter))]
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
        CardDecsTextColor,
        StoryLevelColor1,
        StoryLevelColor2,
        StoryLevelColor3,
        StoryLevelColor4,
        StoryLevelColor5,
        StoryLevelColor6,
        StoryLevelColor7,
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
            string color = colorNode.Attributes["color"].Value;
            float intensity = float.Parse(colorNode.Attributes["intensity"].Value);
            if (!ColorDict.ContainsKey(colorType)) ColorDict.Add(colorType, color);
            if (!IntensityDict.ContainsKey(colorType)) IntensityDict.Add(colorType, intensity);
        }
    }
}
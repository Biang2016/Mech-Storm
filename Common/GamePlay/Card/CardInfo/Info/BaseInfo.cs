using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BaseInfo
{
    public int PictureID;
    public SortedDictionary<string, string> CardNames;
    public bool IsTemp;//临时卡，如卡牌的召唤物、临时复制等。无法在选牌界面看见
    public bool IsHide;//隐藏卡，如boss专用卡，打败boss后的特殊奖励等。无法在选牌界面看见
    public int Metal;
    public int Energy;
    public int Coin;
    public int EffectFactor;
    public int LimitNum;
    public int CardRareLevel;
    public DragPurpose DragPurpose;
    public CardTypes CardType;

    public BaseInfo(int pictureID, SortedDictionary<string, string> cardNames, bool isTemp, bool isHide, int metal, int energy, int coin, int effectFactor, int limitNum, int cardRareLevel, CardTypes cardType)
    {
        PictureID = pictureID;
        CardNames = cardNames;
        IsTemp = isTemp;
        IsHide = isHide;
        Metal = metal;
        Energy = energy;
        Coin = coin;
        EffectFactor = effectFactor;
        LimitNum = limitNum;
        CardRareLevel = cardRareLevel;
        DragPurpose = DragPurpose.None;
        CardType = cardType;
    }

    private static string GetHighLightColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardHighLightColor];
    }

    private static string GetImportantColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardImportantColor];
    }

    public static string AddHighLightColorToText(string highLightText)
    {
        return "<" + GetHighLightColor() + ">" + highLightText + "</color>";
    }

    public static string AddImportantColorToText(string highLightText)
    {
        return "<" + GetImportantColor() + ">" + highLightText + "</color>";
    }

    public float BaseValue()
    {
        return Energy * 3 + Metal * ((float) Coin / 100);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(PictureID);
        writer.WriteSInt32(CardNames.Count);
        foreach (KeyValuePair<string, string> kv in CardNames)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }

        writer.WriteByte(IsTemp ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsHide ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(Metal);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(Coin);
        writer.WriteSInt32(EffectFactor);
        writer.WriteSInt32(LimitNum);
        writer.WriteSInt32(CardRareLevel);
        writer.WriteSInt32((int) CardType);
    }

    public static BaseInfo Deserialze(DataStream reader)
    {
        int PictureID = reader.ReadSInt32();
        int descRawCount = reader.ReadSInt32();
        SortedDictionary<string, string> CardNames = new SortedDictionary<string, string>();
        for (int i = 0; i < descRawCount; i++)
        {
            string ls = reader.ReadString8();
            string value = reader.ReadString8();
            CardNames[ls] = value;
        }

        bool IsTemp = reader.ReadByte() == 0x01;
        bool IsHide = reader.ReadByte() == 0x01;
        int Metal = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int Coin = reader.ReadSInt32();
        int EffectFactor = reader.ReadSInt32();
        int LimitNum = reader.ReadSInt32();
        int CardRareLevel = reader.ReadSInt32();
        CardTypes CardType = (CardTypes) reader.ReadSInt32();
        return new BaseInfo(PictureID, CardNames, IsTemp, IsHide, Metal, Energy, Coin, EffectFactor, LimitNum, CardRareLevel, CardType);
    }

    public static Dictionary<string, Dictionary<CardTypes, string>> CardTypeNameDict = new Dictionary<string, Dictionary<CardTypes, string>>
    {
        {
            "zh", new Dictionary<CardTypes, string>
            {
                {CardTypes.Mech, "机甲牌"},
                {CardTypes.Spell, "法术牌"},
                {CardTypes.Energy, "能量牌"},
                {CardTypes.Equip, "装备牌"},
            }
        },
        {
            "en", new Dictionary<CardTypes, string>
            {
                {CardTypes.Mech, "Mech"},
                {CardTypes.Spell, "Spell"},
                {CardTypes.Energy, "Energy"},
                {CardTypes.Equip, "Equip"}, 
            }
        }
    };
}

[JsonConverter(typeof(StringEnumConverter))]
public enum CardTypes
{
    Mech,
    Spell,
    Energy,
    Equip,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum DragPurpose
{
    None = 0,
    Summon = 1,
    Equip = 2,
    Target = 3,
}
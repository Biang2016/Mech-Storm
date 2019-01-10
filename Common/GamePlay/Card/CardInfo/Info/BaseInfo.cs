using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BaseInfo
{
    public int PictureID;
    public string CardName;
    public string CardName_en;
    public string CardDescRaw;
    public bool IsTemp;
    public bool Hide;
    public int Metal;
    public int Energy;
    public int Coin;
    public int EffectFactor;
    public int LimitNum;
    public int CardRareLevel;
    public DragPurpose DragPurpose;
    public CardTypes CardType;

    public BaseInfo(int pictureID, string cardName, string cardName_en, string cardDescRaw, bool isTemp, bool hide, int metal, int energy, int coin, int effectFactor, int limitNum, int cardRareLevel, CardTypes cardType)
    {
        PictureID = pictureID;
        CardName = cardName;
        CardName_en = cardName_en;
        CardDescRaw = cardDescRaw;
        IsTemp = isTemp;
        Hide = hide;
        Metal = metal;
        Energy = energy;
        Coin = coin;
        EffectFactor = effectFactor;
        LimitNum = limitNum;
        CardRareLevel = cardRareLevel;
        DragPurpose = DragPurpose.None;
        CardType = cardType;
    }

    private static string GetHightLightColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardHightLightColor];
    }

    private static string GetImportantColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardImportantColor];
    }

    public static string AddHightLightColorToText(string hightLightText)
    {
        return "<color=\"" + GetHightLightColor() + "\">" + hightLightText + "</color>";
    }

    public static string AddImportantColorToText(string hightLightText)
    {
        return "<color=\"" + GetImportantColor() + "\">" + hightLightText + "</color>";
    }

    public float BaseValue()
    {
        return Energy * 3 + Metal * ((float) Coin / 100);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(PictureID);
        writer.WriteString8(CardName);
        writer.WriteString8(CardName_en);
        writer.WriteString8(CardDescRaw);
        writer.WriteByte(Hide ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsTemp ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(Metal);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(Coin);
        writer.WriteSInt32(EffectFactor);
        writer.WriteSInt32(LimitNum);
        writer.WriteSInt32(CardRareLevel);
        writer.WriteSInt32((int) DragPurpose);
        writer.WriteSInt32((int) CardType);
    }

    public static BaseInfo Deserialze(DataStream reader)
    {
        int PictureID = reader.ReadSInt32();
        string CardName = reader.ReadString8();
        string CardName_en = reader.ReadString8();
        string CardDesc = reader.ReadString8();
        bool IsTemp = reader.ReadByte() == 0x01;
        bool Hide = reader.ReadByte() == 0x01;
        int Metal = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int Coin = reader.ReadSInt32();
        int EffectFactor = reader.ReadSInt32();
        int LimitNum = reader.ReadSInt32();
        int CardRareLevel = reader.ReadSInt32();
        DragPurpose DragPurpose = (DragPurpose) reader.ReadSInt32();
        CardTypes CardType = (CardTypes) reader.ReadSInt32();
        return new BaseInfo(PictureID, CardName, CardName_en, CardDesc, IsTemp, Hide, Metal, Energy, Coin, EffectFactor, LimitNum, CardRareLevel, CardType);
    }

    public static Dictionary<CardTypes, string> CardTypeNameDict_en = new Dictionary<CardTypes, string>
    {
        {CardTypes.Retinue, "Mech"},
        {CardTypes.Spell, "Spell"},
        {CardTypes.Energy, "Energy"},
        {CardTypes.Equip, "Equip"},
    };

    public static Dictionary<CardTypes, string> CardTypeNameDict = new Dictionary<CardTypes, string>
    {
        {CardTypes.Retinue, "机甲牌"},
        {CardTypes.Spell, "法术牌"},
        {CardTypes.Energy, "能量牌"},
        {CardTypes.Equip, "装备牌"},
    };
}

[JsonConverter(typeof(StringEnumConverter))]
public enum CardTypes
{
    Retinue,
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
public struct BaseInfo
{
    public int PictureID;
    public string CardName;
    public string CardName_en;
    public string CardDescRaw;
    public int Metal;
    public int Energy;
    public int Coin;
    public int EffectFactor;
    public DragPurpose DragPurpose;
    public CardTypes CardType;
    public string CardColor;
    public string HightLightColor;

    public BaseInfo(int pictureID, string cardName, string cardName_en, string cardDescRaw, int metal, int energy, int coin, int effectFactor, DragPurpose dragPurpose, CardTypes cardType, string cardColor, string hightLightColor)
    {
        PictureID = pictureID;
        CardName = cardName;
        CardName_en = cardName_en;
        CardDescRaw = cardDescRaw;
        Metal = metal;
        Energy = energy;
        Coin = coin;
        EffectFactor = effectFactor;
        DragPurpose = dragPurpose;
        CardType = cardType;
        CardColor = cardColor;
        HightLightColor = hightLightColor;
    }

    public static string AddHightLightColorToText(string hightLightColor, string hightLightText)
    {
        return "<color=\"" + hightLightColor + "\">" + hightLightText + "</color>";
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(PictureID);
        writer.WriteString8(CardName);
        writer.WriteString8(CardName_en);
        writer.WriteString8(CardDescRaw);
        writer.WriteSInt32(Metal);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(Coin);
        writer.WriteSInt32(EffectFactor);
        writer.WriteSInt32((int) DragPurpose);
        writer.WriteSInt32((int) CardType);
        writer.WriteString8(CardColor);
        writer.WriteString8(HightLightColor);
    }

    public static BaseInfo Deserialze(DataStream reader)
    {
        int PictureID = reader.ReadSInt32();
        string CardName = reader.ReadString8();
        string CardName_en = reader.ReadString8();
        string CardDesc = reader.ReadString8();
        int Metal = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int Coin = reader.ReadSInt32();
        int EffectFactor = reader.ReadSInt32();
        DragPurpose DragPurpose = (DragPurpose) reader.ReadSInt32();
        CardTypes CardType = (CardTypes) reader.ReadSInt32();
        string CardColor = reader.ReadString8();
        string HightLightColor = reader.ReadString8();
        return new BaseInfo(PictureID, CardName, CardName_en, CardDesc, Metal, Energy, Coin, EffectFactor, DragPurpose, CardType, CardColor, HightLightColor);
    }
}

public enum CardTypes
{
    Retinue,
    Spell,
    Equip,
}

public enum DragPurpose
{
    None = 0,
    Summon = 1,
    Equip = 2,
    Target = 3,
}
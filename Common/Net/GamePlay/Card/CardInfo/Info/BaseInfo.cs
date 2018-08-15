public struct BaseInfo
{
    public string CardName;
    public string CardDescRaw;
    public int Cost;
    public DragPurpose DragPurpose;
    public CardTypes CardType;
    public string CardColor;
    public string HightLightColor;

    public BaseInfo(string cardName, string cardDescRaw, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, string hightLightColor)
    {
        CardName = cardName;
        CardDescRaw = cardDescRaw;
        Cost = cost;
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
        writer.WriteString8(CardName);
        writer.WriteString8(CardDescRaw);
        writer.WriteSInt32(Cost);
        writer.WriteSInt32((int) DragPurpose);
        writer.WriteSInt32((int) CardType);
        writer.WriteString8(CardColor);
        writer.WriteString8(HightLightColor);
    }

    public static BaseInfo Deserialze(DataStream reader)
    {
        string CardName = reader.ReadString8();
        string CardDesc = reader.ReadString8();
        int Cost = reader.ReadSInt32();
        DragPurpose DragPurpose = (DragPurpose) reader.ReadSInt32();
        CardTypes CardType = (CardTypes) reader.ReadSInt32();
        string CardColor = reader.ReadString8();
        string HightLightColor = reader.ReadString8();
        return new BaseInfo(CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, HightLightColor);
    }
}

public enum CardTypes
{
    Retinue = 0,
    Spell = 1,
    Weapon = 2,
    Shield = 3,
    Pack = 4,
    MA = 5,
}

public enum DragPurpose
{
    None = 0,
    Summon = 1,
    Equip = 2,
    Attack = 3,
    Spell = 4,
}
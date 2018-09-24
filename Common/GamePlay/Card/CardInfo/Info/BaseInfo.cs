using System.Collections.Generic;

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
    public bool IsSoldier;
    public SlotTypes SlotType;

    public BaseInfo(int pictureID, string cardName, string cardName_en, string cardDescRaw, int metal, int energy, int coin, int effectFactor, DragPurpose dragPurpose, CardTypes cardType, bool isSoldier, SlotTypes slotType)
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
        IsSoldier = isSoldier;
        SlotType = slotType;
    }

    public string GetCardColor()
    {
        return GetCardColor(CardType, IsSoldier, SlotType);
    }

    public static string GetCardColor(CardTypes cardType, bool isSoldier, SlotTypes slotType)
    {
        if (cardType == CardTypes.Retinue)
        {
            if (isSoldier) return GamePlaySettings.SoldierCardColor;
            else return GamePlaySettings.HeroCardColor;
        }
        else if (cardType == CardTypes.Energy)
        {
            return GamePlaySettings.EnergyCardColor;
        }
        else if (cardType == CardTypes.Spell)
        {
            return GamePlaySettings.SpellCardColor;
        }
        else if (cardType == CardTypes.Equip)
        {
            switch (slotType)
            {
                case SlotTypes.Weapon:
                    return GamePlaySettings.WeaponCardColor;
                case SlotTypes.Shield:
                    return GamePlaySettings.ShieldCardColor;
                case SlotTypes.Pack:
                    return GamePlaySettings.PackCardColor;
                case SlotTypes.MA:
                    return GamePlaySettings.MACardColor;
            }
        }

        return null;
    }

    public static string GetHightLightColor()
    {
        return GamePlaySettings.CardHightLightColor;
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
        writer.WriteByte(IsSoldier ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32((int) SlotType);
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
        bool IsSoldier = reader.ReadByte() == 0x01;
        SlotTypes SlotType = (SlotTypes) reader.ReadSInt32();
        return new BaseInfo(PictureID, CardName, CardName_en, CardDesc, Metal, Energy, Coin, EffectFactor, DragPurpose, CardType, IsSoldier, SlotType);
    }
}

public enum CardTypes
{
    Retinue,
    Spell,
    Energy,
    Equip,
}

public enum DragPurpose
{
    None = 0,
    Summon = 1,
    Equip = 2,
    Target = 3,
}
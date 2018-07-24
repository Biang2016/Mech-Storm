using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public enum CardTypes
{
    Retinue = 0,
    Spell = 1,
    Weapon = 2,
    Shield = 3,
    Pack = 4,
    MA = 5,
}

public class CardInfo_Base
{
    public int CardID;
    public BaseInfo BaseInfo;
    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public SlotInfo SlotInfo;
    public WeaponInfo WeaponInfo;
    public ShieldInfo ShieldInfo;

    public List<SideEffectBase> SideEffects_OnDie = new List<SideEffectBase>();
    public List<SideEffectBase> SideEffects_OnSummoned = new List<SideEffectBase>();

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
    }

    public virtual CardInfo_Base Clone()
    {
        return new CardInfo_Base(CardID, BaseInfo);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteString8(GetType().ToString());
        writer.WriteSInt32(CardID);
        BaseInfo.Serialize(writer);
        UpgradeInfo.Serialize(writer);
        LifeInfo.Serialize(writer);
        BattleInfo.Serialize(writer);
        SlotInfo.Serialize(writer);
        WeaponInfo.Serialize(writer);
        ShieldInfo.Serialize(writer);
    }

    public static CardInfo_Base Deserialze(DataStream reader)
    {
        string myType = reader.ReadString8();
        Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
        CardInfo_Base newCardInfo_Base = (CardInfo_Base) assembly.CreateInstance(myType);

        newCardInfo_Base.CardID = reader.ReadSInt32();
        newCardInfo_Base.BaseInfo = BaseInfo.Deserialze(reader);
        newCardInfo_Base.UpgradeInfo = UpgradeInfo.Deserialze(reader);
        newCardInfo_Base.LifeInfo = LifeInfo.Deserialze(reader);
        newCardInfo_Base.BattleInfo = BattleInfo.Deserialze(reader);
        newCardInfo_Base.SlotInfo = SlotInfo.Deserialze(reader);
        newCardInfo_Base.WeaponInfo = WeaponInfo.Deserialze(reader);
        newCardInfo_Base.ShieldInfo = ShieldInfo.Deserialze(reader);

        return newCardInfo_Base;
    }

    public static string textToVertical(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char ch in text)
        {
            sb.Append(ch);
            sb.Append("\n");
        }

        return sb.ToString().Trim('\n');
    }
}

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo, List<SideEffectBase> sideEffects_OnDie, List<SideEffectBase> sideEffects_OnSummoned) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
        SideEffects_OnDie = sideEffects_OnDie;
        SideEffects_OnSummoned = sideEffects_OnSummoned;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (BattleInfo.BasicAttack != 0) CardDescShow += "攻击力 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicAttack) + "\n";
        if (BattleInfo.BasicArmor != 0) CardDescShow += "护甲 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicArmor) + "\n";
        if (BattleInfo.BasicShield != 0) CardDescShow += "护盾 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicShield) + "\n";


        if (SideEffects_OnDie.Count > 0)
        {
            CardDescShow += "亡语:";
            foreach (SideEffectBase se in SideEffects_OnDie)
            {
                CardDescShow += se.Desc + ";\n";
            }
        }


        if (SideEffects_OnSummoned.Count > 0)
        {
            CardDescShow += "战吼:";
            foreach (SideEffectBase se in SideEffects_OnSummoned)
            {
                CardDescShow += se.Desc + ";\n";
            }
        }

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add(sideEffectBase.Clone());
        }

        List<SideEffectBase> new_SideEffects_OnSummoned = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in new_SideEffects_OnSummoned)
        {
            new_SideEffects_OnSummoned.Add(sideEffectBase.Clone());
        }

        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, BaseInfo, UpgradeInfo, LifeInfo, BattleInfo, SlotInfo, new_SideEffects_OnDie, new_SideEffects_OnSummoned);
        return cb;
    }
}

public class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon()
    {
    }

    public CardInfo_Weapon(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, WeaponInfo weaponInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        WeaponInfo = weaponInfo;
        UpgradeInfo = upgradeInfo;
        SideEffects_OnDie = sideEffects_OnDie;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (WeaponInfo.WeaponType == WeaponTypes.Sword)
        {
            CardDescShow += "攻击力: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Attack.ToString()) + " 点\n";
            CardDescShow += "充能: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax) + "\n";
        }
        else if (WeaponInfo.WeaponType == WeaponTypes.Gun)
        {
            CardDescShow += "弹丸伤害: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Attack.ToString()) + " 点\n";
            CardDescShow += "弹药: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax) + "\n";
        }

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add(sideEffectBase.Clone());
        }

        CardInfo_Weapon cb = new CardInfo_Weapon(CardID, BaseInfo, UpgradeInfo, WeaponInfo, new_SideEffects_OnDie);
        return cb;
    }
}

public class CardInfo_Shield : CardInfo_Base
{
    public CardInfo_Shield()
    {
    }

    public CardInfo_Shield(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, ShieldInfo shieldInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        ShieldInfo = shieldInfo;
        SideEffects_OnDie = sideEffects_OnDie;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (ShieldInfo.ShieldType == ShieldTypes.Armor)
        {
            CardDescShow += "阻挡 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Armor.ToString()) + " 点伤害\n";
        }
        else if (ShieldInfo.ShieldType == ShieldTypes.Shield)
        {
            CardDescShow += "受到的伤害减少 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Shield.ToString()) + " 点\n";
        }

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add(sideEffectBase.Clone());
        }

        CardInfo_Shield cb = new CardInfo_Shield(CardID, BaseInfo, UpgradeInfo, ShieldInfo, new_SideEffects_OnDie);
        return cb;
    }
}

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

public struct UpgradeInfo
{
    public int UpgradeCardID;
    public int CardLevel;

    public UpgradeInfo(int upgradeCardID, int cardLevel)
    {
        UpgradeCardID = upgradeCardID;
        CardLevel = cardLevel;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(UpgradeCardID);
        writer.WriteSInt32(CardLevel);
    }

    public static UpgradeInfo Deserialze(DataStream reader)
    {
        int UpgradeCardID = reader.ReadSInt32();
        int CardLevel = reader.ReadSInt32();
        return new UpgradeInfo(UpgradeCardID, CardLevel);
    }
}

public struct LifeInfo
{
    public int Life;
    public int TotalLife;

    public LifeInfo(int life, int totalLife)
    {
        Life = life;
        TotalLife = totalLife;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Life);
        writer.WriteSInt32(TotalLife);
    }

    public static LifeInfo Deserialze(DataStream reader)
    {
        int Life = reader.ReadSInt32();
        int TotalLife = reader.ReadSInt32();
        return new LifeInfo(Life, TotalLife);
    }
}

public struct BattleInfo
{
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;

    public BattleInfo(int basicAttack, int basicShield, int basicArmor)
    {
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BasicAttack);
        writer.WriteSInt32(BasicShield);
        writer.WriteSInt32(BasicArmor);
    }

    public static BattleInfo Deserialze(DataStream reader)
    {
        int BasicAttack = reader.ReadSInt32();
        int BasicShield = reader.ReadSInt32();
        int BasicArmor = reader.ReadSInt32();
        return new BattleInfo(BasicAttack, BasicShield, BasicArmor);
    }
}

public struct SlotInfo
{
    public SlotTypes Slot1;
    public SlotTypes Slot2;
    public SlotTypes Slot3;
    public SlotTypes Slot4;

    public SlotInfo(SlotTypes slot1, SlotTypes slot2, SlotTypes slot3, SlotTypes slot4)
    {
        Slot1 = slot1;
        Slot2 = slot2;
        Slot3 = slot3;
        Slot4 = slot4;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) Slot1);
        writer.WriteSInt32((int) Slot2);
        writer.WriteSInt32((int) Slot3);
        writer.WriteSInt32((int) Slot4);
    }

    public static SlotInfo Deserialze(DataStream reader)
    {
        SlotTypes Slot1 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot2 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot3 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot4 = (SlotTypes) reader.ReadSInt32();
        return new SlotInfo(Slot1, Slot2, Slot3, Slot4);
    }
}

public struct WeaponInfo
{
    public int Energy;
    public int EnergyMax;
    public int Attack;
    public WeaponTypes WeaponType;

    public WeaponInfo(int energy, int energyMax, int attack, WeaponTypes weaponType)
    {
        Energy = energy;
        EnergyMax = energyMax;
        Attack = attack;
        WeaponType = weaponType;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(EnergyMax);
        writer.WriteSInt32(Attack);
        writer.WriteSInt32((int) WeaponType);
    }

    public static WeaponInfo Deserialze(DataStream reader)
    {
        int Energy = reader.ReadSInt32();
        int EnergyMax = reader.ReadSInt32();
        int Attack = reader.ReadSInt32();
        WeaponTypes WeaponType = (WeaponTypes) reader.ReadSInt32();
        return new WeaponInfo(Energy, EnergyMax, Attack, WeaponType);
    }
}

public struct ShieldInfo
{
    public int Armor;
    public int Shield;
    public ShieldTypes ShieldType;

    public ShieldInfo(int armor, int shield, ShieldTypes shieldType)
    {
        Armor = armor;
        Shield = shield;
        ShieldType = shieldType;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Armor);
        writer.WriteSInt32(Shield);
        writer.WriteSInt32((int) ShieldType);
    }

    public static ShieldInfo Deserialze(DataStream reader)
    {
        int Armor = reader.ReadSInt32();
        int Shield = reader.ReadSInt32();
        ShieldTypes ShieldType = (ShieldTypes) reader.ReadSInt32();
        return new ShieldInfo(Armor, Shield, ShieldType);
    }
}
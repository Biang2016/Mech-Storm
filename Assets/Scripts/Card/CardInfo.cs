using System.Text;
using UnityEngine;

internal enum CardTypes
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
    internal CardInfo_Base(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, Color cardColor, int upgradeCardID,int cardLevel)
    {
        CardID = cardID;
        CardName = cardName;
        CardDesc = cardDesc;
        Cost = cost;
        DragPurpose = dragPurpose;
        CardType = cardType;
        CardColor = cardColor;
        UpgradeID = upgradeCardID;
        CardLevel = cardLevel;
    }

    internal int CardID;
    internal string CardName;
    internal string CardDesc;
    internal int Cost;
    internal DragPurpose DragPurpose;
    internal CardTypes CardType;
    internal Color CardColor;
    internal int UpgradeID;
    internal int CardLevel;

    public virtual CardInfo_Base Clone()
    {
        CardInfo_Base cb = new CardInfo_Base(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID,CardLevel);
        return cb;
    }
}

public class CardInfo_Retinue : CardInfo_Base
{
    internal CardInfo_Retinue(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, Color cardColor, int upgradeCardID,int cardLevel, int life, int totalLife, int basicAttack, int basicShield, int basicArmor,SlotType slot1,SlotType slot2,SlotType slot3,SlotType slot4) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
    {
        Life = life;
        TotalLife = totalLife;
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
        Slot1 = slot1;
        Slot2 = slot2;
        Slot3 = slot3;
        Slot4 = slot4;
    }

    internal int Life;
    internal int TotalLife;
    internal int BasicAttack;
    internal int BasicShield;
    internal int BasicArmor;
    internal SlotType Slot1;
    internal SlotType Slot2;
    internal SlotType Slot3;
    internal SlotType Slot4;

    public override CardInfo_Base Clone()
    {
        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID,CardLevel, Life, TotalLife, BasicAttack, BasicShield, BasicArmor,Slot1,Slot2,Slot3,Slot4);
        return cb;
    }
}

public class CardInfo_Weapon : CardInfo_Base
{
    internal CardInfo_Weapon(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, Color cardColor, int upgradeCardID,int cardLevel, int energy, int energyMax, int attack, WeaponType weaponType) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
    {
        Energy = energy;
        EnergyMax = energyMax;
        Attack = attack;
        M_WeaponType = weaponType;
    }

    internal int Energy;
    internal int EnergyMax;
    internal int Attack;
    internal WeaponType M_WeaponType;

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

    public override CardInfo_Base Clone()
    {
        CardInfo_Weapon cb = new CardInfo_Weapon(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID, CardLevel, Energy, EnergyMax, Attack, M_WeaponType);
        return cb;
    }
}

public class CardInfo_Shield : CardInfo_Base
{
    internal CardInfo_Shield(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, Color cardColor, int upgradeCardID,int cardLevel, ShieldType shielType, int armor, int armorMax, int shield, int shieldMax) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
    {
        M_ShieldType = shielType;
        Armor = armor;
        ArmorMax = armorMax;
        Shield = shield;
        ShieldMax = shieldMax;
    }

    internal ShieldType M_ShieldType;
    internal int Armor;
    internal int ArmorMax;
    internal int Shield;
    internal int ShieldMax;

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

    public override CardInfo_Base Clone()
    {
        CardInfo_Shield cb = new CardInfo_Shield(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID,CardLevel, M_ShieldType, Armor, ArmorMax, Shield, ShieldMax);
        return cb;
    }
}
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
    internal CardInfo_Base(int cardID, string cardName, string cardDesc, int cost, bool hasTarget, CardTypes cardType, Color cardColor, int upgreaCardID)
    {
        CardID = cardID;
        CardName = cardName;
        CardDesc = cardDesc;
        Cost = cost;
        HasTarget = hasTarget;
        CardType = cardType;
        CardColor = cardColor;
        UpgradeID = upgreaCardID;
    }

    internal int CardID;
    internal string CardName;
    internal string CardDesc;
    internal int Cost;
    internal bool HasTarget;
    internal CardTypes CardType;
    internal Color CardColor;
    internal int UpgradeID;

    public virtual CardInfo_Base Clone()
    {
        CardInfo_Base cb = new CardInfo_Base(CardID, CardName, CardDesc, Cost, HasTarget, CardType, CardColor, UpgradeID);
        return cb;
    }
}

public class CardInfo_Retinue : CardInfo_Base
{
    internal CardInfo_Retinue(int cardID, string cardName, string cardDesc, int cost, bool hasTarget, CardTypes cardType, Color cardColor, int upgreaCardID, int life, int totalLife, int basicAttack, int basicShield, int basicArmor) : base(cardID, cardName, cardDesc, cost, hasTarget, cardType, cardColor, upgreaCardID)
    {
        Life = life;
        TotalLife = totalLife;
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
    }

    internal int Life;
    internal int TotalLife;
    internal int BasicAttack;
    internal int BasicShield;
    internal int BasicArmor;

    public override CardInfo_Base Clone()
    {
        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, CardName, CardDesc, Cost, HasTarget, CardType, CardColor, UpgradeID, Life, TotalLife, BasicAttack, BasicShield, BasicArmor);
        return cb;
    }
}

public class CardInfo_Weapon : CardInfo_Base
{
    internal CardInfo_Weapon(int cardID, string cardName, string cardDesc, int cost, bool hasTarget, CardTypes cardType, Color cardColor, int upgreaCardID, int energy, int energyMax, int attack, WeaponType weaponType) : base(cardID, cardName, cardDesc, cost, hasTarget, cardType, cardColor, upgreaCardID)
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
        CardInfo_Weapon cb = new CardInfo_Weapon(CardID, CardName, CardDesc, Cost, HasTarget, CardType, CardColor, UpgradeID, Energy, EnergyMax, Attack, M_WeaponType);
        return cb;
    }
}

public class CardInfo_Shield : CardInfo_Base
{
    internal CardInfo_Shield(int cardID, string cardName, string cardDesc, int cost, bool hasTarget, CardTypes cardType, Color cardColor, int upgreaCardID, ShieldType shielType, int armor, int armorMax, int shield, int shieldMax) : base(cardID, cardName, cardDesc, cost, hasTarget, cardType, cardColor, upgreaCardID)
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
        CardInfo_Shield cb = new CardInfo_Shield(CardID, CardName, CardDesc, Cost, HasTarget, CardType, CardColor, UpgradeID, M_ShieldType, Armor, ArmorMax, Shield, ShieldMax);
        return cb;
    }
}
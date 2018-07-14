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
    public CardInfo_Base(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID,int cardLevel)
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

    public int CardID;
    public string CardName;
    public string CardDesc;
    public int Cost;
    public DragPurpose DragPurpose;
    public CardTypes CardType;
    public string CardColor;
    public int UpgradeID;
    public int CardLevel;

    public virtual CardInfo_Base Clone()
    {
        CardInfo_Base cb = new CardInfo_Base(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID,CardLevel);
        return cb;
    }
}

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID,int cardLevel, int life, int totalLife, int basicAttack, int basicShield, int basicArmor,SlotTypes slot1,SlotTypes slot2,SlotTypes slot3,SlotTypes slot4) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
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

    public int Life;
    public int TotalLife;
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;
    public SlotTypes Slot1;
    public SlotTypes Slot2;
    public SlotTypes Slot3;
    public SlotTypes Slot4;

    public override CardInfo_Base Clone()
    {
        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID,CardLevel, Life, TotalLife, BasicAttack, BasicShield, BasicArmor,Slot1,Slot2,Slot3,Slot4);
        return cb;
    }
}

public class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID,int cardLevel, int energy, int energyMax, int attack, WeaponType weaponType) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
    {
        Energy = energy;
        EnergyMax = energyMax;
        Attack = attack;
        M_WeaponType = weaponType;
    }

    public int Energy;
    public int EnergyMax;
    public int Attack;
    public WeaponType M_WeaponType;

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
    public CardInfo_Shield(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID,int cardLevel, ShieldType shielType, int armor, int armorMax, int shield, int shieldMax) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
    {
        M_ShieldType = shielType;
        Armor = armor;
        ArmorMax = armorMax;
        Shield = shield;
        ShieldMax = shieldMax;
    }

    public ShieldType M_ShieldType;
    public int Armor;
    public int ArmorMax;
    public int Shield;
    public int ShieldMax;

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
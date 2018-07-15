using System.Text;

internal enum CardTypes
{
    Retinue = 0,
    Spell = 1,
    Weapon = 2,
    Shield = 3,
    Pack = 4,
    MA = 5,
}

internal class CardInfo_Base
{
    public CardInfo_Base(int cardID, BaseInfo baseInfo)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
    }

    public int CardID;
    public BaseInfo BaseInfo;


    public virtual CardInfo_Base Clone()
    {
        CardInfo_Base cb = new CardInfo_Base(CardID, BaseInfo);
        return cb;
    }
}

internal class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
    }

    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public SlotInfo SlotInfo;


    public override CardInfo_Base Clone()
    {
        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, BaseInfo, UpgradeInfo, LifeInfo, BattleInfo, SlotInfo);
        return cb;
    }
}

internal class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID, int cardLevel, int energy, int energyMax, int attack, WeaponType weaponType) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
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

internal class CardInfo_Shield : CardInfo_Base
{
    public CardInfo_Shield(int cardID, string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor, int upgradeCardID, int cardLevel, ShieldType shielType, int armor, int armorMax, int shield, int shieldMax) : base(cardID, cardName, cardDesc, cost, dragPurpose, cardType, cardColor, upgradeCardID, cardLevel)
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
        CardInfo_Shield cb = new CardInfo_Shield(CardID, CardName, CardDesc, Cost, DragPurpose, CardType, CardColor, UpgradeID, CardLevel, M_ShieldType, Armor, ArmorMax, Shield, ShieldMax);
        return cb;
    }
}

internal struct BaseInfo
{
    public string CardName;
    public string CardDesc;
    public int Cost;
    public DragPurpose DragPurpose;
    public CardTypes CardType;
    public string CardColor;
}

internal struct UpgradeInfo
{
    public int UpgradeCardID;
    public int CardLevel;
}

internal struct LifeInfo
{
    public int Life;
    public int TotalLife;
}

internal struct BattleInfo
{
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;
}

internal struct SlotInfo
{
    public SlotTypes Slot1;
    public SlotTypes Slot2;
    public SlotTypes Slot3;
    public SlotTypes Slot4;
}
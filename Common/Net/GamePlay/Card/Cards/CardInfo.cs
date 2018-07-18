using System.Collections.Generic;
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

public abstract class CardInfo_Base
{
    public CardInfo_Base(int cardID, BaseInfo baseInfo)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
    }

    public int CardID;
    public BaseInfo BaseInfo;
    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public SlotInfo SlotInfo;
    public WeaponInfo WeaponInfo;
    public ShieldInfo ShieldInfo;

    public List<SideEffectBase> SideEffects_OnDie;
    public List<SideEffectBase> SideEffects_OnSummoned;

    public abstract CardInfo_Base Clone();

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
    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo,List<SideEffectBase> sideEffects_OnDie,List<SideEffectBase> sideEffects_OnSummoned) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
        SideEffects_OnDie = sideEffects_OnDie;
        SideEffects_OnSummoned = sideEffects_OnSummoned;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie=new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add(sideEffectBase.Clone());
        }
        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, BaseInfo, UpgradeInfo, LifeInfo, BattleInfo, SlotInfo, new_SideEffects_OnDie, SideEffects_OnSummoned);
        return cb;
    }
}

public class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, WeaponInfo weaponInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        WeaponInfo = weaponInfo;
        UpgradeInfo = upgradeInfo;
        SideEffects_OnDie = sideEffects_OnDie;
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
    public CardInfo_Shield(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, ShieldInfo shieldInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        ShieldInfo = shieldInfo;
        SideEffects_OnDie = sideEffects_OnDie;
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
    public string CardDesc;
    public int Cost;
    public DragPurpose DragPurpose;
    public CardTypes CardType;
    public string CardColor;

    public BaseInfo(string cardName, string cardDesc, int cost, DragPurpose dragPurpose, CardTypes cardType, string cardColor)
    {
        CardName = cardName;
        CardDesc = cardDesc;
        Cost = cost;
        DragPurpose = dragPurpose;
        CardType = cardType;
        CardColor = cardColor;
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
}
public struct ShieldInfo
{
    public int Armor;
    public int ArmorMax;
    public int Shield;
    public int ShieldMax;
    public ShieldTypes ShieldType;

    public ShieldInfo(int armor, int armorMax, int shield, int shieldMax, ShieldTypes shieldType)
    {
        Armor = armor;
        ArmorMax = armorMax;
        Shield = shield;
        ShieldMax = shieldMax;
        ShieldType = shieldType;
    }
}
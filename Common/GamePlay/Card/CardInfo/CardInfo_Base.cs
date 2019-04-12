using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

public class CardInfo_Base
{
    public int CardID;
    public BaseInfo BaseInfo;
    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public RetinueInfo RetinueInfo;
    public EquipInfo EquipInfo;
    public TargetInfo TargetInfo;
    public WeaponInfo WeaponInfo;
    public ShieldInfo ShieldInfo;
    public PackInfo PackInfo;
    public MAInfo MAInfo;

    public SideEffectBundle SideEffectBundle = new SideEffectBundle();
    public SideEffectBundle SideEffectBundle_OnBattleGround = new SideEffectBundle(); //只有在战场上才会生效的特效（如装备牌和随从牌）

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        UpgradeInfo = upgradeInfo;
        SideEffectBundle = sideEffectBundle;
        SideEffectBundle_OnBattleGround = sideEffectBundle_OnBattleGround;
        TargetInfo.Initialize(this);
        Pro_Initialize();
    }

    protected void Pro_Initialize()
    {
        if (EquipInfo.SlotType != SlotTypes.None)
        {
            BaseInfo.DragPurpose = DragPurpose.Equip;
        }
        else if (TargetInfo.HasNoTarget || BaseInfo.CardType == CardTypes.Retinue)
        {
            BaseInfo.DragPurpose = DragPurpose.Summon;
        }
        else
        {
            BaseInfo.DragPurpose = DragPurpose.Target;
        }
    }

    public virtual string GetCardDescShow()
    {
        string CardDescShow = "";
        CardDescShow += SideEffectBundle.GetSideEffectsDesc();
        CardDescShow += SideEffectBundle_OnBattleGround.GetSideEffectsDesc();
        return CardDescShow;
    }

    public virtual string GetCardColor()
    {
        return null;
    }

    public virtual float GetCardColorIntensity()
    {
        return 0f;
    }

    public string GetCardDecsTextColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardDescTextColor];
    }

    public virtual CardInfo_Base Clone()
    {
        return new CardInfo_Base(CardID, BaseInfo, UpgradeInfo, SideEffectBundle.Clone(), SideEffectBundle_OnBattleGround.Clone());
    }

    public void Serialize(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(CardID);
        BaseInfo.Serialize(writer);
        UpgradeInfo.Serialize(writer);
        LifeInfo.Serialize(writer);
        BattleInfo.Serialize(writer);
        RetinueInfo.Serialize(writer);
        EquipInfo.Serialize(writer);
        WeaponInfo.Serialize(writer);
        ShieldInfo.Serialize(writer);
        PackInfo.Serialize(writer);
        MAInfo.Serialize(writer);

        SideEffectBundle.Serialize(writer);
        SideEffectBundle_OnBattleGround.Serialize(writer);
    }

    public static CardInfo_Base Deserialze(DataStream reader)
    {
        string myType = reader.ReadString8();
        Assembly assembly = Assembly.GetAssembly(typeof(CardInfo_Base)); // 获取当前程序集 
        CardInfo_Base newCardInfo_Base = (CardInfo_Base) assembly.CreateInstance(myType);

        newCardInfo_Base.CardID = reader.ReadSInt32();
        newCardInfo_Base.BaseInfo = BaseInfo.Deserialze(reader);
        newCardInfo_Base.UpgradeInfo = UpgradeInfo.Deserialze(reader);
        newCardInfo_Base.LifeInfo = LifeInfo.Deserialze(reader);
        newCardInfo_Base.BattleInfo = BattleInfo.Deserialze(reader);
        newCardInfo_Base.RetinueInfo = RetinueInfo.Deserialze(reader);
        newCardInfo_Base.EquipInfo = EquipInfo.Deserialze(reader);
        newCardInfo_Base.WeaponInfo = WeaponInfo.Deserialze(reader);
        newCardInfo_Base.ShieldInfo = ShieldInfo.Deserialze(reader);
        newCardInfo_Base.PackInfo = PackInfo.Deserialze(reader);
        newCardInfo_Base.MAInfo = MAInfo.Deserialze(reader);

        newCardInfo_Base.SideEffectBundle = SideEffectBundle.Deserialze(reader);
        newCardInfo_Base.SideEffectBundle_OnBattleGround = SideEffectBundle.Deserialze(reader);

        return newCardInfo_Base;
    }

    public virtual string GetCardTypeDesc()
    {
        return null;
    }

    public static T ConvertCardInfo<T>(CardInfo_Base src) where T : CardInfo_Base
    {
        switch (src)
        {
            case CardInfo_Retinue ci:
            {
                switch (typeof(T).Name)
                {
                    case "CardInfo_Retinue":
                    {
                        return (T) ci.Clone();
                    }
                    case "CardInfo_Equip":
                    {
                        CardInfo_Base res = new CardInfo_Equip(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new EquipInfo(SlotTypes.Weapon),
                            new WeaponInfo(1, 1, 1, WeaponTypes.Sword, false, false),
                            new ShieldInfo(),
                            new PackInfo(),
                            new MAInfo(),
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Equip;
                        return (T) res;
                    }
                    case "CardInfo_Spell":
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Spell;
                        return (T) res;
                    }
                }

                break;
            }
            case CardInfo_Equip ci:
            {
                switch (typeof(T).Name)
                {
                    case "CardInfo_Retinue":
                    {
                        CardInfo_Base res = new CardInfo_Retinue(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new LifeInfo(1, 1),
                            new BattleInfo(0, 0, 0),
                            new RetinueInfo(false, false, false, false, false, SlotTypes.None, SlotTypes.None, SlotTypes.None, SlotTypes.None),
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Retinue;
                        return (T) res;
                    }
                    case "CardInfo_Equip":
                    {
                        return (T) ci.Clone();
                    }
                    case "CardInfo_Spell":
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Spell;
                        return (T) res;
                    }
                }

                break;
            }
            case CardInfo_Spell ci:
            {
                switch (typeof(T).Name)
                {
                    case "CardInfo_Retinue":
                    {
                        CardInfo_Base res = new CardInfo_Retinue(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new LifeInfo(1, 1),
                            new BattleInfo(0, 0, 0),
                            new RetinueInfo(false, false, false, false, false, SlotTypes.None, SlotTypes.None, SlotTypes.None, SlotTypes.None),
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Retinue;
                        return (T) res;
                    }
                    case "CardInfo_Equip":
                    {
                        CardInfo_Base res = new CardInfo_Equip(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new EquipInfo(SlotTypes.Weapon),
                            new WeaponInfo(1, 1, 1, WeaponTypes.Sword, false, false),
                            new ShieldInfo(),
                            new PackInfo(),
                            new MAInfo(),
                            ci.SideEffectBundle.Clone(),
                            ci.SideEffectBundle_OnBattleGround.Clone());
                        res.BaseInfo.CardType = CardTypes.Equip;
                        return (T) res;
                    }
                    case "CardInfo_Spell":
                    {
                        return (T) ci.Clone();
                    }
                }

                break;
            }
        }

        return null;
    }
}
using System.Reflection;

public class CardInfo_Base
{
    public int CardID;
    public BaseInfo BaseInfo;
    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public RetinueInfo RetinueInfo;
    public EquipInfo EquipInfo;
    public WeaponInfo WeaponInfo;
    public ShieldInfo ShieldInfo;
    public PackInfo PackInfo;
    public MAInfo MAInfo;

    public SideEffectBundle SideEffectBundle = new SideEffectBundle();
    public SideEffectBundle SideEffectBundle_OnBattleGround = new SideEffectBundle(); //只有在战场上才会生效的特效（如装备牌和随从牌）

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        SideEffectBundle = sideEffectBundle;
        SideEffectBundle_OnBattleGround = sideEffectBundle_OnBattleGround;
    }

    public virtual string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;
        CardDescShow += SideEffectBundle.GetSideEffectsDesc(isEnglish);
        CardDescShow += SideEffectBundle_OnBattleGround.GetSideEffectsDesc(isEnglish);
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
        return AllColors.ColorDict[AllColors.ColorType.CardDecsTextColor];
    }

    public virtual CardInfo_Base Clone()
    {
        return new CardInfo_Base(CardID, BaseInfo, SideEffectBundle.Clone(), SideEffectBundle_OnBattleGround.Clone());
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


    public virtual string GetCardTypeDesc(bool isEnglish)
    {
        return null;
    }
}
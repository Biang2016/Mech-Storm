using System;
using System.Collections.Generic;
using System.Reflection;

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
    public SlotTypes M_SlotType;

    public SideEffectBundle SideEffects = new SideEffectBundle();
    public SideEffectBundle SideEffects_OnBattleGround = new SideEffectBundle();//只有在战场上才会生效的特效（如装备牌和随从牌）

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, SlotTypes slotType, SideEffectBundle sideEffects, SideEffectBundle sideEffects_OnBattleGround)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        M_SlotType = slotType;
        SideEffects = sideEffects;
        SideEffects_OnBattleGround = sideEffects_OnBattleGround;
    }

    public virtual string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;
        CardDescShow += SideEffects.GetSideEffectsDesc(isEnglish);
        CardDescShow += SideEffects_OnBattleGround.GetSideEffectsDesc(isEnglish);
        return CardDescShow;
    }

    public virtual CardInfo_Base Clone()
    {
        return new CardInfo_Base(CardID, BaseInfo, M_SlotType, SideEffects.Clone(), SideEffects_OnBattleGround.Clone());
    }

    public void Serialize(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(CardID);
        BaseInfo.Serialize(writer);
        writer.WriteSInt32((int) M_SlotType);
        UpgradeInfo.Serialize(writer);
        LifeInfo.Serialize(writer);
        BattleInfo.Serialize(writer);
        SlotInfo.Serialize(writer);
        WeaponInfo.Serialize(writer);
        ShieldInfo.Serialize(writer);

        SideEffects.Serialize(writer);
        SideEffects_OnBattleGround.Serialize(writer);
    }

    public static CardInfo_Base Deserialze(DataStream reader)
    {
        string myType = reader.ReadString8();
        Assembly assembly = Assembly.GetAssembly(typeof(CardInfo_Base)); // 获取当前程序集 
        CardInfo_Base newCardInfo_Base = (CardInfo_Base) assembly.CreateInstance(myType);

        newCardInfo_Base.CardID = reader.ReadSInt32();
        newCardInfo_Base.BaseInfo = BaseInfo.Deserialze(reader);
        newCardInfo_Base.M_SlotType = (SlotTypes) reader.ReadSInt32();
        newCardInfo_Base.UpgradeInfo = UpgradeInfo.Deserialze(reader);
        newCardInfo_Base.LifeInfo = LifeInfo.Deserialze(reader);
        newCardInfo_Base.BattleInfo = BattleInfo.Deserialze(reader);
        newCardInfo_Base.SlotInfo = SlotInfo.Deserialze(reader);
        newCardInfo_Base.WeaponInfo = WeaponInfo.Deserialze(reader);
        newCardInfo_Base.ShieldInfo = ShieldInfo.Deserialze(reader);

        newCardInfo_Base.SideEffects = SideEffectBundle.Deserialze(reader);
        newCardInfo_Base.SideEffects_OnBattleGround = SideEffectBundle.Deserialze(reader);

        return newCardInfo_Base;
    }
}
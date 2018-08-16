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
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(CardID);
        BaseInfo.Serialize(writer);
        UpgradeInfo.Serialize(writer);
        LifeInfo.Serialize(writer);
        BattleInfo.Serialize(writer);
        SlotInfo.Serialize(writer);
        WeaponInfo.Serialize(writer);
        ShieldInfo.Serialize(writer);

        writer.WriteSInt32(SideEffects_OnDie.Count);
        foreach (SideEffectBase se in SideEffects_OnDie)
        {
            se.Serialze(writer);
        }

        writer.WriteSInt32(SideEffects_OnSummoned.Count);
        foreach (SideEffectBase se in SideEffects_OnSummoned)
        {
            se.Serialze(writer);
        }
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
        newCardInfo_Base.SlotInfo = SlotInfo.Deserialze(reader);
        newCardInfo_Base.WeaponInfo = WeaponInfo.Deserialze(reader);
        newCardInfo_Base.ShieldInfo = ShieldInfo.Deserialze(reader);

        List<SideEffectBase> SideEffects_OnDie = new List<SideEffectBase>();
        int SE_OnDie_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnDie_count; i++)
        {
            SideEffects_OnDie.Add(SideEffectBase.BaseDeserialze(reader));
        }

        List<SideEffectBase> SideEffects_OnSummoned = new List<SideEffectBase>();
        int SE_OnSummon_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnSummon_count; i++)
        {
            SideEffects_OnSummoned.Add(SideEffectBase.BaseDeserialze(reader));
        }

        newCardInfo_Base.SideEffects_OnDie = SideEffects_OnDie;
        newCardInfo_Base.SideEffects_OnSummoned = SideEffects_OnSummoned;

        return newCardInfo_Base;
    }
}
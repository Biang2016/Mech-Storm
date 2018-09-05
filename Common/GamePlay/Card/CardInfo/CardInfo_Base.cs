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

    public SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>> SideEffects = new SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>>();

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, SlotTypes slotType, SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>> sideEffects)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        M_SlotType = slotType;
        SideEffects = sideEffects;
    }

    public virtual string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        foreach (KeyValuePair<SideEffectBase.TriggerTime, List<SideEffectBase>> kv in SideEffects)
        {
            if (kv.Value.Count > 0) CardDescShow += SideEffectBase.TriggerTimeDesc[kv.Key];
            foreach (SideEffectBase se in kv.Value)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }

        return CardDescShow;
    }

    public virtual CardInfo_Base Clone()
    {
        SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>> newSideEffects = new SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>>();

        foreach (KeyValuePair<SideEffectBase.TriggerTime, List<SideEffectBase>> kv in SideEffects)
        {
            List<SideEffectBase> SEs = new List<SideEffectBase>();
            foreach (SideEffectBase se in kv.Value)
            {
                SEs.Add((SideEffectBase) ((ICloneable) se).Clone());
            }

            newSideEffects.Add(kv.Key, SEs);
        }

        return new CardInfo_Base(CardID, BaseInfo, M_SlotType, newSideEffects);
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

        writer.WriteSInt32(SideEffects.Count);
        foreach (KeyValuePair<SideEffectBase.TriggerTime, List<SideEffectBase>> kv in SideEffects)
        {
            writer.WriteSInt32((int) kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (SideEffectBase se in kv.Value)
            {
                se.Serialze(writer);
            }
        }
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

        int SideEffectCount = reader.ReadSInt32();
        newCardInfo_Base.SideEffects = new SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>>();
        for (int i = 0; i < SideEffectCount; i++)
        {
            SideEffectBase.TriggerTime tt = (SideEffectBase.TriggerTime) reader.ReadSInt32();
            List<SideEffectBase> SEs = new List<SideEffectBase>();
            int SECount = reader.ReadSInt32();
            for (int j = 0; j < SECount; j++)
            {
                SEs.Add(SideEffectBase.BaseDeserialze(reader));
            }

            newCardInfo_Base.SideEffects.Add(tt, SEs);
        }

        return newCardInfo_Base;
    }
}
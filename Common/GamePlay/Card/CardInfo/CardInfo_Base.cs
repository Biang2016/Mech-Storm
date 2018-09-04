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

    public List<SideEffectBase> SideEffects_OnEndRound = new List<SideEffectBase>();
    public List<SideEffectBase> SideEffects_OnPlayOut = new List<SideEffectBase>();
    public List<SideEffectBase> SideEffects_OnSummoned = new List<SideEffectBase>();
    public List<SideEffectBase> SideEffects_OnDie = new List<SideEffectBase>();

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, List<SideEffectBase> sideEffects_OnEndRound, List<SideEffectBase> sideEffects_OnPlayOut, List<SideEffectBase> sideEffects_OnSummoned, List<SideEffectBase> sideEffects_OnDie)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        SideEffects_OnEndRound = sideEffects_OnEndRound;
        SideEffects_OnPlayOut = sideEffects_OnPlayOut;
        SideEffects_OnSummoned = sideEffects_OnSummoned;
        SideEffects_OnDie = sideEffects_OnDie;
    }

    public virtual string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (SideEffects_OnPlayOut.Count > 0)
        {
            foreach (SideEffectBase se in SideEffects_OnPlayOut)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }

        if (SideEffects_OnEndRound.Count > 0)
        {
            CardDescShow += "我方回合结束时,";
            foreach (SideEffectBase se in SideEffects_OnEndRound)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }

        if (SideEffects_OnSummoned.Count > 0)
        {
            CardDescShow += "战吼:";
            foreach (SideEffectBase se in SideEffects_OnSummoned)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }

        if (SideEffects_OnDie.Count > 0)
        {
            CardDescShow += "亡语:";
            foreach (SideEffectBase se in SideEffects_OnDie)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }

        return CardDescShow;
    }

    public virtual CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnEndRound = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnEndRound)
        {
            new_SideEffects_OnEndRound.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        List<SideEffectBase> new_SideEffects_OnPlayOut = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnPlayOut)
        {
            new_SideEffects_OnPlayOut.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        List<SideEffectBase> new_SideEffects_OnSummoned = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnSummoned)
        {
            new_SideEffects_OnSummoned.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        return new CardInfo_Base(CardID, BaseInfo, new_SideEffects_OnEndRound, new_SideEffects_OnPlayOut, new_SideEffects_OnSummoned, SideEffects_OnDie);
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

        writer.WriteSInt32(SideEffects_OnEndRound.Count);
        foreach (SideEffectBase se in SideEffects_OnEndRound)
        {
            se.Serialze(writer);
        }

        writer.WriteSInt32(SideEffects_OnPlayOut.Count);
        foreach (SideEffectBase se in SideEffects_OnPlayOut)
        {
            se.Serialze(writer);
        }

        writer.WriteSInt32(SideEffects_OnSummoned.Count);
        foreach (SideEffectBase se in SideEffects_OnSummoned)
        {
            se.Serialze(writer);
        }

        writer.WriteSInt32(SideEffects_OnDie.Count);
        foreach (SideEffectBase se in SideEffects_OnDie)
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

        List<SideEffectBase> SideEffects_OnEndRound = new List<SideEffectBase>();
        int SE_OnEndRound_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnEndRound_count; i++)
        {
            SideEffects_OnEndRound.Add(SideEffectBase.BaseDeserialze(reader));
        }

        List<SideEffectBase> SideEffects_OnPlayOut = new List<SideEffectBase>();
        int SE_OnPlayOut_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnPlayOut_count; i++)
        {
            SideEffects_OnPlayOut.Add(SideEffectBase.BaseDeserialze(reader));
        }

        List<SideEffectBase> SideEffects_OnSummoned = new List<SideEffectBase>();
        int SE_OnSummon_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnSummon_count; i++)
        {
            SideEffects_OnSummoned.Add(SideEffectBase.BaseDeserialze(reader));
        }

        List<SideEffectBase> SideEffects_OnDie = new List<SideEffectBase>();
        int SE_OnDie_count = reader.ReadSInt32();
        for (int i = 0; i < SE_OnDie_count; i++)
        {
            SideEffects_OnDie.Add(SideEffectBase.BaseDeserialze(reader));
        }

        newCardInfo_Base.SideEffects_OnEndRound = SideEffects_OnEndRound;
        newCardInfo_Base.SideEffects_OnPlayOut = SideEffects_OnPlayOut;
        newCardInfo_Base.SideEffects_OnSummoned = SideEffects_OnSummoned;
        newCardInfo_Base.SideEffects_OnDie = SideEffects_OnDie;

        return newCardInfo_Base;
    }
}
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Encapsulate sideeffect and its TriggerTime, TriggerRange, number of trigger times, and other attributes. 
/// </summary>
public class SideEffectExecute
{
    private static int idGenerator = 5000;

    public static int GenerateID()
    {
        return idGenerator++;
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SideEffectFrom
    {
        Unknown = 0,
        Buff = 1,
        SpellCard = 2,
        EnergyCard = 4,
        RetinueSideEffect = 8,
        EquipSideEffect = 16,
    }

    public SideEffectFrom M_SideEffectFrom;

    public int ID;
    public SideEffectBase SideEffectBase;

    public SideEffectBundle.TriggerTime TriggerTime; //when to trigger
    public SideEffectBundle.TriggerRange TriggerRange; //which range of events can trigger this effect
    public int TriggerDelayTimes;//how many times we need to trigger it before it can realy trigger
    public int TriggerTimes;//the max times we can trigger it.

    public SideEffectBundle.TriggerTime RemoveTriggerTime; //when to remove this effect/decrease the remove time of this effect
    public SideEffectBundle.TriggerRange RemoveTriggerRange; //which range of events can remove this effect
    public int RemoveTriggerTimes; //how many times of remove before we can remove the effect permenantly. (usually used in buffs)

    private SideEffectExecute()
    {
    }

    public SideEffectExecute(SideEffectFrom sideEffectFrom, SideEffectBase sideEffectBase, SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange, int triggerDelayTimes, int triggerTimes, SideEffectBundle.TriggerTime removeTriggerTime, SideEffectBundle.TriggerRange removeTriggerRange, int removeTriggerTimes)
    {
        M_SideEffectFrom = sideEffectFrom;
        ID = GenerateID();
        SideEffectBase = sideEffectBase;
        TriggerTime = triggerTime;
        TriggerRange = triggerRange;
        TriggerDelayTimes = triggerDelayTimes;
        TriggerTimes = triggerTimes;
        RemoveTriggerTime = removeTriggerTime;
        RemoveTriggerRange = removeTriggerRange;
        RemoveTriggerTimes = removeTriggerTimes;
    }

    public SideEffectExecute Clone()
    {
        return new SideEffectExecute(M_SideEffectFrom, SideEffectBase.Clone(), TriggerTime, TriggerRange, TriggerDelayTimes, TriggerTimes, RemoveTriggerTime, RemoveTriggerRange, RemoveTriggerTimes);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) M_SideEffectFrom);
        writer.WriteSInt32(ID);
        SideEffectBase.Serialize(writer);
        writer.WriteSInt32((int) TriggerTime);
        writer.WriteSInt32((int) TriggerRange);
        writer.WriteSInt32(TriggerDelayTimes);
        writer.WriteSInt32(TriggerTimes);
        writer.WriteSInt32((int) RemoveTriggerTime);
        writer.WriteSInt32((int) RemoveTriggerRange);
        writer.WriteSInt32(RemoveTriggerTimes);
    }

    public static SideEffectExecute Deserialize(DataStream reader)
    {
        SideEffectExecute see = new SideEffectExecute();
        see.M_SideEffectFrom = (SideEffectFrom) reader.ReadSInt32();
        see.ID = reader.ReadSInt32();
        see.SideEffectBase = SideEffectBase.BaseDeserialize(reader);
        see.TriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        see.TriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
        see.TriggerDelayTimes = reader.ReadSInt32();
        see.TriggerTimes = reader.ReadSInt32();
        see.RemoveTriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        see.RemoveTriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
        see.RemoveTriggerTimes = reader.ReadSInt32();
        return see;
    }
}
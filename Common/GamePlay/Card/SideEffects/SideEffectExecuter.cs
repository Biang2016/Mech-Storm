using System;

/// <summary>
/// 将SideEffect和其触发时机、触发次数等参数封装起来
/// </summary>
public class SideEffectExecute
{
    private static int idGenerator = 5000;

    public static int GenerateID()
    {
        return idGenerator++;
    }

    [Flags]
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

    public SideEffectBundle.TriggerTime TriggerTime; //触发SE时机
    public SideEffectBundle.TriggerRange TriggerRange; //触发SE条件
    public int TriggerDelayTimes;
    public int TriggerTimes;

    public SideEffectBundle.TriggerTime RemoveTriggerTime; //移除SE时机
    public SideEffectBundle.TriggerRange RemoveTriggerRange; //移除SE条件
    public int RemoveTriggerTimes; //Remove触发多少次后，移除此SE（如：3回合内全体攻击力+1）

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

    public static SideEffectExecute Deserialze(DataStream reader)
    {
        SideEffectExecute see = new SideEffectExecute();
        see.M_SideEffectFrom = (SideEffectFrom) reader.ReadSInt32();
        see.ID = reader.ReadSInt32();
        see.SideEffectBase = SideEffectBase.BaseDeserialze(reader);
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
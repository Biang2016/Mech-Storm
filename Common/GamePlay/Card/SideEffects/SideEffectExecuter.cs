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

    public SideEffectExecute(SideEffectBase sideEffectBase, SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange, int triggerDelayTimes, int triggerTimes, SideEffectBundle.TriggerTime removeTriggerTime, SideEffectBundle.TriggerRange removeTriggerRange, int removeTriggerTimes)
    {
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
        return new SideEffectExecute(SideEffectBase.Clone(), TriggerTime, TriggerRange, TriggerDelayTimes, TriggerTimes, RemoveTriggerTime, RemoveTriggerRange, RemoveTriggerTimes);
    }

    public void Serialize(DataStream writer)
    {
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
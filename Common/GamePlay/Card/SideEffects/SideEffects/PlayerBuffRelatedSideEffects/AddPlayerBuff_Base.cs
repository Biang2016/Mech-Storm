public class AddPlayerBuff_Base : PlayerBuffSideEffects
{
    public SideEffectBundle.TriggerTime TriggerTime; //触发SE时机
    public SideEffectBundle.TriggerRange TriggerRange; //触发SE条件
    public int TriggerDelayTimes;
    public int TriggerTimes;
    public SideEffectBundle.TriggerTime RemoveTriggerTime; //移除SE时机
    public SideEffectBundle.TriggerRange RemoveTriggerRange; //移除SE条件
    public int RemoveTriggerTimes; //Remove触发多少次后，移除此SE（如：3回合内全体攻击力+1）


    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32((int) TriggerTime);
        writer.WriteSInt32((int) TriggerRange);
        writer.WriteSInt32(TriggerDelayTimes);
        writer.WriteSInt32(TriggerTimes);
        writer.WriteSInt32((int) RemoveTriggerTime);
        writer.WriteSInt32((int) RemoveTriggerRange);
        writer.WriteSInt32(RemoveTriggerTimes);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        TriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        TriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
        TriggerDelayTimes = reader.ReadSInt32();
        TriggerTimes = reader.ReadSInt32();
        RemoveTriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        RemoveTriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
        RemoveTriggerTimes = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddPlayerBuff_Base) copy).TriggerTime = TriggerTime;
        ((AddPlayerBuff_Base) copy).TriggerRange = TriggerRange;
        ((AddPlayerBuff_Base) copy).TriggerDelayTimes = TriggerDelayTimes;
        ((AddPlayerBuff_Base) copy).TriggerTimes = TriggerTimes;
        ((AddPlayerBuff_Base) copy).RemoveTriggerTime = RemoveTriggerTime;
        ((AddPlayerBuff_Base) copy).RemoveTriggerRange = RemoveTriggerRange;
        ((AddPlayerBuff_Base) copy).RemoveTriggerTimes = RemoveTriggerTimes;
    }
}
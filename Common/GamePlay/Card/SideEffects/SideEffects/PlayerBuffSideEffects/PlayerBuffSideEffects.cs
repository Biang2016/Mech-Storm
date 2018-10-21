using System;
using System.Collections.Generic;

public abstract class PlayerBuffSideEffects : SideEffectBase
{
    public int BuffPicId;
    public string BuffColor="";
    public bool HasNumberShow;
    public bool CanPiled;
    public bool Singleton;

    public SideEffectBundle.TriggerTime TriggerTime; //触发SE时机
    public SideEffectBundle.TriggerRange TriggerRange; //触发SE条件
    public int TriggerDelayTimes;
    public int TriggerTimes;
    public SideEffectBundle.TriggerTime RemoveTriggerTime; //移除SE时机
    public SideEffectBundle.TriggerRange RemoveTriggerRange; //移除SE条件
    public int RemoveTriggerTimes; //Remove触发多少次后，移除此SE（如：3回合内全体攻击力+1）

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuffPicId);
        writer.WriteString8(BuffColor);
        writer.WriteByte((byte) (HasNumberShow ? 0x01 : 0x00));
        writer.WriteByte((byte) (CanPiled ? 0x01 : 0x00));
        writer.WriteByte((byte) (Singleton ? 0x01 : 0x00));

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
        BuffPicId = reader.ReadSInt32();
        BuffColor = reader.ReadString8();
        HasNumberShow = reader.ReadByte() == 0x01;
        CanPiled = reader.ReadByte() == 0x01;
        Singleton = reader.ReadByte() == 0x01;

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
        ((PlayerBuffSideEffects) copy).BuffPicId = BuffPicId;
        ((PlayerBuffSideEffects) copy).BuffColor = BuffColor;
        ((PlayerBuffSideEffects) copy).HasNumberShow = HasNumberShow;
        ((PlayerBuffSideEffects) copy).CanPiled = CanPiled;
        ((PlayerBuffSideEffects) copy).Singleton = Singleton;

        ((PlayerBuffSideEffects) copy).TriggerTime = TriggerTime;
        ((PlayerBuffSideEffects) copy).TriggerRange = TriggerRange;
        ((PlayerBuffSideEffects) copy).TriggerDelayTimes = TriggerDelayTimes;
        ((PlayerBuffSideEffects) copy).TriggerTimes = TriggerTimes;
        ((PlayerBuffSideEffects) copy).RemoveTriggerTime = RemoveTriggerTime;
        ((PlayerBuffSideEffects) copy).RemoveTriggerRange = RemoveTriggerRange;
        ((PlayerBuffSideEffects) copy).RemoveTriggerTimes = RemoveTriggerTimes;
    }
}
public class ShowSideEffectTriggeredRequest : ServerRequestBase
{
    public SideEffectBase.ExecuterInfo ExecuterInfo;
    public SideEffectBundle.TriggerTime TriggerTime;
    public SideEffectBundle.TriggerRange TriggerRange;


    public ShowSideEffectTriggeredRequest()
    {
    }

    public ShowSideEffectTriggeredRequest(SideEffectBase.ExecuterInfo executerInfo, SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange)
    {
        ExecuterInfo = executerInfo;
        TriggerTime = triggerTime;
        TriggerRange = triggerRange;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT;
    }

    public override string GetProtocolName()
    {
        return "SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        ExecuterInfo.Serialize(writer);
        writer.WriteSInt32((int) TriggerTime);
        writer.WriteSInt32((int)TriggerRange);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ExecuterInfo = SideEffectBase.ExecuterInfo.Deserialize(reader);
        TriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        TriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [ExecuterInfo]=" + ExecuterInfo.DeserializeLog();
        log += " [TriggerTime]=" + TriggerTime;
        log += " [TriggerRange]=" + TriggerRange;
        return log;
    }
}
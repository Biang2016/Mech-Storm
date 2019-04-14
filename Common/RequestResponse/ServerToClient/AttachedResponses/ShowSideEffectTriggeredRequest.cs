public class ShowSideEffectTriggeredRequest : ServerRequestBase
{
    public ExecutorInfo ExecutorInfo;
    public SideEffectBundle.TriggerTime TriggerTime;
    public SideEffectBundle.TriggerRange TriggerRange;


    public ShowSideEffectTriggeredRequest()
    {
    }

    public ShowSideEffectTriggeredRequest(ExecutorInfo executorInfo, SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange)
    {
        ExecutorInfo = executorInfo;
        TriggerTime = triggerTime;
        TriggerRange = triggerRange;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        ExecutorInfo.Serialize(writer);
        writer.WriteSInt32((int) TriggerTime);
        writer.WriteSInt32((int)TriggerRange);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ExecutorInfo = ExecutorInfo.Deserialize(reader);
        TriggerTime = (SideEffectBundle.TriggerTime) reader.ReadSInt32();
        TriggerRange = (SideEffectBundle.TriggerRange) reader.ReadSInt32();
    }

}
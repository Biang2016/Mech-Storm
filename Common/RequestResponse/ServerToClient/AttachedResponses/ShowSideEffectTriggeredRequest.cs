public class ShowSideEffectTriggeredRequest : ServerRequestBase
{
    public ExecutorInfo ExecutorInfo;
    public SideEffectExecute.TriggerTime TriggerTime;
    public SideEffectExecute.TriggerRange TriggerRange;


    public ShowSideEffectTriggeredRequest()
    {
    }

    public ShowSideEffectTriggeredRequest(ExecutorInfo executorInfo, SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange)
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
        TriggerTime = (SideEffectExecute.TriggerTime) reader.ReadSInt32();
        TriggerRange = (SideEffectExecute.TriggerRange) reader.ReadSInt32();
    }

}
public abstract class PlayerBuffSideEffects : SideEffectBase
{
    public enum BuffPiledBy
    {
        RemoveTriggerTimes = 0,
        Value = 1,
    }

    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_ConstInt("BuffPicId", 0);
        M_SideEffectParam.SetParam_String("BuffColor", "#ffffff");
        M_SideEffectParam.SetParam_Bool("HasNumberShow", false);
        M_SideEffectParam.SetParam_Bool("CanPiled", false);
        M_SideEffectParam.SetParam_Bool("Singleton", false);
        M_SideEffectParam.SetParam_ConstInt("PiledBy", (int) BuffPiledBy.RemoveTriggerTimes, typeof(BuffPiledBy));

        M_SideEffectParam.SetParam_ConstInt("TriggerTime", (int) SideEffectExecute.TriggerTime.None, typeof(SideEffectExecute.TriggerTime)); //触发SE时机
        M_SideEffectParam.SetParam_ConstInt("TriggerRange", (int) SideEffectExecute.TriggerRange.None, typeof(SideEffectExecute.TriggerRange)); //触发SE条件
        M_SideEffectParam.SetParam_ConstInt("TriggerDelayTimes", 0);
        M_SideEffectParam.SetParam_ConstInt("TriggerTimes", 0);
        M_SideEffectParam.SetParam_ConstInt("RemoveTriggerTime", (int) SideEffectExecute.TriggerTime.None, typeof(SideEffectExecute.TriggerTime)); //移除SE时机
        M_SideEffectParam.SetParam_ConstInt("RemoveTriggerRange", (int) SideEffectExecute.TriggerRange.None, typeof(SideEffectExecute.TriggerRange)); //移除SE条件
        M_SideEffectParam.SetParam_ConstInt("RemoveTriggerTimes", 0); //Remove触发多少次后，移除此SE（如：3回合内全体攻击力+1）
    }
}
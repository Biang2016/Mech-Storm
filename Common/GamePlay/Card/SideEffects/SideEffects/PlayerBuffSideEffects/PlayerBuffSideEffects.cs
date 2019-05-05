public abstract class PlayerBuffSideEffects : SideEffectBase
{
    public SideEffectExecute MyBuffSEE;
    
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
    }
}
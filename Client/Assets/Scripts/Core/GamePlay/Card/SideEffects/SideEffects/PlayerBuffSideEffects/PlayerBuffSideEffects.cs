using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class PlayerBuffSideEffects : SideEffectBase
{
    public SideEffectExecute MyBuffSEE;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuffPiledBy
    {
        RemoveTriggerTimes = 0,
        RemoveTriggerDelayTimes = 1,
        Value = 2,
    }

    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_ConstInt("BuffPicId", 0);
        M_SideEffectParam.SetParam_String("BuffColor", "#ffffff");
        M_SideEffectParam.SetParam_Bool("HasNumberShow", false);
        M_SideEffectParam.SetParam_Bool("CanPiled", false);
        M_SideEffectParam.SetParam_Bool("Singleton", false);
        M_SideEffectParam.SetParam_ConstInt("PiledBy", (int) BuffPiledBy.RemoveTriggerTimes, typeof(BuffPiledBy));
    }
}
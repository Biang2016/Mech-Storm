public class DamageEnemyShipWhenUseCards_Base : PlayerBuffSideEffects
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], ((DamageOne_Base) Sub_SideEffect[0]).M_SideEffectParam.GetParam_MultipliedInt("Damage"), M_SideEffectParam.GetParam_ConstInt("RemoveTriggerTimes"));
    }
}
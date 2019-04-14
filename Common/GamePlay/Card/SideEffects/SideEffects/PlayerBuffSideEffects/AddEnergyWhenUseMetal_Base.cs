public class AddEnergyWhenUseMetal_Base : PlayerBuffSideEffects
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], ((AddEnergy_Base) Sub_SideEffect[0]).M_SideEffectParam.GetParam_MultipliedInt("EnergyValue"));
    }
}
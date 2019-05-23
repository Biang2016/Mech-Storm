public class AddEnergyWhenUseMetal_Base : PlayerBuffSideEffects
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
        base.InitSideEffectParam();
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], ((AddEnergy_Base) Sub_SideEffect[0]).M_SideEffectParam.GetParam_MultipliedInt("Energy"));
    }
}
public class Illusion_Base : TargetSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_MultipliedInt("Rounds", 0);
    }

    public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
            GetDescOfTargetRange(),
            M_SideEffectParam.GetParam_MultipliedInt("Rounds"));
    }
}
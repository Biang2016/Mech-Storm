public class SummonRetinuePerMechs_Base : TargetSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_ConstInt("SummonCardId", 0);
    }

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardId")).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange((TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"), false, false), bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
    }
}
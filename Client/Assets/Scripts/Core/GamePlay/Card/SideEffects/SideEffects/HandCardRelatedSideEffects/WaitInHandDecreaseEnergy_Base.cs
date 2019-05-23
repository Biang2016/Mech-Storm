public class WaitInHandDecreaseEnergy_Base : HandCardRelatedSideEffect
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("DecValue", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("DecValue"));
    }
}
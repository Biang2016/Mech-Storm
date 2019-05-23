public class TriggerHandCard_Base : HandCardRelatedSideEffect
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
    }
}
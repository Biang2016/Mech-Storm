public class DrawTypeCards_Base : CardDeckRelatedSideEffects
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        M_SideEffectParam.SetParam_ConstInt("DrawCardType", (int) CardTypes.Energy, typeof(CardTypes));
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("CardCount"), BaseInfo.CardTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][(CardTypes) M_SideEffectParam.GetParam_ConstInt("DrawCardType")],
            M_SideEffectParam.GetParam_MultipliedInt("CardCount") <= 1 ? "" : "s");
    }
}
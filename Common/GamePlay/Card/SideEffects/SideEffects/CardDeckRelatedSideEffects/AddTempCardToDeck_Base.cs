public class AddTempCardToDeck_Base : CardDeckRelatedSideEffects
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        M_SideEffectParam.SetParam_ConstInt("CardID", 0, typeof(CardDeck));
    }

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("CardID")).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("CardCount"), "[" + bi.CardNames[LanguageManager_Common.GetCurrentLanguage()] + "]");
    }
}
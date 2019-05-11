public class SummonMech_Base : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_ConstInt("SummonCardID", 0,typeof(CardDeck));
    }

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
    }
}
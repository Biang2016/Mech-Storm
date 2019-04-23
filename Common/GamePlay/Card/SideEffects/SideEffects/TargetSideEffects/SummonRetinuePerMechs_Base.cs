﻿public class SummonRetinuePerMechs_Base : TargetSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_ConstInt("SummonCardID", 0, typeof(CardDeck));
    }

    public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.RetinueBased;

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
            GetDescOfTargetRange(),
            bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
    }
}
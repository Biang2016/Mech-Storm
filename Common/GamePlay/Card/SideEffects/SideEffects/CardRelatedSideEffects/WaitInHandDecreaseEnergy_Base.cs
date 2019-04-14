﻿public class WaitInHandDecreaseEnergy_Base : CardRelatedSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_MultipliedInt("DecValue", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("DecValue"));
    }
}
﻿public class AddAttackAll_Base : TargetSideEffect
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_MultipliedInt("AttackValue", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange((TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"), true, false), M_SideEffectParam.GetParam_MultipliedInt("AttackValue"));
    }
}
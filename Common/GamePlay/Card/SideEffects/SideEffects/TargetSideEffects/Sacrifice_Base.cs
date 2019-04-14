public class Sacrifice_Base : TargetSideEffect, IDamage
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_MultipliedInt("ValueBasic", 0);
        M_SideEffectParam.SetParam_MultipliedInt("ValuePlus", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange((TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"), false, false), M_SideEffectParam.GetParam_MultipliedInt("ValueBasic"),
            M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
    }

    public int CalculateDamage()
    {
        return 0;
    }

    public IDamageType IDamageType
    {
        get { return IDamageType.UnknownValue; }
    }
}
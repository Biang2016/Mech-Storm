public class DamageRandom_Base : TargetSideEffect, IDamage
{
    protected override void InitSideEffectParam()
    {
        base.InitSideEffectParam();
        M_SideEffectParam.SetParam_MultipliedInt("Damage", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange((TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"), false, true), M_SideEffectParam.GetParam_MultipliedInt("Damage"));
    }

    public int CalculateDamage()
    {
        return M_SideEffectParam.GetParam_MultipliedInt("Damage");
    }

    public IDamageType IDamageType
    {
        get { return IDamageType.Known; }
    }
}
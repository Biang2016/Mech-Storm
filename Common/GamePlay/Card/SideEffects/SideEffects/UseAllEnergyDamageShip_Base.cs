public class UseAllEnergyDamageShip_Base : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_MultipliedInt("Value", 0);
        M_SideEffectParam.SetParam_MultipliedInt("ValuePlus", 0);
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("Value"), M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
    }
}
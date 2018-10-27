public class AddEnergyWhenUseMetal_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, ((AddEnergy_Base) Sub_SideEffect[0]).FinalValue);
    }
}
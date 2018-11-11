public class AddEnergyWhenRoundBegin_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, RemoveTriggerTimes, ((AddEnergy_Base) Sub_SideEffect[0]).FinalValue);
    }
}
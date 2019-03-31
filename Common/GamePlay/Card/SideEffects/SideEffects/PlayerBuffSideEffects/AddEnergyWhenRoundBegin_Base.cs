public class AddEnergyWhenRoundBegin_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], RemoveTriggerTimes, ((AddEnergy_Base) Sub_SideEffect[0]).FinalValue);
    }
}
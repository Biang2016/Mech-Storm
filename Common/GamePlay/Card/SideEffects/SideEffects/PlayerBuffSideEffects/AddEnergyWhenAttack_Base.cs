﻿public class AddEnergyWhenAttack_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], RemoveTriggerTimes, ((AddEnergy_Base) Sub_SideEffect[0]).FinalValue);
    }
}
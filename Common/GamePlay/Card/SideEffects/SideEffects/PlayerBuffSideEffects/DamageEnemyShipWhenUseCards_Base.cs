public class DamageEnemyShipWhenUseCards_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], ((DamageOne_Base) Sub_SideEffect[0]).FinalValue, RemoveTriggerTimes);
    }
}
public class DamageEnemyShipWhenUseCards_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], ((DamageOne_Base) Sub_SideEffect[0]).FinalValue, RemoveTriggerTimes);
    }
}
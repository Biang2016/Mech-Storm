public class DamageEnemyShipWhenAttack_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], RemoveTriggerTimes, ((DamageOne_Base) Sub_SideEffect[0]).FinalValue);
    }
}
public class DamageEnemyShipWhenUseCards_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, ((DamageOne_Base) Sub_SideEffect[0]).FinalValue, RemoveTriggerTimes);
    }
}
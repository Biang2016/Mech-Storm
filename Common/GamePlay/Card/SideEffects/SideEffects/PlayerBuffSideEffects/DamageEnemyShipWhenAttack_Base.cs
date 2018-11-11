public class DamageEnemyShipWhenAttack_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, RemoveTriggerTimes, ((DamageOne_Base) Sub_SideEffect[0]).FinalValue);
    }
}
public class DoSthWhenTrigger_RemoveBySomeTime_Base : PlayerBuffSideEffects
{
    public override string GenerateDesc()
    {
        string sub_desc = "";
        foreach (SideEffectBase sub_se in Sub_SideEffect)
        {
            sub_desc += sub_se.GenerateDesc().TrimEnd("，。;,.;/n ".ToCharArray()) + " & ";
        }

        sub_desc = sub_desc.TrimEnd("& ".ToCharArray());

        return HighlightStringFormat(
            DescRaws[LanguageManager_Common.GetCurrentLanguage()],
            new[] {false, false, false, false},
            SideEffectExecute.GetRemoveTriggerTimeTriggerRangeDescCombination(MyBuffSEE.M_ExecuteSetting.RemoveTriggerTime, MyBuffSEE.M_ExecuteSetting.RemoveTriggerTimes, MyBuffSEE.M_ExecuteSetting.RemoveTriggerRange),
            SideEffectExecute.GetEachTimeDesc(),
            SideEffectExecute.GetTriggerTimeTriggerRangeDescCombination(MyBuffSEE.M_ExecuteSetting.TriggerTime, MyBuffSEE.M_ExecuteSetting.TriggerRange),
            sub_desc);
    }
}
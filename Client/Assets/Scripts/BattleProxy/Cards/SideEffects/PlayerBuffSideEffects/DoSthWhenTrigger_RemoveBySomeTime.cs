namespace SideEffects
{
    public class DoSthWhenTrigger_RemoveBySomeTime : PlayerBuffSideEffects
    {
        public DoSthWhenTrigger_RemoveBySomeTime()
        {
        }

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
                SideEffectExecute.GetTriggerTimeTriggerRangeDescCombination(MyBuffSEE.M_ExecuteSetting.TriggerTime, MyBuffSEE.M_ExecuteSetting.TriggerRange),
                sub_desc);
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.PlayerBuffTrigger(M_ExecutorInfo.SideEffectExecutorID, this);
            foreach (SideEffectBase se in Sub_SideEffect)
            {
                se.Player = player;
                se.Execute(executorInfo);
            }
        }
    }
}
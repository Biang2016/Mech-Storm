namespace SideEffects
{
    public class IllusionToOne : IllusionToOne_Base
    {
        public IllusionToOne()
        {
        }

        public override string GenerateDesc()
        {
            return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetChineseDescOfTargetRange(M_TargetRange, false, false), FinalValue);
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.SelfMechs) != 0)
            {
                player.MyBattleGroundManager.GetRetinue(executerInfo.TargetRetinueId).M_ImmuneLeftRounds += 1;
                player.MyBattleGroundManager.GetRetinue(executerInfo.TargetRetinueId).M_InactivityRounds += 1;
            }
        }
    }
}
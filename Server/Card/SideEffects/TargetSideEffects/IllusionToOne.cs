namespace SideEffects
{
    public class IllusionToOne : IllusionToOne_Base
    {
        public IllusionToOne()
        {
        }

        public override string GenerateDesc(bool isEnglish)
        {
            return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, false, false), FinalValue);
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
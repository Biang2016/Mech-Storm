namespace SideEffects
{
    public class IllusionToOne : IllusionToOne_Base
    {
        public IllusionToOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.SelfMechs) != 0)
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.TargetRetinueId).M_ImmuneLeftRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                player.MyBattleGroundManager.GetRetinue(executorInfo.TargetRetinueId).M_InactivityRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
            }
        }
    }
}
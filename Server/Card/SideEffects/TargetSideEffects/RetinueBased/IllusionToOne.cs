namespace SideEffects
{
    public class Illusion : Illusion_Base
    {
        public Illusion()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int retinueId in executorInfo.TargetRetinueIds)
                {
                    player.MyGameManager.GetRetinue(retinueId).M_ImmuneLeftRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                    player.MyGameManager.GetRetinue(retinueId).M_InactivityRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                }
            }
        }
    }
}
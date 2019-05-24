namespace SideEffects
{
    public class Illusion : Illusion_Base
    {
        public Illusion()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int mechId in executorInfo.TargetMechIds)
                {
                    player.GameManager.GetMech(mechId).M_ImmuneLeftRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                    player.GameManager.GetMech(mechId).M_InactivityRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                }
            }
        }
    }
}
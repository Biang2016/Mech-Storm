namespace SideEffects
{
    public class Kill : Kill_Base
    {
        public Kill()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.KillMechs(ChoiceCount, executorInfo.TargetMechIds, player, TargetRange, TargetSelect, -1);
        }
    }
}
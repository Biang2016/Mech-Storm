namespace SideEffects
{
    public class Kill : Kill_Base
    {
        public Kill()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyGameManager.KillMechs(ChoiceCount,executorInfo.TargetMechIds,player,TargetRange,TargetSelect,-1);
        }
    }
}
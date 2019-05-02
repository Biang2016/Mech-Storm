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
            player.MyGameManager.KillRetinues(ChoiceCount,executorInfo.TargetRetinueIds,player,TargetRange,TargetSelect,-1);
        }
    }
}
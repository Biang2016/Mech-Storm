namespace SideEffects
{
    public class AddAttackOne : AddAttackOne_Base
    {
        public AddAttackOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyBattleGroundManager.AddAttackForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
            player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
        }
    }
}
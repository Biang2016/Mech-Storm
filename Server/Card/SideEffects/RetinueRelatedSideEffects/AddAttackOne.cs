namespace SideEffects
{
    public class AddAttackOne : AddAttackOne_Base
    {
        public AddAttackOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyBattleGroundManager.AddAttackForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
        }
    }
}
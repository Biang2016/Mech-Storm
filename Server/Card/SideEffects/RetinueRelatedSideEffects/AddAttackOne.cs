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
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");
            player.MyBattleGroundManager.AddAttackForOneRetinue(executorInfo.TargetRetinueId, value);
            player.MyEnemyPlayer.MyBattleGroundManager.AddAttackForOneRetinue(executorInfo.TargetRetinueId, value);
        }
    }
}
namespace SideEffects
{
    public class AddShieldOne : AddShieldOne_Base
    {
        public AddShieldOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddShieldForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.AddShieldForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddShieldForOneRetinue(executorInfo.RetinueId, FinalValue);
            }
        }
    }
}
namespace SideEffects
{
    public class AddArmorOne : AddArmorOne_Base
    {
        public AddArmorOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.TargetRetinueId, FinalValue);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.RetinueId, FinalValue);
            }
        }
    }
}
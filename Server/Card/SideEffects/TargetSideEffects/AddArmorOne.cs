namespace SideEffects
{
    public class AddArmorOne : AddArmorOne_Base
    {
        public AddArmorOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.AddArmorForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}
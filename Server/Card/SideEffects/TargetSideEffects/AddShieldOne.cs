namespace SideEffects
{
    public class AddShieldOne : AddShieldOne_Base
    {
        public AddShieldOne()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddShieldForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
                player.MyEnemyPlayer.MyBattleGroundManager.AddShieldForOneRetinue(executerInfo.TargetRetinueId, FinalValue);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddShieldForOneRetinue(executerInfo.RetinueId, FinalValue);
            }
        }
    }
}
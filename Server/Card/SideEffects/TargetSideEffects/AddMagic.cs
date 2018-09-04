namespace SideEffects
{
    public class AddMagic : AddMagic_Base
    {
        public AddMagic()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Ships:
                    player.AddMagicWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddMagicWithinMax(FinalValue);
                    break;
                case TargetRange.SelfShip:
                    player.AddMagicWithinMax(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddMagicWithinMax(FinalValue);
                    break;
            }
        }
    }
}
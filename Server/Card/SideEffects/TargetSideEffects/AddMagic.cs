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
                    player.AddMagicWithinMax(Value);
                    player.MyEnemyPlayer.AddMagicWithinMax(Value);
                    break;
                case TargetRange.SelfShip:
                    player.AddMagicWithinMax(Value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddMagicWithinMax(Value);
                    break;
            }
        }
    }
}
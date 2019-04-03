namespace SideEffects
{
    public class AddEnergy : AddEnergy_Base
    {
        public AddEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.Ships:
                    player.AddEnergyWithinMax(FinalValue);
                    player.MyEnemyPlayer.AddEnergyWithinMax(FinalValue);
                    break;
                case TargetRange.SelfShip:
                    player.AddEnergyWithinMax(FinalValue);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddEnergyWithinMax(FinalValue);
                    break;
            }
        }
    }
}
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
            int value = M_SideEffectParam.GetParam_MultipliedInt("EnergyValue");
            switch (M_TargetRange)
            {
                case TargetRange.Ships:
                    player.AddEnergyWithinMax(value);
                    player.MyEnemyPlayer.AddEnergyWithinMax(value);
                    break;
                case TargetRange.SelfShip:
                    player.AddEnergyWithinMax(value);
                    break;
                case TargetRange.EnemyShip:
                    player.MyEnemyPlayer.AddEnergyWithinMax(value);
                    break;
            }
        }
    }
}
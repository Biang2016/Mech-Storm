namespace SideEffects
{
    public class UseAllEnergyDamageShip : UseAllEnergyDamageShip_Base
    {
        public UseAllEnergyDamageShip()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            player.MyEnemyPlayer.DamageLifeAboveZero(FinalValue);
            int plus_damage = player.EnergyLeft;
            player.UseEnergyAboveZero(player.EnergyLeft);
            player.MyEnemyPlayer.DamageLifeAboveZero(plus_damage * FinalValue_Plus);
        }
    }
}
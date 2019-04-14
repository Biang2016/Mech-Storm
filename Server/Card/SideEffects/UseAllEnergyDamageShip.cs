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

            player.MyEnemyPlayer.DamageLifeAboveZero(M_SideEffectParam.GetParam_MultipliedInt("Value"));
            int plus_damage = player.EnergyLeft;
            player.UseEnergyAboveZero(player.EnergyLeft);
            player.MyEnemyPlayer.DamageLifeAboveZero(plus_damage * M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
        }
    }
}
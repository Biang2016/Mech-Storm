namespace SideEffects
{
    public class UseAllEnergyDamageShip : SideEffectBase
    {
        public UseAllEnergyDamageShip()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("Value", 0);
            M_SideEffectParam.SetParam_MultipliedInt("ValuePlus", 0);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("Value"), M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            player.MyEnemyPlayer.Damage(M_SideEffectParam.GetParam_MultipliedInt("Value"));
            int plus_damage = player.EnergyLeft;
            player.UseAllEnergy();
            player.MyEnemyPlayer.Damage(plus_damage * M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
        }
    }
}
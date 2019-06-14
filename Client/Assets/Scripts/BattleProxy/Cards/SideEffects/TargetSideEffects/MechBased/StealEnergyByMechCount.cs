using System;

namespace SideEffects
{
    public class StealEnergyByMechCount : TargetSideEffect
    {
        public StealEnergyByMechCount()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.EveryMechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Energy"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int energyValue = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            int finalValue = energyValue * player.GameManager.CountMechsByTargetRange(TargetRange, player);
            finalValue = Math.Min(finalValue, player.MyEnemyPlayer.EnergyLeft);
            player.AddEnergy(finalValue);
            player.MyEnemyPlayer.UseEnergy(finalValue);
        }
    }
}
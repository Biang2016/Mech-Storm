namespace SideEffects
{
    public class DoubleEnergy : TargetSideEffect, IShipEnergy
    {
        public DoubleEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.ShipBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange());
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp) { sp.AddEnergy(sp.EnergyLeft); },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }

        public int GetSideEffectFunctionBias()
        {
            return 5;
        }
    }
}
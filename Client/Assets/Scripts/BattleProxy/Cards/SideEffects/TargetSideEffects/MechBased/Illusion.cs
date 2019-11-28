namespace SideEffects
{
    public class Illusion : TargetSideEffect, IDefend
    {
        public Illusion()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Rounds", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Rounds"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int mechId in executorInfo.TargetMechIds)
                {
                    player.GameManager.GetMech(mechId).M_ImmuneLeftRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                    player.GameManager.GetMech(mechId).M_InactivityRounds += M_SideEffectParam.GetParam_MultipliedInt("Rounds");
                }
            }
        }

        public int GetSideEffectFunctionBias()
        {
            return 5;
        }
    }
}
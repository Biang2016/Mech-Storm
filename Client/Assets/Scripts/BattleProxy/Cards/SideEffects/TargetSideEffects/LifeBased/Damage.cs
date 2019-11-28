namespace SideEffects
{
    public class Damage : TargetSideEffect, IDamage, INegative
    {
        public Damage()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Damage", 0);
            M_SideEffectParam.SetParam_ConstInt("DamageTimes", 1);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.LifeBased;

        public override string GenerateDesc()
        {
            int times = M_SideEffectParam.GetParam_ConstInt("DamageTimes");
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Damage"),
                times == 1 ? "" : ("*" + M_SideEffectParam.GetParam_ConstInt("DamageTimes")));
        }

        public int CalculateDamage()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("Damage");
        }

        public IDamageType IDamageType
        {
            get { return IDamageType.Known; }
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Damage");
            int times = M_SideEffectParam.GetParam_ConstInt("DamageTimes");
            player.GameManager.SideEffect_ILifeAction(
                delegate(ILife life)
                {
                    for (int i = 0; i < times; i++)
                    {
                        life.Damage(value);
                    }
                },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds,
                executorInfo.TargetMechIds
            );
        }

        public int GetSideEffectFunctionBias()
        {
            return -M_SideEffectParam.GetParam_MultipliedInt("Damage") * M_SideEffectParam.GetParam_ConstInt("DamageTimes");
        }
    }
}
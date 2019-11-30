namespace SideEffects
{
    public class DecreaseAttack : TargetSideEffect, IWeaken
    {
        public DecreaseAttack()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("AttackValue", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("AttackValue"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                int attack = player.GameManager.GetMech(executorInfo.MechId).M_MechAttack;
                if (attack >= value)
                {
                    attack -= value;
                }
                else
                {
                    attack = 0;
                }

                player.GameManager.GetMech(executorInfo.MechId).M_MechAttack = attack;
            }
            else
            {
                player.GameManager.SideEffect_MechAction(
                    delegate(ModuleMech mech) { mech.M_MechAttack += value; },
                    player,
                    0,
                    executorInfo.TargetMechIds,
                    TargetRange,
                    TargetSelect,
                    -1
                );
            }
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return -M_SideEffectParam.GetParam_MultipliedInt("AttackValue") * 2;
        }
    }
}
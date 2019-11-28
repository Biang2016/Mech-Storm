using System;
using UnityEngine;

namespace SideEffects
{
    public class HealLifeMaxPercent : TargetSideEffect, IPositive
    {
        public HealLifeMaxPercent()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("Percent", 10);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.LifeBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("Percent"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int healPercent = M_SideEffectParam.GetParam_MultipliedInt("Percent");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                ModuleMech mech = player.BattleGroundManager.GetMech(executorInfo.MechId);
                int healValue = (int) Math.Ceiling((float) healPercent * mech.M_MechTotalLife / 100);
                player.BattleGroundManager.GetMech(executorInfo.MechId).Heal(healValue);
            }
            else
            {
                player.GameManager.SideEffect_ILifeAction(
                    delegate(ILife life)
                    {
                        int healValue = (int) Math.Ceiling((float) healPercent * life.GetTotalLife() / 100);
                        life.Heal(healValue);
                    },
                    player,
                    ChoiceCount,
                    TargetRange,
                    TargetSelect,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetMechIds
                );
            }
        }

        public int GetSideEffectFunctionBias()
        {
            return Mathf.RoundToInt(M_SideEffectParam.GetParam_MultipliedInt("Percent") / 10f) * 3;
        }
    }
}
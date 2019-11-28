using System.Collections.Generic;

namespace SideEffects
{
    public class Sacrifice : TargetSideEffect, IDefend, IPostUsed
    {
        public Sacrifice()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("ValueBasic", 0);
            M_SideEffectParam.SetParam_MultipliedInt("ValuePlus", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override List<TargetSelect> ValidTargetSelects => new List<TargetSelect> {TargetSelect.All};

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("ValueBasic"),
                M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
        }

        public int CalculateDamage()
        {
            return 0;
        }

        public IDamageType IDamageType
        {
            get { return IDamageType.UnknownValue; }
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            ModuleMech mech;
            if (TargetRange == TargetRange.Self)
            {
                mech = player.BattleGroundManager.GetMech(executorInfo.MechId);
            }
            else
            {
                mech = player.BattleGroundManager.GetMech(executorInfo.TargetMechIds[0]);
            }

            int equipCount = 0;
            int plusDamage = M_SideEffectParam.GetParam_MultipliedInt("ValuePlus");
            if (mech.M_Weapon != null)
            {
                mech.M_Weapon = null;
                equipCount++;
            }

            if (mech.M_Shield != null)
            {
                mech.M_Shield = null;
                equipCount++;
            }

            if (mech.M_Pack != null)
            {
                mech.M_Pack = null;
                equipCount++;
            }

            if (mech.M_MA != null)
            {
                mech.M_MA = null;
                equipCount++;
            }

            player.GameManager.SideEffect_ILifeAction(
                delegate(ILife life) { life.Damage(M_SideEffectParam.GetParam_MultipliedInt("ValueBasic")); },
                player,
                ChoiceCount,
                TargetRange.EnemyLife,
                TargetSelect.All,
                executorInfo.TargetClientIds,
                executorInfo.TargetMechIds
            );
            for (int i = 0; i < equipCount; i++)
            {
                player.GameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Damage(plusDamage); },
                    player,
                    ChoiceCount,
                    TargetRange.EnemyLife,
                    TargetSelect.All,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetMechIds
                );
            }

            player.GameManager.KillMechs(new List<int> {mech.M_MechID});
        }

        public int GetSideEffectFunctionBias()
        {
            return (M_SideEffectParam.GetParam_MultipliedInt("ValueBasic") + M_SideEffectParam.GetParam_MultipliedInt("ValuePlus")) * 2;
        }
    }
}
using System;

namespace SideEffects
{
    //only used in mech's effect
    public class GetChanceToAttackAgain : SideEffectBase
    {
        public GetChanceToAttackAgain()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_ConstInt("Chance", 50);
            M_SideEffectParam.SetParam_MultipliedInt("AttackTimes", 1);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_ConstInt("Chance"), M_SideEffectParam.GetParam_MultipliedInt("AttackTimes"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            ModuleMech mech = player.BattleGroundManager.GetMech(executorInfo.MechId);
            if (mech != null)
            {
                int chance = M_SideEffectParam.GetParam_ConstInt("Chance");
                int attackTimes = M_SideEffectParam.GetParam_MultipliedInt("AttackTimes");

                var r = new Random();
                int random = r.Next(0, 100);

                if (!mech.IsSentry)
                {
                    if (random < chance)
                    {
                        mech.AttackTimesThisRound += attackTimes;
                    }
                }
            }
        }
    }
}
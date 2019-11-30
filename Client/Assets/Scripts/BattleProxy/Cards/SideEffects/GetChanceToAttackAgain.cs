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
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("AttackTimes", 1);
        }

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("AttackTimes"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            ModuleMech mech = player.BattleGroundManager.GetMech(executorInfo.MechId);
            if (mech != null)
            {
                int attackTimes = M_SideEffectParam.GetParam_MultipliedInt("AttackTimes");

                if (!mech.IsSentry)
                {
                    mech.AttackTimesThisRound += attackTimes;
                }
            }
            return true;
        }
    }
}
using System.Collections.Generic;

namespace SideEffects
{
    public class KillAllMechsAndDamageSelfShip : SideEffectBase, IPriorUsed
    {
        public KillAllMechsAndDamageSelfShip()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("Damage", 3);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("Damage"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            int count = player.BattleGroundManager.MechCount + player.MyEnemyPlayer.BattleGroundManager.MechCount;
            player.Damage(count * M_SideEffectParam.GetParam_ConstInt("Damage"));

            List<int> killMechIds = new List<int>();
            foreach (ModuleMech mech in player.BattleGroundManager.Mechs)
            {
                killMechIds.Add(mech.M_MechID);
            }

            foreach (ModuleMech mech in player.MyEnemyPlayer.BattleGroundManager.Mechs)
            {
                killMechIds.Add(mech.M_MechID);
            }

            player.GameManager.KillMechs(killMechIds);
        }
    }
}
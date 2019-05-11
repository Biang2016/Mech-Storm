using System.Collections.Generic;

namespace SideEffects
{
    public class Sacrifice : Sacrifice_Base
    {
        public Sacrifice()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            ServerModuleMech mech;
            if (TargetRange == TargetRange.Self)
            {
                mech = player.MyBattleGroundManager.GetMech(executorInfo.MechId);
            }
            else
            {
                mech = player.MyBattleGroundManager.GetMech(executorInfo.TargetMechIds[0]);
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

            player.MyGameManager.SideEffect_ILifeAction(
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
                player.MyGameManager.SideEffect_ILifeAction(
                    delegate(ILife life) { life.Damage(plusDamage); },
                    player,
                    ChoiceCount,
                    TargetRange.EnemyLife,
                    TargetSelect.All,
                    executorInfo.TargetClientIds,
                    executorInfo.TargetMechIds
                );
            }

            player.MyGameManager.KillMechs(new List<int> {mech.M_MechID});
        }
    }
}
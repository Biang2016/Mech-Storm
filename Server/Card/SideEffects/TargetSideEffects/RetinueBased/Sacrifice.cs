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
            ServerModuleRetinue retinue;
            if (TargetRange == TargetRange.Self)
            {
                retinue = player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId);
            }
            else
            {
                retinue = player.MyBattleGroundManager.GetRetinue(executorInfo.TargetRetinueIds[0]);
            }

            int equipCount = 0;
            int plusDamage = M_SideEffectParam.GetParam_MultipliedInt("ValuePlus");
            if (retinue.M_Weapon != null)
            {
                retinue.M_Weapon = null;
                equipCount++;
            }

            if (retinue.M_Shield != null)
            {
                retinue.M_Shield = null;
                equipCount++;
            }

            if (retinue.M_Pack != null)
            {
                retinue.M_Pack = null;
                equipCount++;
            }

            if (retinue.M_MA != null)
            {
                retinue.M_MA = null;
                equipCount++;
            }

            player.MyGameManager.SideEffect_ILifeAction(
                delegate(ILife life) { life.Damage(M_SideEffectParam.GetParam_MultipliedInt("ValueBasic")); },
                player,
                ChoiceCount,
                TargetRange.EnemyLife,
                TargetSelect.All,
                executorInfo.TargetClientIds,
                executorInfo.TargetRetinueIds
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
                    executorInfo.TargetRetinueIds
                );
            }

            player.MyGameManager.KillRetinues(new List<int> {retinue.M_RetinueID});
        }
    }
}
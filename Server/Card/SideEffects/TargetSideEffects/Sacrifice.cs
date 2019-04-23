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
            switch (TargetRange)
            {
                case TargetRange.SelfMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(M_SideEffectParam.GetParam_MultipliedInt("ValueBasic"));

                    ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executorInfo.TargetRetinueId);
                    if (retinue.M_Weapon != null)
                    {
                        retinue.M_Weapon = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
                    }

                    if (retinue.M_Shield != null)
                    {
                        retinue.M_Shield = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
                    }

                    if (retinue.M_Pack != null)
                    {
                        retinue.M_Pack = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
                    }

                    if (retinue.M_MA != null)
                    {
                        retinue.M_MA = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(M_SideEffectParam.GetParam_MultipliedInt("ValuePlus"));
                    }

                    player.MyBattleGroundManager.KillOneRetinue(executorInfo.TargetRetinueId);
                    break;
            }
        }
    }
}
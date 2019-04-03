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
            switch (M_TargetRange)
            {
                case TargetRange.SelfMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValueBasic);

                    ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executorInfo.TargetRetinueId);
                    if (retinue.M_Weapon != null)
                    {
                        retinue.M_Weapon = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValuePlus);
                    }

                    if (retinue.M_Shield != null)
                    {
                        retinue.M_Shield = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValuePlus);
                    }

                    if (retinue.M_Pack != null)
                    {
                        retinue.M_Pack = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValuePlus);
                    }

                    if (retinue.M_MA != null)
                    {
                        retinue.M_MA = null;
                        player.MyEnemyPlayer.MyBattleGroundManager.DamageAllRetinues(FinalValuePlus);
                    }

                    player.MyBattleGroundManager.KillOneRetinue(executorInfo.TargetRetinueId);
                    break;
            }
        }
    }
}
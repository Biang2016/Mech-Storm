namespace SideEffects
{
    public class Cooperation : Cooperation_Base
    {
        public Cooperation()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                int life = 0;
                int shield = 0;
                int armor = 0;
                foreach (int targetRetinueId in executorInfo.TargetRetinueIds)
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRetinue(targetRetinueId);
                    life += retinue.M_RetinueLeftLife;
                    shield += retinue.M_RetinueShield;
                    armor += retinue.M_RetinueArmor;
                }

                player.MyGameManager.KillRetinues(executorInfo.TargetRetinueIds);

                ServerModuleRetinue targetHeroRetinue = null;
                int tempLife = 99999;
                foreach (ServerModuleRetinue hero in player.MyBattleGroundManager.Heroes)
                {
                    if (hero.M_RetinueLeftLife < tempLife)
                    {
                        tempLife = hero.M_RetinueLeftLife;
                        targetHeroRetinue = hero;
                    }
                }

                if (targetHeroRetinue != null)
                {
                    targetHeroRetinue.AddLife(life);
                    targetHeroRetinue.M_RetinueShield += shield;
                    targetHeroRetinue.M_RetinueArmor += armor;
                }
            }
        }
    }
}
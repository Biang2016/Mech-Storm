using System.Collections.Generic;

namespace SideEffects
{
    public class Cooperation : Cooperation_Base
    {
        public Cooperation()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((M_TargetRange & TargetRange.SelfSoldiers) != 0)
            {
                ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executerInfo.TargetRetinueId);
                int life = retinue.M_RetinueLeftLife;
                int shield = retinue.M_RetinueShield;
                int armor = retinue.M_RetinueArmor;

                player.MyBattleGroundManager.KillOneRetinue(executerInfo.TargetRetinueId);
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

                player.MyBattleGroundManager.AddLifeForOneRetinue(targetHeroRetinue.M_RetinueID,life);
                targetHeroRetinue.M_RetinueShield += shield;
                targetHeroRetinue.M_RetinueArmor += armor;
            }
        }
    }
}
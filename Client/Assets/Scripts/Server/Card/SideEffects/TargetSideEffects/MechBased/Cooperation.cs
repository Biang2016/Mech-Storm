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
                foreach (int targetMechId in executorInfo.TargetMechIds)
                {
                    ServerModuleMech mech = player.MyGameManager.GetMech(targetMechId);
                    life += mech.M_MechLeftLife;
                    shield += mech.M_MechShield;
                    armor += mech.M_MechArmor;
                }

                player.MyGameManager.KillMechs(executorInfo.TargetMechIds);

                ServerModuleMech targetHeroMech = null;
                int tempLife = 99999;
                foreach (ServerModuleMech hero in player.MyBattleGroundManager.Heroes)
                {
                    if (hero.M_MechLeftLife < tempLife)
                    {
                        tempLife = hero.M_MechLeftLife;
                        targetHeroMech = hero;
                    }
                }

                if (targetHeroMech != null)
                {
                    targetHeroMech.AddLife(life);
                    targetHeroMech.M_MechShield += shield;
                    targetHeroMech.M_MechArmor += armor;
                }
            }
        }
    }
}
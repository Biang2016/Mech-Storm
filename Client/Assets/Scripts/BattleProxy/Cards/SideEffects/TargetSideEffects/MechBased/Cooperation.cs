namespace SideEffects
{
    public class Cooperation : TargetSideEffect, IStrengthen
    {
        public Cooperation()
        {
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.MechBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                int life = 0;
                int shield = 0;
                int armor = 0;
                foreach (int targetMechId in executorInfo.TargetMechIds)
                {
                    ModuleMech mech = player.GameManager.GetMech(targetMechId);
                    life += mech.M_MechLeftLife;
                    shield += mech.M_MechShield;
                    armor += mech.M_MechArmor;
                }

                player.GameManager.KillMechs(executorInfo.TargetMechIds);

                ModuleMech targetHeroMech = null;
                int tempLife = 99999;
                foreach (ModuleMech hero in player.BattleGroundManager.Heroes)
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

        public int GetSideEffectFunctionBias()
        {
            return 5;
        }
    }
}
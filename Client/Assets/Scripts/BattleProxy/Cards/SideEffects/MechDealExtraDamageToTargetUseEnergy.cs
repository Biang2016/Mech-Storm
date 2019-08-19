namespace SideEffects
{
    //only used in mech's effect
    public class MechDealExtraDamageToTargetUseEnergy : SideEffectBase
    {
        public MechDealExtraDamageToTargetUseEnergy()
        {
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_MultipliedInt("Energy", 0);
            M_SideEffectParam.SetParam_MultipliedInt("Damage", 0);
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], M_SideEffectParam.GetParam_MultipliedInt("Energy"), M_SideEffectParam.GetParam_MultipliedInt("Damage"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int energy = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            if (player.EnergyLeft >= energy)
            {
                player.UseEnergy(energy);
                foreach (int mechID in executorInfo.TargetMechIds)
                {
                    int damage = M_SideEffectParam.GetParam_MultipliedInt("Damage");
                    player.GameManager.GetMech(executorInfo.MechId).OnMakeDamage(damage);
                    player.GameManager.GetMech(mechID).Damage(damage);
                }
            }
        }
    }
}
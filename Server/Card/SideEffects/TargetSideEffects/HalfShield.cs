namespace SideEffects
{
    public class HalfShield : HalfShield_Base
    {
        public HalfShield()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if (TR_Retinues.Contains(M_TargetRange))
            {
                ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(executerInfo.TargetRetinueId);
                if (retinue != null && retinue.M_RetinueShield != 0)
                {
                    retinue.M_RetinueShield = retinue.M_RetinueShield / 2;
                }
            }
        }
    }
}
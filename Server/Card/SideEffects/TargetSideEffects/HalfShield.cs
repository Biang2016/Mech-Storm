namespace SideEffects
{
    public class HalfShield : HalfShield_Base
    {
        public HalfShield()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(executorInfo.TargetRetinueId);
                if (retinue != null && retinue.M_RetinueShield != 0)
                {
                    retinue.M_RetinueShield = retinue.M_RetinueShield / 2;
                }
            }
        }
    }
}
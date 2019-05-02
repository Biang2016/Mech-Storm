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
                foreach (int retinueId in executorInfo.TargetRetinueIds)
                {
                    ServerModuleRetinue retinue = player.MyGameManager.GetRetinueOnBattleGround(retinueId);
                    if (retinue != null && retinue.M_RetinueShield != 0)
                    {
                        retinue.M_RetinueShield = retinue.M_RetinueShield / 2;
                    }
                }
            }
        }
    }
}
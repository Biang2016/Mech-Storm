namespace SideEffects
{
    public class BreakEquip : BreakEquip_Base
    {
        public BreakEquip()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyGameManager.RemoveEquipByEquipID(executorInfo.TargetEquipIds[0]);
        }
    }
}
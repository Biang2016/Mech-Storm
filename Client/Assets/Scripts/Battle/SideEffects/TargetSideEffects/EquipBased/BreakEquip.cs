namespace SideEffects
{
    public class BreakEquip : BreakEquip_Base
    {
        public BreakEquip()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.RemoveEquipByEquipID(executorInfo.TargetEquipIds[0]);
        }
    }
}
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
            switch (TargetRange)
            {
                case TargetRange.Mechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetRange.SelfMechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetRange.Heroes:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetRange.EnemyHeroes:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetRange.Soldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
                case TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
                case TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
            }
        }
    }
}
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
            switch (M_TargetRange)
            {
                case TargetSideEffect.TargetRange.Mechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfMechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.Heroes:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfHeroes:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.Soldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executorInfo.TargetEquipId);
                    break;
            }
        }
    }
}
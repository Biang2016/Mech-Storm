namespace SideEffects
{
    public class BreakEquip : BreakEquip_Base
    {
        public BreakEquip()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetSideEffect.TargetRange.Mechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executerInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfMechs:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemyMechs:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.All, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.Heros:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executerInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfHeros:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemyHeros:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Hero, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.Soldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executerInfo.TargetEquipId);
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.SelfSoldiers:
                    player.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executerInfo.TargetEquipId);
                    break;
                case TargetSideEffect.TargetRange.EnemySoldiers:
                    player.MyEnemyPlayer.MyBattleGroundManager.RemoveEquip(ServerBattleGroundManager.RetinueType.Soldier, executerInfo.TargetEquipId);
                    break;
            }
        }
    }
}
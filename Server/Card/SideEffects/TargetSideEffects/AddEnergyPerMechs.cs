namespace SideEffects
{
    public class AddEnergyPerMechs : AddEnergyPerMechs_Base
    {
        public AddEnergyPerMechs()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = 0;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    value = FinalValue * (player.MyBattleGroundManager.RetinueCount + player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount);
                    break;
                case TargetRange.Heros:
                    value = FinalValue * (player.MyBattleGroundManager.HeroCount + player.MyEnemyPlayer.MyBattleGroundManager.HeroCount);
                    break;
                case TargetRange.Soldiers:
                    value = FinalValue * (player.MyBattleGroundManager.SoldierCount + player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount);
                    break;
                case TargetRange.SelfMechs:
                    value = FinalValue * player.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.SelfHeros:
                    value = FinalValue * player.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.SelfSoldiers:
                    value = FinalValue * player.MyBattleGroundManager.SoldierCount;
                    break;
                case TargetRange.EnemyMechs:
                    value = FinalValue * player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.EnemyHeros:
                    value = FinalValue * player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.EnemySoldiers:
                    value = FinalValue * player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    break;
            }

            player.AddEnergyWithinMax(value);
            player.MyEnemyPlayer.UseEnergyAboveZero(value);
        }
    }
}
namespace SideEffects
{
    public class SummonRetinuePerMechs : SummonRetinuePerMechs_Base
    {
        public SummonRetinuePerMechs()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = 0;
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    value = (player.MyBattleGroundManager.RetinueCount + player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount);
                    break;
                case TargetRange.Heroes:
                    value = (player.MyBattleGroundManager.HeroCount + player.MyEnemyPlayer.MyBattleGroundManager.HeroCount);
                    break;
                case TargetRange.Soldiers:
                    value = (player.MyBattleGroundManager.SoldierCount + player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount);
                    break;
                case TargetRange.SelfMechs:
                    value = player.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.SelfHeroes:
                    value = player.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.SelfSoldiers:
                    value = player.MyBattleGroundManager.SoldierCount;
                    break;
                case TargetRange.EnemyMechs:
                    value = player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.EnemyHeros:
                    value = player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.EnemySoldiers:
                    value = player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    break;
            }

            for (int i = 0; i < value; i++)
            {
                player.MyBattleGroundManager.AddRetinue((CardInfo_Retinue) AllCards.GetCard(RetinueCardId));
            }
        }
    }
}
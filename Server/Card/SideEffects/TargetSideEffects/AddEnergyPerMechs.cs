namespace SideEffects
{
    public class AddEnergyPerMechs : AddEnergyPerMechs_Base
    {
        public AddEnergyPerMechs()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int finalValue = 0;
            int energyValue = M_SideEffectParam.GetParam_MultipliedInt("EnergyValue");
            switch (M_TargetRange)
            {
                case TargetRange.Mechs:
                    finalValue = energyValue * (player.MyBattleGroundManager.RetinueCount + player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount);
                    break;
                case TargetRange.Heroes:
                    finalValue = energyValue * (player.MyBattleGroundManager.HeroCount + player.MyEnemyPlayer.MyBattleGroundManager.HeroCount);
                    break;
                case TargetRange.Soldiers:
                    finalValue = energyValue * (player.MyBattleGroundManager.SoldierCount + player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount);
                    break;
                case TargetRange.SelfMechs:
                    finalValue = energyValue * player.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.SelfHeroes:
                    finalValue = energyValue * player.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.SelfSoldiers:
                    finalValue = energyValue * player.MyBattleGroundManager.SoldierCount;
                    break;
                case TargetRange.EnemyMechs:
                    finalValue = energyValue * player.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                    break;
                case TargetRange.EnemyHeros:
                    finalValue = energyValue * player.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                    break;
                case TargetRange.EnemySoldiers:
                    finalValue = energyValue * player.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                    break;
            }

            player.AddEnergyWithinMax(finalValue);
            player.MyEnemyPlayer.UseEnergyAboveZero(finalValue);
        }
    }
}
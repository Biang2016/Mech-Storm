namespace SideEffects
{
    public class AddAttack : AddAttack_Base
    {
        public AddAttack()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("AttackValue");

            ServerBattleGroundManager.RetinueType retinueType = ServerBattleGroundManager.RetinueType.All;

            foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
            {
                serverPlayer.MyBattleGroundManager.ChangeRetinuesValue(
                    ServerBattleGroundManager.RetinueValueTypes.Attack, 
                    value, 
                    ChoiceCount,
                    executorInfo.TargetRetinueIds,
                    TargetSelect, 
                    retinueType,
                    -1);
            }
        }
    }
}
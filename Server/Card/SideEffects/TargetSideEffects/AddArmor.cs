namespace SideEffects
{
    public class AddArmor : AddArmor_Base
    {
        public AddArmor()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ArmorValue");

            ServerBattleGroundManager.RetinueType retinueType = ServerBattleGroundManager.GetRetinueTypeByTargetRange(TargetRange);

            foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
            {
                serverPlayer.MyBattleGroundManager.ChangeRetinuesValue(
                    ServerBattleGroundManager.RetinueValueTypes.Armor,
                    value,
                    0,
                    executorInfo.TargetRetinueIds,
                    TargetSelect,
                    retinueType,
                    -1
                );
            }
        }
    }
}
namespace SideEffects
{
    public class AddShield : AddShield_Base
    {
        public AddShield()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ShieldValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId).M_RetinueShield += value;
            }

            ServerBattleGroundManager.RetinueType retinueType = ServerBattleGroundManager.GetRetinueTypeByTargetRange(TargetRange);

            foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
            {
                serverPlayer.MyBattleGroundManager.ChangeRetinuesValue(ServerBattleGroundManager.RetinueValueTypes.Shield, value, ChoiceCount, executorInfo.TargetRetinueIds, TargetSelect, retinueType, -1);
            }
        }
    }
}
namespace SideEffects
{
    public class AddArmorOne : AddArmorOne_Base
    {
        public AddArmorOne()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ArmorValue");
            if ((M_TargetRange & TargetRange.Mechs) != 0)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.TargetRetinueId, value);
                player.MyEnemyPlayer.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.TargetRetinueId, value);
            }

            if (M_TargetRange == TargetRange.Self)
            {
                player.MyBattleGroundManager.AddArmorForOneRetinue(executorInfo.RetinueId, value);
            }
        }
    }
}
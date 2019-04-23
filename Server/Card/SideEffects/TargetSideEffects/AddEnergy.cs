namespace SideEffects
{
    public class AddEnergy : AddEnergy_Base
    {
        public AddEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
            {
                serverPlayer.MyGameManager.ChangePlayerValue(
                    ServerGameManager.PlayerValueType.Energy,
                    value, 
                    ChoiceCount, 
                    TargetSelect,
                    executorInfo.TargetClientIds);
            }
        }
    }
}
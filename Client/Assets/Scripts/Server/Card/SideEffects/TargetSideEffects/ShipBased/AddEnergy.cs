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
            player.MyGameManager.SideEffect_ShipAction(
                delegate(ServerPlayer sp) { sp.AddEnergy(value); },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }
    }
}
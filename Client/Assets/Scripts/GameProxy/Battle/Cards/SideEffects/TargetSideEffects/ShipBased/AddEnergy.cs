namespace SideEffects
{
    public class AddEnergy : AddEnergy_Base
    {
        public AddEnergy()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("Energy");
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp) { sp.AddEnergy(value); },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }
    }
}
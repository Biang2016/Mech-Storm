namespace SideEffects
{
    public class AddShield : AddShield_Base
    {
        public AddShield()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("ShieldValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.GameManager.GetMech(executorInfo.MechId).M_MechShield += value;
            }

            player.GameManager.SideEffect_MechAction(
                delegate(ModuleMech mech) { mech.M_MechShield += value; },
                player,
                0,
                executorInfo.TargetMechIds,
                TargetRange,
                TargetSelect,
                -1
            );
        }
    }
}
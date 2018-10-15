namespace SideEffects
{
    public class AddPlayerBuff : AddPlayerBuff_Base
    {
        public AddPlayerBuff()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            SideEffectBase se = AllSideEffects.SideEffectsNameDict[SideEffectName].Clone();
            SideEffectExecute see = new SideEffectExecute(se, TriggerTime, TriggerRange, TriggerDelayTimes, TriggerTimes, RemoveTriggerTime, RemoveTriggerRange, RemoveTriggerTimes);
            player.AddSideEffectBundleForPlayerBuff(see);
        }
    }
}
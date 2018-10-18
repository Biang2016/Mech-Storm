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
            foreach (SideEffectBase sub_SE in Sub_SideEffect)
            {
                sub_SE.Player = player;
                sub_SE.M_ExecuterInfo = new ExecuterInfo(executerInfo.ClientId, isPlayerBuff: true);
                SideEffectExecute see = new SideEffectExecute(sub_SE, TriggerTime, TriggerRange, TriggerDelayTimes, TriggerTimes, RemoveTriggerTime, RemoveTriggerRange, RemoveTriggerTimes);
                player.AddSideEffectBundleForPlayerBuff(see, BuffPicId, HasNumberShow, CanPiled, Singleton);
            }
        }
    }
}
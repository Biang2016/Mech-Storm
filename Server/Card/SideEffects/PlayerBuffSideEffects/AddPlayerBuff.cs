namespace SideEffects
{
    public class AddPlayerBuff : AddPlayerBuff_Base
    {
        public AddPlayerBuff()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            AttachedBuffSEE.SideEffectBase.Player = player;
            AttachedBuffSEE.SideEffectBase.M_ExecutorInfo = new ExecutorInfo(clientId: player.ClientId, sideEffectExecutorID: AttachedBuffSEE.ID, isPlayerBuff: true);
            player.UpdatePlayerBuff(AttachedBuffSEE, true);
        }
    }
}
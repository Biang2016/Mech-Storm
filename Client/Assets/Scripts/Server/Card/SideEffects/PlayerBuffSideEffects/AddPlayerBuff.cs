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

            foreach (SideEffectBase se in AttachedBuffSEE.SideEffectBases)
            {
                se.Player = player;
                se.M_ExecutorInfo = new ExecutorInfo(clientId: player.ClientId, sideEffectExecutorID: AttachedBuffSEE.ID, isPlayerBuff: true);
            }

            player.UpdatePlayerBuff(AttachedBuffSEE, true);
        }
    }
}
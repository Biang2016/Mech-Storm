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

            AttachedBuffSEE.SideEffectBase.Player = player;
            AttachedBuffSEE.SideEffectBase.M_ExecuterInfo = new ExecuterInfo(clientId: player.ClientId, sideEffectExecutorID: AttachedBuffSEE.ID, isPlayerBuff: true);
            player.UpdatePlayerBuff(AttachedBuffSEE, (PlayerBuffSideEffects) AttachedBuffSEE.SideEffectBase, true);
        }
    }
}
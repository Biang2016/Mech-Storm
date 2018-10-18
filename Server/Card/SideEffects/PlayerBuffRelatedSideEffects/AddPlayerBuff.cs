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

            SideEffectExecute see = AllBuffs.GetBuff(BuffName).Clone();

            see.SideEffectBase.Player = player;
            see.SideEffectBase.M_ExecuterInfo.ClientId = player.ClientId;
            player.UpdatePlayerBuff(see, (PlayerBuffSideEffects) see.SideEffectBase, true);
        }
    }
}
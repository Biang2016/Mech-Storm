using System;

namespace SideEffects
{
    public class DrawCards : DrawCards_Base
    {
        public DrawCards()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.DrawCards(FinalValue);
        }
    }
}
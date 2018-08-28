using System;

namespace SideEffects
{
    public class CopyNextDrawCard : CopyNextDrawCard_Base
    {
        public CopyNextDrawCard()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer player = (ServerPlayer) Player;
            CardInfo_Base ci = player.MyCardDeckManager.M_CurrentCardDeck.GetFirstCardInfo();
            player.MyHandManager.DrawCard();
            for (int i = 0; i < Value; i++)
            {
                player.MyHandManager.GetACardByID(ci.CardID);
            }
        }
    }
}
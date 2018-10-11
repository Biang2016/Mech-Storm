namespace SideEffects
{
    public class CopyNextDrawCard : CopyNextDrawCard_Base
    {
        public CopyNextDrawCard()
        {
        }

        public override void Excute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            CardInfo_Base ci = player.MyCardDeckManager.CardDeck.GetFirstCardInfo();
            player.MyHandManager.DrawCards(1);
            for (int i = 0; i < FinalValue; i++)
            {
                player.MyHandManager.GetACardByID(ci.CardID);
            }
        }
    }
}
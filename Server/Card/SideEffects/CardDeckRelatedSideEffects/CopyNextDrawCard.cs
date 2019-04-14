namespace SideEffects
{
    public class CopyNextDrawCard : CopyNextDrawCard_Base
    {
        public CopyNextDrawCard()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            CardInfo_Base ci = player.MyCardDeckManager.CardDeck.GetFirstCardInfo();
            if (ci != null)
            {
                player.MyHandManager.DrawCards(1);
                for (int i = 0; i < M_SideEffectParam.GetParam_MultipliedInt("CardCount"); i++)
                {
                    player.MyHandManager.GetACardByID(ci.CardID);
                }
            }
        }
    }
}
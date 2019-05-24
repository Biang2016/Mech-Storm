namespace SideEffects
{
    public class CopyNextDrawCard : CopyNextDrawCard_Base
    {
        public CopyNextDrawCard()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            CardInfo_Base ci = player.CardDeckManager.CardDeck.GetFirstCardInfo();
            if (ci != null)
            {
                player.HandManager.DrawCards(1);
                for (int i = 0; i < M_SideEffectParam.GetParam_MultipliedInt("CardCount"); i++)
                {
                    player.HandManager.GetACardByID(ci.CardID);
                }
            }
        }
    }
}
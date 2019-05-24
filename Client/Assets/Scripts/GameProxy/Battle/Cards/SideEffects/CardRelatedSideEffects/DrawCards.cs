namespace SideEffects
{
    public class DrawCards : DrawCards_Base
    {
        public DrawCards()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.HandManager.DrawCards(M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}
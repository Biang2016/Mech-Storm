namespace SideEffects
{
    public class GetHandCardCopy : HandCardRelatedSideEffect
    {
        public GetHandCardCopy()
        {
        }

        protected override void InitSideEffectParam()
        {
        }

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            int cardId = player.HandManager.GetRandomHandCardId();
            player.HandManager.GetATempCardByID(cardId);
        }
    }
}
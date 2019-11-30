namespace SideEffects
{
    public class Exile : Exile_Base
    {
        public Exile()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
        }

        public override string GenerateDesc()
        {
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()]);
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;
            CardBase card = player.HandManager.GetCardByCardInstanceId(executorInfo.CardInstanceId);
            if (card != null)
            {
                card.CardInfo.BaseInfo.IsTemp = true;
            }
            return true;
        }
    }
}
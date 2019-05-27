namespace SideEffects
{
    public class SummonMech : TargetSideEffect
    {
        public SummonMech()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("SummonCardID", 0, typeof(CardDeck));
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.ShipBased;

        public override string GenerateDesc()
        {
            BaseInfo bi = AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID")).BaseInfo;
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer)Player;
            player.GameManager.SideEffect_ShipAction(
                delegate (BattlePlayer sp) { sp.BattleGroundManager.AddMech((CardInfo_Mech)AllCards.GetCard(M_SideEffectParam.GetParam_ConstInt("SummonCardID"))); },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }
    }
}
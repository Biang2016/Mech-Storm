using System.Collections.Generic;

namespace SideEffects
{
    public class GetHandCardCopy : HandCardRelatedSideEffect
    {
        public GetHandCardCopy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_MultipliedInt("CardCount", 0);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount") <= 1 ? "" : "s");
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp)
                {
                    if (executorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE)
                    {
                        int cardId = player.HandManager.GetRandomHandCardId(new HashSet<int> {executorInfo.CardInstanceId});
                        if (cardId != -1)
                        {
                            for (int i = 0; i < M_SideEffectParam.GetParam_MultipliedInt("CardCount"); i++)
                            {
                                player.HandManager.GetATempCardByID(cardId);
                            }
                        }
                    }
                },
                player,
                ChoiceCount,
                TargetRange,
                TargetSelect,
                executorInfo.TargetClientIds);
        }
    }
}
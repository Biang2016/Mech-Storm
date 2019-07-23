using System.Collections.Generic;

namespace SideEffects
{
    public class GetHandCardCopy : HandCardRelatedSideEffect, IPriorUsed, IPositive
    {
        public GetHandCardCopy()
        {
        }

        protected override void InitSideEffectParam()
        {
            base.InitSideEffectParam();
            M_SideEffectParam.SetParam_ConstInt("CardCount", 1);
            M_SideEffectParam.SetParam_MultipliedInt("CopyCount", 1);
        }

        public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.DeckBased;

        public override string GenerateDesc()
        {
            return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_ConstInt("CardCount"),
                M_SideEffectParam.GetParam_MultipliedInt("CopyCount"));
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;
            player.GameManager.SideEffect_ShipAction(
                delegate(BattlePlayer sp)
                {
                    int cardCount = M_SideEffectParam.GetParam_ConstInt("CardCount");
                    int copyCount = M_SideEffectParam.GetParam_MultipliedInt("CopyCount");

                    if (executorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE)
                    {
                        List<int> cardIds = player.HandManager.GetRandomHandCardIds(cardCount, new HashSet<int> {executorInfo.CardInstanceId});

                        foreach (int cardId in cardIds)
                        {
                            player.HandManager.GetTempCardsByID(cardId, copyCount);
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
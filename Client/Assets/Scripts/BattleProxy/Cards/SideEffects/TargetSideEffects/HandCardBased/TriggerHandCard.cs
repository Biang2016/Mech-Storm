using System.Collections.Generic;

namespace SideEffects
{
    public class TriggerHandCard : HandCardRelatedSideEffect, IPriorUsed, IPositive
    {
        public TriggerHandCard()
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
            return base.GenerateDesc() + HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()],
                GetDescOfTargetRange(),
                M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;

            CheckValidFunction = delegate(CardInfo_Base ci)
            {
                if (ci.BaseInfo.CardType == CardTypes.Spell)
                {
                    foreach (SideEffectExecute see in ci.SideEffectBundle.SideEffectExecutes)
                    {
                        foreach (SideEffectBase sb in see.SideEffectBases)
                        {
                            if (sb is TriggerHandCard)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            };

            List<int> cardInstanceIds = player.HandManager.GetRandomHandCardInstanceIds(M_SideEffectParam.GetParam_MultipliedInt("CardCount"), executorInfo.CardInstanceId,CheckValidFunction);
            foreach (int cardInstanceId in cardInstanceIds)
            {
                player.HandManager.UseCard(cardInstanceId, onlyTriggerNotUse: true);
            }
            return true;
        }

        public int GetSideEffectFunctionBias()
        {
            return M_SideEffectParam.GetParam_MultipliedInt("CardCount") * 5;
        }

        public delegate bool CheckValid(CardInfo_Base cb);

        public CheckValid CheckValidFunction;
    }
}
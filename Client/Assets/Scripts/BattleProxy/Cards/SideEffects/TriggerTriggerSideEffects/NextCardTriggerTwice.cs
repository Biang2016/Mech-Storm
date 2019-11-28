using System;

namespace SideEffects
{
    public class NextCardTriggerTwice : TriggerTriggerSideEffects, IPriorUsed
    {
        public NextCardTriggerTwice()
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
            foreach (SideEffectBase se in PeekSEE.SideEffectBases)
            {
                try
                {
                    se.Execute(executorInfo);
                }
                catch (Exception e)
                {
                }
            }
        }

        public override bool IsTrigger(ExecutorInfo ei)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if (ei.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE)
            {
                CardBase card = player.HandManager.GetCardByCardInstanceId(ei.CardInstanceId);
                if (card != null)
                {
                    return card.CardInfo.BaseInfo.CardType == CardTypes.Spell || card.CardInfo.BaseInfo.CardType == CardTypes.Energy;
                }
            }

            return false;
        }

        public int GetSideEffectFunctionBias()
        {
            return 5;
        }
    }
}
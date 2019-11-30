namespace SideEffects
{
    public class SpellDamageDouble : TriggerTriggerSideEffects, IPriorUsed
    {
        public SpellDamageDouble()
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
            foreach (SideEffectBase se in PeekSEE.SideEffectBases)
            {
                if (se is IDamage)
                {
                    se.M_SideEffectParam.Factor *= 2;
                }
            }
            return true;
        }

        public override bool IsTrigger(ExecutorInfo ei)
        {
            BattlePlayer player = (BattlePlayer) Player;
            if (ei.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE)
            {
                CardBase card = player.HandManager.GetCardByCardInstanceId(ei.CardInstanceId);
                if (card != null)
                {
                    if (card.CardInfo.BaseInfo.CardType != CardTypes.Spell && card.CardInfo.BaseInfo.CardType != CardTypes.Energy)
                    {
                        return false;
                    }
                }
            }

            foreach (SideEffectBase se in PeekSEE.SideEffectBases)
            {
                if (se is IDamage)
                {
                    if (se.M_SideEffectParam.HasParamCanBeMultiplied())
                    {
                        return true;
                    }
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
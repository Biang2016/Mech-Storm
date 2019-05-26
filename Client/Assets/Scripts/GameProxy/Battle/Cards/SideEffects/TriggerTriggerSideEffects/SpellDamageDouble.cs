﻿namespace SideEffects
{
    public class SpellDamageDouble : TriggerTriggerSideEffects
    {
        public SpellDamageDouble()
        {
        }

        protected override void InitSideEffectParam()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            BattlePlayer player = (BattlePlayer) Player;

            foreach (SideEffectBase se in PeekSEE.SideEffectBases)
            {
                if (se is IDamage)
                {
                    se.M_SideEffectParam.Factor *= 2;
                }
            }
        }

        public override bool IsTrigger()
        {
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
    }
}
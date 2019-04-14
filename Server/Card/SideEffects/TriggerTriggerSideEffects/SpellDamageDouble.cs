namespace SideEffects
{
    public class SpellDamageDouble : SpellDamageDouble_Base
    {
        public SpellDamageDouble()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if (PeekSEE.SideEffectBase is IDamage)
            {
                PeekSEE.SideEffectBase.M_SideEffectParam.Factor *= 2;
            }
        }

        public override bool IsTrigger()
        {
            if (PeekSEE.SideEffectBase is IDamage)
            {
                if (PeekSEE.SideEffectBase.M_SideEffectParam.HasParamCanBeMultiplied())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
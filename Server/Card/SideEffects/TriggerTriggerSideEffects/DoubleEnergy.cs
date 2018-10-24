namespace SideEffects
{
    public class SpellDamageDouble : SpellDamageDouble_Base
    {
        public SpellDamageDouble()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;

            if (PeekSEE.SideEffectBase is IDamage damage_se)
            {
                if (damage_se is IEffectFactor ef_se)
                {
                    ef_se.SetFactor(ef_se.GetFactor() * 2);
                }
            }
        }

        public override bool IsTrigger()
        {
            if (PeekSEE.SideEffectBase is IDamage damage_se)
            {
                if (damage_se is IEffectFactor)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
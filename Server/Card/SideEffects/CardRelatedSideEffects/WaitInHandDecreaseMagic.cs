using System;

namespace SideEffects
{
    public class WaitInHandDecreaseMagic : WaitInHandDecreaseMagic_Base
    {
        public WaitInHandDecreaseMagic()
        {
        }

        public override void Excute(object Player)
        {
            ServerPlayer sp = (ServerPlayer) Player;
            ServerCardBase ci = sp.MyHandManager.GetCardByCardInstanceId(CardInstanceID);

            if (ci.CardInfo.BaseInfo.Magic > 0)
            {
                ci.M_Magic--;
            }
        }
    }
}
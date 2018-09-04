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
            ServerCardBase ci = sp.MyHandManager.GetCardByCardInstanceId(TargetCardInstanceId);

            if (ci.CardInfo.BaseInfo.Magic >= FinalValue)
            {
                ci.M_Magic-= FinalValue;
            }
        }
    }
}
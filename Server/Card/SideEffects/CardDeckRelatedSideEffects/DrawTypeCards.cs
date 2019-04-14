﻿namespace SideEffects
{
    public class DrawTypeCards : DrawTypeCards_Base
    {
        public DrawTypeCards()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            player.MyHandManager.DrawCardsByType((CardTypes) M_SideEffectParam.GetHashCode(), M_SideEffectParam.GetParam_MultipliedInt("CardCount"));
        }
    }
}
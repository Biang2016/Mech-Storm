﻿using System;
using System.Collections.Generic;

public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, SlotTypes slotType, UpgradeInfo upgradeInfo, SortedDictionary<SideEffectBase.TriggerTime, List<SideEffectBase>> sideEffects)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            slotType: slotType,
            sideEffects: sideEffects)
    {
        UpgradeInfo = upgradeInfo;
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        CardDescShow += base.GetCardDescShow();

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Spell cs = new CardInfo_Spell(
            cardID: CardID,
            baseInfo: BaseInfo,
            slotType: M_SlotType,
            upgradeInfo: UpgradeInfo,
            sideEffects:temp.SideEffects);
        return cs;
    }
}
using System;
using System.Collections.Generic;

public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, List<SideEffectBase> sideEffects_OnEndRound, List<SideEffectBase> sideEffects_OnPlayOut, List<SideEffectBase> sideEffects_OnSummoned,  List<SideEffectBase> sideEffects_OnDie)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects_OnEndRound: sideEffects_OnEndRound,
            sideEffects_OnPlayOut: sideEffects_OnPlayOut,
            sideEffects_OnSummoned: sideEffects_OnSummoned,
            sideEffects_OnDie: sideEffects_OnDie)
    {
        UpgradeInfo = upgradeInfo;
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (BaseInfo.Magic > 0) CardDescShow += SideEffectBase.HightlightStringFormat(BaseInfo.HightLightColor, "使用{0}点能量,", BaseInfo.Magic);

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
            upgradeInfo: UpgradeInfo,
            sideEffects_OnEndRound: temp.SideEffects_OnEndRound,
            sideEffects_OnPlayOut: temp.SideEffects_OnPlayOut,
            sideEffects_OnSummoned: temp.SideEffects_OnSummoned,
            sideEffects_OnDie: temp.SideEffects_OnDie);
        return cs;
    }
}
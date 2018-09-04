using System;
using System.Collections.Generic;

public class CardInfo_Shield : CardInfo_Base
{
    public CardInfo_Shield()
    {
    }

    public CardInfo_Shield(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, ShieldInfo shieldInfo, List<SideEffectBase> sideEffects_OnEndRound, List<SideEffectBase> sideEffects_OnPlayOut, List<SideEffectBase> sideEffects_OnSummoned, List<SideEffectBase> sideEffects_OnDie)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects_OnEndRound: sideEffects_OnEndRound,
            sideEffects_OnPlayOut: sideEffects_OnPlayOut,
            sideEffects_OnSummoned: sideEffects_OnSummoned,
            sideEffects_OnDie: sideEffects_OnDie)
    {
        UpgradeInfo = upgradeInfo;
        ShieldInfo = shieldInfo;
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (ShieldInfo.ShieldType == ShieldTypes.Armor)
        {
            CardDescShow += "阻挡 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Armor.ToString()) + " 点伤害\n";
        }
        else if (ShieldInfo.ShieldType == ShieldTypes.Shield)
        {
            CardDescShow += "受到的伤害减少 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Shield.ToString()) + " 点\n";
        }

        CardDescShow += base.GetCardDescShow();

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Shield cb = new CardInfo_Shield(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            shieldInfo: ShieldInfo,
            sideEffects_OnEndRound: temp.SideEffects_OnEndRound,
            sideEffects_OnPlayOut: temp.SideEffects_OnPlayOut,
            sideEffects_OnSummoned: temp.SideEffects_OnSummoned,
            sideEffects_OnDie: temp.SideEffects_OnDie);
        return cb;
    }
}
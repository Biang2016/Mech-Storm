using System;
using System.Collections.Generic;

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo, List<SideEffectBase> sideEffects_OnEndRound, List<SideEffectBase> sideEffects_OnPlayOut, List<SideEffectBase> sideEffects_OnSummoned, List<SideEffectBase> sideEffects_OnDie)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects_OnEndRound: sideEffects_OnEndRound,
            sideEffects_OnPlayOut: sideEffects_OnPlayOut,
            sideEffects_OnSummoned: sideEffects_OnSummoned,
            sideEffects_OnDie: sideEffects_OnDie)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (BattleInfo.BasicAttack != 0) CardDescShow += "攻击力 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicAttack) + "\n";
        if (BattleInfo.BasicArmor != 0) CardDescShow += "护甲 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicArmor) + "\n";
        if (BattleInfo.BasicShield != 0) CardDescShow += "护盾 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicShield) + "\n";

        CardDescShow += base.GetCardDescShow();

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Retinue cb = new CardInfo_Retinue(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            lifeInfo: LifeInfo,
            battleInfo: BattleInfo,
            slotInfo: SlotInfo,
            sideEffects_OnEndRound: temp.SideEffects_OnEndRound,
            sideEffects_OnPlayOut: temp.SideEffects_OnPlayOut,
            sideEffects_OnSummoned: temp.SideEffects_OnSummoned,
            sideEffects_OnDie: temp.SideEffects_OnDie);
        return cb;
    }
}
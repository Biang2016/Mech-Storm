using System;
using System.Collections.Generic;

public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, SlotTypes slotType, UpgradeInfo upgradeInfo, SideEffectBundle sideEffects, SideEffectBundle sideEffects_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            slotType: slotType,
            sideEffects: sideEffects,
            sideEffects_OnBattleGround: sideEffects_OnBattleGround)
    {
        UpgradeInfo = upgradeInfo;
    }

    public override string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        CardDescShow += base.GetCardDescShow(isEnglish);

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
            sideEffects: temp.SideEffects.Clone(),
            sideEffects_OnBattleGround: SideEffects_OnBattleGround.Clone());
        return cs;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        if (BaseInfo.CardType == CardTypes.Spell) return isEnglish ? "Spell" : "法术牌";
        else if (BaseInfo.CardType == CardTypes.Energy) return isEnglish ? "Energy" : "能量牌";
        return null;
    }
}
using System;
using System.Collections.Generic;

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, SlotTypes slotType, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo, SideEffectBundle sideEffects)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            slotType: slotType,
            sideEffects: sideEffects)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
    }

    public override string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (BattleInfo.BasicAttack != 0) CardDescShow += (isEnglish ? "Attack " : "攻击力 ") + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicAttack) + "\n";
        if (BattleInfo.BasicArmor != 0) CardDescShow += (isEnglish ? "Armor " : "护甲 ") + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicArmor) + "\n";
        if (BattleInfo.BasicShield != 0) CardDescShow += (isEnglish ? "Shield " : "护盾 ") + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicShield) + "\n";

        CardDescShow += base.GetCardDescShow(isEnglish);

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Retinue cb = new CardInfo_Retinue(
            cardID: CardID,
            baseInfo: BaseInfo,
            slotType: M_SlotType,
            upgradeInfo: UpgradeInfo,
            lifeInfo: LifeInfo,
            battleInfo: BattleInfo,
            slotInfo: SlotInfo,
            sideEffects: temp.SideEffects);
        return cb;
    }
}
public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, RetinueInfo retinueInfo, SideEffectBundle sideEffects, SideEffectBundle sideEffects_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects: sideEffects,
            sideEffects_OnBattleGround: sideEffects_OnBattleGround)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        RetinueInfo = retinueInfo;
    }

    public override string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (RetinueInfo.IsDefence) CardDescShow += BaseInfo.AddImportantColorToText((isEnglish ? "Defence. " : "嘲讽; "));
        if (RetinueInfo.IsSniper) CardDescShow += BaseInfo.AddImportantColorToText((isEnglish ? "Sniper. " : "狙击; "));
        if (BattleInfo.BasicAttack != 0) CardDescShow += (isEnglish ? "Attack " : "攻击力 ") + BaseInfo.AddHightLightColorToText("+" + BattleInfo.BasicAttack) + ", ";
        if (BattleInfo.BasicArmor != 0) CardDescShow += (isEnglish ? "Armor " : "护甲 ") + BaseInfo.AddHightLightColorToText("+" + BattleInfo.BasicArmor) + ", ";
        if (BattleInfo.BasicShield != 0) CardDescShow += (isEnglish ? "Shield " : "护盾 ") + BaseInfo.AddHightLightColorToText("+" + BattleInfo.BasicShield) + ", ";

        CardDescShow += base.GetCardDescShow(isEnglish);

        CardDescShow = CardDescShow.TrimEnd().TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override string GetCardColor()
    {
        if (RetinueInfo.IsSoldier) return GamePlaySettings.SoldierCardColor;
        else return GamePlaySettings.HeroCardColor;
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
            retinueInfo: RetinueInfo,
            sideEffects: temp.SideEffects.Clone(),
            sideEffects_OnBattleGround: SideEffects_OnBattleGround.Clone());
        return cb;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        if (RetinueInfo.IsSoldier) return isEnglish ? "Soldier" : "士兵";
        else return isEnglish ? "Hero" : "英雄";
    }
}
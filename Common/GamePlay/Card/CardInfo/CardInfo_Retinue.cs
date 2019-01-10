public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, RetinueInfo retinueInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle,
            sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        RetinueInfo = retinueInfo;
        Pro_Initialize();
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

        CardDescShow = CardDescShow.TrimEnd().TrimEnd(",. ;\n".ToCharArray());

        return CardDescShow;
    }

    public override string GetCardColor()
    {
        if (RetinueInfo.IsSoldier) return AllColors.ColorDict[AllColors.ColorType.SoldierCardColor];
        else return AllColors.ColorDict[AllColors.ColorType.HeroCardColor];
    }

    public override float GetCardColorIntensity()
    {
        if (RetinueInfo.IsSoldier) return AllColors.IntensityDict[AllColors.ColorType.SoldierCardColor];
        else return AllColors.IntensityDict[AllColors.ColorType.HeroCardColor];
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
            sideEffectBundle: temp.SideEffectBundle.Clone(),
            sideEffectBundle_OnBattleGround: SideEffectBundle_OnBattleGround.Clone());
        return cb;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        if (RetinueInfo.IsSoldier) return isEnglish ? "Soldier" : "士兵";
        else return isEnglish ? "Hero" : "英雄";
    }
}
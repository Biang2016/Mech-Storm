public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, RetinueInfo retinueInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
        : base(cardID: cardID,
            upgradeInfo: upgradeInfo,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle,
            sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround)
    {
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        RetinueInfo = retinueInfo;
        Pro_Initialize();
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = "";
        if (RetinueInfo.IsDefence) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Defense") + ". ");
        if (RetinueInfo.IsSniper) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Sniper") + ". ");
        if (BattleInfo.BasicAttack != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_AttackValue")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicAttack) + ", ";
        if (BattleInfo.BasicArmor != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Armor")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicArmor) + ", ";
        if (BattleInfo.BasicShield != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Shield")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicShield) + ", ";

        CardDescShow += base.GetCardDescShow();

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

    public override string GetCardTypeDesc()
    {
        if (RetinueInfo.IsSoldier) return LanguageManager_Common.GetText("KeyWords_CardRetinue_Soldier");
        else return LanguageManager_Common.GetText("KeyWords_CardRetinue_Hero");
    }
}
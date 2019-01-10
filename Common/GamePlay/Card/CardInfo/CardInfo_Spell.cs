public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle,
            sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround)
    {
        UpgradeInfo = upgradeInfo;
        Pro_Initialize();
    }

    public override string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        CardDescShow += base.GetCardDescShow(isEnglish);

        CardDescShow = CardDescShow.TrimEnd(",. ;\n".ToCharArray());

        return CardDescShow;
    }

    public override string GetCardColor()
    {
        if (BaseInfo.CardType == CardTypes.Energy) return AllColors.ColorDict[AllColors.ColorType.EnergyCardColor];
        else return AllColors.ColorDict[AllColors.ColorType.SpellCardColor];
    }

    public override float GetCardColorIntensity()
    {
        if (BaseInfo.CardType == CardTypes.Energy) return AllColors.IntensityDict[AllColors.ColorType.EnergyCardColor];
        else return AllColors.IntensityDict[AllColors.ColorType.SpellCardColor];
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Spell cs = new CardInfo_Spell(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            sideEffectBundle: temp.SideEffectBundle.Clone(),
            sideEffectBundle_OnBattleGround: SideEffectBundle_OnBattleGround.Clone());
        return cs;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        string cardTypeName = "";
        cardTypeName = isEnglish ? BaseInfo.CardTypeNameDict_en[BaseInfo.CardType] : BaseInfo.CardTypeNameDict[BaseInfo.CardType];
        return cardTypeName;
    }
}
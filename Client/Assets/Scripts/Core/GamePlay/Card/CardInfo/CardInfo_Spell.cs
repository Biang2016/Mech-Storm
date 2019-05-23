using System.Collections.Generic;
using System.Xml;

public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, SideEffectBundle sideEffectBundle)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            upgradeInfo: upgradeInfo,
            sideEffectBundle: sideEffectBundle)
    {
        Pro_Initialize();
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = "";
        CardDescShow += base.GetCardDescShow();
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
        CardInfo_Spell cs = new CardInfo_Spell(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            sideEffectBundle: SideEffectBundle.Clone());
        return cs;
    }

    protected override void ChildrenExportToXML(XmlElement card_ele)
    {
        base.ChildrenExportToXML(card_ele);
        XmlDocument doc = card_ele.OwnerDocument;
    }

    public override string GetCardTypeDesc()
    {
        string cardTypeName = "";
        cardTypeName = BaseInfo.CardTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][BaseInfo.CardType];
        return cardTypeName;
    }
}
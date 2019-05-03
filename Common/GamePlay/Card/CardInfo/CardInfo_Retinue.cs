using System.Collections.Generic;
using System.Xml;

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, RetinueInfo retinueInfo, SideEffectBundle sideEffectBundle)
        : base(cardID: cardID,
            upgradeInfo: upgradeInfo,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle)
    {
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        RetinueInfo = retinueInfo;
        Pro_Initialize();
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = "";
        if (RetinueInfo.IsDefense) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Defense") + ". ");
        if (RetinueInfo.IsSniper) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Sniper") + ". ");
        if (RetinueInfo.IsCharger) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Charger") + ". ");
        if (RetinueInfo.IsFrenzy) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Frenzy") + ". ");
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
        CardInfo_Retinue cb = new CardInfo_Retinue(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            lifeInfo: LifeInfo,
            battleInfo: BattleInfo,
            retinueInfo: RetinueInfo,
            sideEffectBundle: SideEffectBundle.Clone());
        return cb;
    }

    protected override void ChildrenExportToXML(XmlElement card_ele)
    {
        base.ChildrenExportToXML(card_ele);
        XmlDocument doc = card_ele.OwnerDocument;
        XmlElement retinueInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(retinueInfo_ele);
        retinueInfo_ele.SetAttribute("name", "retinueInfo");
        retinueInfo_ele.SetAttribute("isSoldier", RetinueInfo.IsSoldier.ToString());
        retinueInfo_ele.SetAttribute("isDefense", RetinueInfo.IsDefense.ToString());
        retinueInfo_ele.SetAttribute("isSniper", RetinueInfo.IsSniper.ToString());
        retinueInfo_ele.SetAttribute("isCharger", RetinueInfo.IsCharger.ToString());
        retinueInfo_ele.SetAttribute("isFrenzy", RetinueInfo.IsFrenzy.ToString());
        retinueInfo_ele.SetAttribute("slot1", RetinueInfo.Slots[0].ToString());
        retinueInfo_ele.SetAttribute("slot2", RetinueInfo.Slots[1].ToString());
        retinueInfo_ele.SetAttribute("slot3", RetinueInfo.Slots[2].ToString());
        retinueInfo_ele.SetAttribute("slot4", RetinueInfo.Slots[3].ToString());

        XmlElement lifeInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(lifeInfo_ele);
        lifeInfo_ele.SetAttribute("name", "lifeInfo");
        lifeInfo_ele.SetAttribute("life", LifeInfo.Life.ToString());
        lifeInfo_ele.SetAttribute("totalLife", LifeInfo.TotalLife.ToString());

        XmlElement battleInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(battleInfo_ele);
        battleInfo_ele.SetAttribute("name", "battleInfo");
        battleInfo_ele.SetAttribute("basicAttack", BattleInfo.BasicAttack.ToString());
        battleInfo_ele.SetAttribute("basicShield", BattleInfo.BasicShield.ToString());
        battleInfo_ele.SetAttribute("basicArmor", BattleInfo.BasicArmor.ToString());
    }

    public override string GetCardTypeDesc()
    {
        if (RetinueInfo.IsSoldier) return LanguageManager_Common.GetText("KeyWords_CardRetinue_Soldier");
        else return LanguageManager_Common.GetText("KeyWords_CardRetinue_Hero");
    }
}
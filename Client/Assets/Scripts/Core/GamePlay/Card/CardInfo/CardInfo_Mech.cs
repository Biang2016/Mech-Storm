using System.Xml;

public class CardInfo_Mech : CardInfo_Base
{
    public CardInfo_Mech()
    {
    }

    public CardInfo_Mech(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, MechInfo mechInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_BattleGroundAura)
        : base(cardID: cardID,
            upgradeInfo: upgradeInfo,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle,
            sideEffectBundle_BattleGroundAura: sideEffectBundle_BattleGroundAura)
    {
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        MechInfo = mechInfo;
        Pro_Initialize();
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = "";
        if (MechInfo.IsDefense) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Defense") + ". ");
        if (MechInfo.IsSniper) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Sniper") + ". ");
        if (MechInfo.IsCharger) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Charger") + ". ");
        if (MechInfo.IsFrenzy) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Frenzy") + ". ");
        if (MechInfo.IsSentry) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Sentry") + ". ");
        if (BattleInfo.BasicAttack != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_AttackValue")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicAttack) + ". ";
        if (BattleInfo.BasicArmor != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Armor")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicArmor) + ". ";
        if (BattleInfo.BasicShield != 0) CardDescShow += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("KeyWords_Shield")) + " " + BaseInfo.AddHighLightColorToText("+" + BattleInfo.BasicShield) + ". ";

        CardDescShow += base.GetCardDescShow();

        CardDescShow = CardDescShow.TrimEnd().TrimEnd(",. ;\n".ToCharArray());

        return CardDescShow;
    }

    public override string GetCardColor()
    {
        if (MechInfo.IsSoldier) return AllColors.ColorDict[AllColors.ColorType.SoldierCardColor];
        else return AllColors.ColorDict[AllColors.ColorType.HeroCardColor];
    }

    public override float GetCardColorIntensity()
    {
        if (MechInfo.IsSoldier) return AllColors.IntensityDict[AllColors.ColorType.SoldierCardColor];
        else return AllColors.IntensityDict[AllColors.ColorType.HeroCardColor];
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Mech cb = new CardInfo_Mech(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            lifeInfo: LifeInfo,
            battleInfo: BattleInfo,
            mechInfo: MechInfo,
            sideEffectBundle: SideEffectBundle.Clone(),
            sideEffectBundle_BattleGroundAura: SideEffectBundle_BattleGroundAura.Clone());
        return cb;
    }

    protected override void ChildrenExportToXML(XmlElement card_ele)
    {
        base.ChildrenExportToXML(card_ele);
        XmlDocument doc = card_ele.OwnerDocument;
        XmlElement mechInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(mechInfo_ele);
        mechInfo_ele.SetAttribute("name", "mechInfo");
        mechInfo_ele.SetAttribute("isSoldier", MechInfo.IsSoldier.ToString());
        mechInfo_ele.SetAttribute("isDefense", MechInfo.IsDefense.ToString());
        mechInfo_ele.SetAttribute("isSniper", MechInfo.IsSniper.ToString());
        mechInfo_ele.SetAttribute("isCharger", MechInfo.IsCharger.ToString());
        mechInfo_ele.SetAttribute("isFrenzy", MechInfo.IsFrenzy.ToString());
        mechInfo_ele.SetAttribute("isSentry", MechInfo.IsSentry.ToString());
        mechInfo_ele.SetAttribute("slot1", MechInfo.Slots[0].ToString());
        mechInfo_ele.SetAttribute("slot2", MechInfo.Slots[1].ToString());
        mechInfo_ele.SetAttribute("slot3", MechInfo.Slots[2].ToString());
        mechInfo_ele.SetAttribute("slot4", MechInfo.Slots[3].ToString());

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
        if (MechInfo.IsSoldier) return LanguageManager_Common.GetText("KeyWords_CardMech_Soldier");
        else return LanguageManager_Common.GetText("KeyWords_CardMech_Hero");
    }
}
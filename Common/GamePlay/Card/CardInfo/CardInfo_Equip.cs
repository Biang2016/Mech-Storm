public class CardInfo_Equip : CardInfo_Base
{
    public CardInfo_Equip()
    {
    }

    public CardInfo_Equip(int cardID,  BaseInfo baseInfo, UpgradeInfo upgradeInfo, EquipInfo equipInfo, WeaponInfo weaponInfo, ShieldInfo shieldInfo, PackInfo packInfo, MAInfo maInfo, SideEffectBundle sideEffectBundle, SideEffectBundle sideEffectBundle_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffectBundle: sideEffectBundle,
            sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround)
    {
        switch (equipInfo.SlotType)
        {
            case SlotTypes.Weapon:
            {
                WeaponInfo = weaponInfo;
                break;
            }
            case SlotTypes.Shield:
            {
                ShieldInfo = shieldInfo;
                break;
            }
            case SlotTypes.Pack:
            {
                PackInfo = packInfo;
                break;
            }
            case SlotTypes.MA:
            {
                MAInfo = maInfo;
                break;
            }
        }

        EquipInfo = equipInfo;
        UpgradeInfo = upgradeInfo;
    }

    public override string GetCardDescShow(bool isEnglish)
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        switch (EquipInfo.SlotType)
        {
            case SlotTypes.Weapon:
            {
                if (WeaponInfo.IsFrenzy) CardDescShow += AddAffixString(isEnglish, "Frenzy", "狂暴");
                if (WeaponInfo.IsSentry) CardDescShow += AddAffixString(isEnglish, "Sentry", "哨戒");
                if (WeaponInfo.WeaponType == WeaponTypes.Sword)
                {
                    CardDescShow += AddAffixString(isEnglish, "Sword", "刀剑");
                    CardDescShow += string.Format(isEnglish ? "Attack +{0}. " : "攻击 +{0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Charge {0}. " : "充能 {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }
                else if (WeaponInfo.WeaponType == WeaponTypes.Gun)
                {
                    CardDescShow += AddAffixString(isEnglish, "Gun", "枪");
                    CardDescShow += string.Format(isEnglish ? "ShootAttack +{0}. " : "每发伤害 +{0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Bullets {0}. " : "弹药 {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }
                else if (WeaponInfo.WeaponType == WeaponTypes.SniperGun)
                {
                    CardDescShow += AddAffixString(isEnglish, "SniperGun", "狙击枪");
                    CardDescShow += string.Format(isEnglish ? "Bullet +{0} attack. " : "弹丸伤害: {0} 点, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Add +{0} bullets. " : "弹药: {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }

                break;
            }
            case SlotTypes.Shield:
            {
                if (ShieldInfo.IsDefence) CardDescShow += AddAffixString(isEnglish, "Defence", "嘲讽");
                if (ShieldInfo.ShieldType == ShieldTypes.Armor)
                {
                    CardDescShow += AddAffixString(isEnglish, "Armor", "护甲");
                    CardDescShow += string.Format(isEnglish ? "Defence {0} damage. " : "阻挡 {0} 点伤害, ", BaseInfo.AddHightLightColorToText(ShieldInfo.Armor.ToString()));
                }
                else if (ShieldInfo.ShieldType == ShieldTypes.Shield)
                {
                    CardDescShow += AddAffixString(isEnglish, "Shield", "护盾");
                    CardDescShow += string.Format(isEnglish ? "Reduce damage per attack by {0}. " : "受到每次伤害减少 {0} 点, ", BaseInfo.AddHightLightColorToText(ShieldInfo.Shield.ToString()));
                }

                break;
            }
            case SlotTypes.Pack:
            {
                if (PackInfo.IsFrenzy) CardDescShow += AddAffixString(isEnglish, "Frenzy", "狂暴");
                if (PackInfo.IsSniper) CardDescShow += AddAffixString(isEnglish, "Sniper", "狙击");
                if (PackInfo.IsDefence) CardDescShow += AddAffixString(isEnglish, "Defence", "嘲讽");
                if (PackInfo.DodgeProp != 0)
                {
                    CardDescShow += AddAffixString(isEnglish, "Dodge", "闪避");
                    CardDescShow += string.Format(isEnglish ? "PR: {0}. " : "概率: {0}, ", BaseInfo.AddHightLightColorToText(PackInfo.DodgeProp + "%"));
                }

                break;
            }
            case SlotTypes.MA:
            {
                if (PackInfo.IsFrenzy) CardDescShow += AddAffixString(isEnglish, "Frenzy", "狂暴");
                if (PackInfo.IsSniper) CardDescShow += AddAffixString(isEnglish, "Sniper", "狙击");
                if (PackInfo.IsDefence) CardDescShow += AddAffixString(isEnglish, "Defence", "嘲讽");
                break;
            }
        }

        CardDescShow += base.GetCardDescShow(isEnglish);

        CardDescShow = CardDescShow.TrimEnd(",.; \n".ToCharArray());

        return CardDescShow;
    }

    private string AddAffixString(bool isEnglish, string english, string chinese)
    {
        return isEnglish ? BaseInfo.AddImportantColorToText(english) + ". " : BaseInfo.AddImportantColorToText(chinese) + ", ";
    }

    public override string GetCardColor()
    {
        switch (EquipInfo.SlotType)
        {
            case SlotTypes.Weapon:
                return AllColors.ColorDict[AllColors.ColorType.WeaponCardColor];
            case SlotTypes.Shield:
                return AllColors.ColorDict[AllColors.ColorType.ShieldCardColor];
            case SlotTypes.Pack:
                return AllColors.ColorDict[AllColors.ColorType.PackCardColor];
            case SlotTypes.MA:
                return AllColors.ColorDict[AllColors.ColorType.MACardColor];
        }

        return null;
    }

    public override float GetCardColorIntensity()
    {
        switch (EquipInfo.SlotType)
        {
            case SlotTypes.Weapon:
                return AllColors.IntensityDict[AllColors.ColorType.WeaponCardColor];
            case SlotTypes.Shield:
                return AllColors.IntensityDict[AllColors.ColorType.ShieldCardColor];
            case SlotTypes.Pack:
                return AllColors.IntensityDict[AllColors.ColorType.PackCardColor];
            case SlotTypes.MA:
                return AllColors.IntensityDict[AllColors.ColorType.MACardColor];
        }

        return 0f;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Equip cb = new CardInfo_Equip(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            equipInfo: EquipInfo,
            weaponInfo: WeaponInfo,
            shieldInfo: ShieldInfo,
            packInfo: PackInfo,
            maInfo: MAInfo,
            sideEffectBundle: SideEffectBundle.Clone(),
            sideEffectBundle_OnBattleGround: SideEffectBundle_OnBattleGround.Clone());
        return cb;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        switch (EquipInfo.SlotType)
        {
            case SlotTypes.Weapon:
            {
                switch (WeaponInfo.WeaponType)
                {
                    case WeaponTypes.Sword:
                    {
                        return isEnglish ? "EQ(Sword)" : "装备牌(刀剑)";
                    }
                    case WeaponTypes.Gun:
                    {
                        return isEnglish ? "EQ(Gun)" : "装备牌(枪)";
                    }
                    case WeaponTypes.SniperGun:
                    {
                        return isEnglish ? "EQ(SnpGun)" : "装备牌(狙击枪)";
                    }
                }

                return "";
            }
            case SlotTypes.Shield:
            {
                return isEnglish ? "EQ(Shield)" : "装备牌(防具)";
            }
            case SlotTypes.Pack:
            {
                return isEnglish ? "EQ(Pack)" : "装备牌(背包)";
            }
            case SlotTypes.MA:
            {
                return isEnglish ? "EQ(MA)" : "装备牌(堡垒)";
            }
        }

        return "";
    }
}
public class CardInfo_Equip : CardInfo_Base
{
    public CardInfo_Equip()
    {
    }

    public CardInfo_Equip(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, EquipInfo equipInfo, WeaponInfo weaponInfo, ShieldInfo shieldInfo, PackInfo packInfo, MAInfo maInfo, SideEffectBundle sideEffects, SideEffectBundle sideEffects_OnBattleGround)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects: sideEffects,
            sideEffects_OnBattleGround: sideEffects_OnBattleGround)
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
                if (WeaponInfo.WeaponType == WeaponTypes.Sword)
                {
                    CardDescShow += string.Format(isEnglish ? "Add +{0} attack. " : "攻击力: {0} 点, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Set +{0} weapon energy. " : "充能:  {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }
                else if (WeaponInfo.WeaponType == WeaponTypes.Gun)
                {
                    CardDescShow += string.Format(isEnglish ? "Bullet +{0} attack. " : "弹丸伤害: {0} 点, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Add +{0} bullets. " : "弹药: {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }
                else if (WeaponInfo.WeaponType == WeaponTypes.SniperGun)
                {
                    CardDescShow += string.Format(BaseInfo.AddHightLightColorToText(isEnglish ? "Sniper. " : "狙击: ") + (isEnglish ? "Bullet +{0} attack. " : "弹丸伤害: {0} 点, "), BaseInfo.AddHightLightColorToText(WeaponInfo.Attack.ToString()));
                    CardDescShow += string.Format(isEnglish ? "Add +{0} bullets. " : "弹药: {0}, ", BaseInfo.AddHightLightColorToText(WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax));
                }

                if (WeaponInfo.IsFrenzy) CardDescShow += BaseInfo.AddImportantColorToText(isEnglish ? "Frenzy. " : "狂暴, ");
                if (WeaponInfo.IsSentry) CardDescShow += BaseInfo.AddImportantColorToText(isEnglish ? "Sentry. " : "哨戒, ");

                break;
            }
            case SlotTypes.Shield:
            {
                if (ShieldInfo.ShieldType == ShieldTypes.Armor)
                {
                    CardDescShow += string.Format(isEnglish ? "Defence {0} damage. " : "阻挡 {0} 点伤害, ", BaseInfo.AddHightLightColorToText(ShieldInfo.Armor.ToString()));
                }
                else if (ShieldInfo.ShieldType == ShieldTypes.Shield)
                {
                    CardDescShow += string.Format(isEnglish ? "Reduce damage per attack by {0}. " : "受到的伤害减少 {0} 点, ", BaseInfo.AddHightLightColorToText(ShieldInfo.Shield.ToString()));
                }

                if (ShieldInfo.IsDefence) CardDescShow += BaseInfo.AddImportantColorToText(isEnglish ? "Defence. " : "嘲讽, ");

                break;
            }
            case SlotTypes.Pack:
            {
                break;
            }
            case SlotTypes.MA:
            {
                break;
            }
        }

        CardDescShow += base.GetCardDescShow(isEnglish);

        CardDescShow = CardDescShow.TrimEnd(",.;\n".ToCharArray());

        return CardDescShow;
    }

    public override string GetCardColor()
    {
        switch (EquipInfo.SlotType)
        {
            case SlotTypes.Weapon:
                return GamePlaySettings.WeaponCardColor;
            case SlotTypes.Shield:
                return GamePlaySettings.ShieldCardColor;
            case SlotTypes.Pack:
                return GamePlaySettings.PackCardColor;
            case SlotTypes.MA:
                return GamePlaySettings.MACardColor;
        }

        return null;
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
            sideEffects: SideEffects.Clone(),
            sideEffects_OnBattleGround: SideEffects_OnBattleGround.Clone());
        return cb;
    }

    public override string GetCardTypeDesc(bool isEnglish)
    {
        return isEnglish ? "Equip" : "装备牌";
    }
}


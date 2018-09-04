using System;
using System.Collections.Generic;

public class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon()
    {
    }

    public CardInfo_Weapon(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, WeaponInfo weaponInfo, List<SideEffectBase> sideEffects_OnEndRound, List<SideEffectBase> sideEffects_OnPlayOut, List<SideEffectBase> sideEffects_OnSummoned, List<SideEffectBase> sideEffects_OnDie)
        : base(cardID: cardID,
            baseInfo: baseInfo,
            sideEffects_OnEndRound: sideEffects_OnEndRound,
            sideEffects_OnPlayOut: sideEffects_OnPlayOut,
            sideEffects_OnSummoned: sideEffects_OnSummoned,
            sideEffects_OnDie: sideEffects_OnDie)
    {
        WeaponInfo = weaponInfo;
        UpgradeInfo = upgradeInfo;
    }

    public override string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (WeaponInfo.WeaponType == WeaponTypes.Sword)
        {
            CardDescShow += "攻击力: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Attack.ToString()) + " 点\n";
            CardDescShow += "充能: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax) + "\n";
        }
        else if (WeaponInfo.WeaponType == WeaponTypes.Gun)
        {
            CardDescShow += "弹丸伤害: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Attack.ToString()) + " 点\n";
            CardDescShow += "弹药: " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, WeaponInfo.Energy + "/" + WeaponInfo.EnergyMax) + "\n";
        }

        CardDescShow += base.GetCardDescShow();

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        CardInfo_Base temp = base.Clone();
        CardInfo_Weapon cb = new CardInfo_Weapon(
            cardID: CardID,
            baseInfo: BaseInfo,
            upgradeInfo: UpgradeInfo,
            weaponInfo: WeaponInfo,
            sideEffects_OnEndRound: temp.SideEffects_OnEndRound,
            sideEffects_OnPlayOut: temp.SideEffects_OnPlayOut,
            sideEffects_OnSummoned: temp.SideEffects_OnSummoned,
            sideEffects_OnDie: temp.SideEffects_OnDie);
        return cb;
    }
}
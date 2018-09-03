using System;
using System.Collections.Generic;

public class CardInfo_Weapon : CardInfo_Base
{
    public CardInfo_Weapon()
    {
    }

    public CardInfo_Weapon(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, WeaponInfo weaponInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        WeaponInfo = weaponInfo;
        UpgradeInfo = upgradeInfo;
        SideEffects_OnDie = sideEffects_OnDie;
    }

    public string GetCardDescShow()
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

        CardDescShow = CardDescShow.TrimEnd(",;\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        CardInfo_Weapon cb = new CardInfo_Weapon(CardID, BaseInfo, UpgradeInfo, WeaponInfo, new_SideEffects_OnDie);
        return cb;
    }
}
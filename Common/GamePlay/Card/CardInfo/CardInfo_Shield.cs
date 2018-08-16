using System;
using System.Collections.Generic;

public class CardInfo_Shield : CardInfo_Base
{
    public CardInfo_Shield()
    {
    }

    public CardInfo_Shield(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, ShieldInfo shieldInfo, List<SideEffectBase> sideEffects_OnDie) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        ShieldInfo = shieldInfo;
        SideEffects_OnDie = sideEffects_OnDie;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (ShieldInfo.ShieldType == ShieldTypes.Armor)
        {
            CardDescShow += "阻挡 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Armor.ToString()) + " 点伤害\n";
        }
        else if (ShieldInfo.ShieldType == ShieldTypes.Shield)
        {
            CardDescShow += "受到的伤害减少 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, ShieldInfo.Shield.ToString()) + " 点\n";
        }

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects_OnDie = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects_OnDie.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        CardInfo_Shield cb = new CardInfo_Shield(CardID, BaseInfo, UpgradeInfo, ShieldInfo, new_SideEffects_OnDie);
        return cb;
    }
}
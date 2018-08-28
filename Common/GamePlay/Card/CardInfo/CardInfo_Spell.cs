using System;
using System.Collections.Generic;

public class CardInfo_Spell : CardInfo_Base
{
    public CardInfo_Spell()
    {
    }

    public CardInfo_Spell(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, List<SideEffectBase> sideEffects) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        SideEffects_OnSummoned = sideEffects;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;
        foreach (SideEffectBase se in SideEffects_OnSummoned)
        {
            CardDescShow += se.GenerateDesc() + ";\n";
        }

        CardDescShow = CardDescShow.TrimEnd(";\n".ToCharArray());

        return CardDescShow;
    }

    public override CardInfo_Base Clone()
    {
        List<SideEffectBase> new_SideEffects = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnDie)
        {
            new_SideEffects.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        CardInfo_Spell cs = new CardInfo_Spell(CardID, BaseInfo, UpgradeInfo, new_SideEffects);
        return cs;
    }
}
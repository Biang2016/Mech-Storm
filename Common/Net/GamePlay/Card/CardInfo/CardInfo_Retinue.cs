using System;
using System.Collections.Generic;

public class CardInfo_Retinue : CardInfo_Base
{
    public CardInfo_Retinue()
    {
    }

    public CardInfo_Retinue(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, LifeInfo lifeInfo, BattleInfo battleInfo, SlotInfo slotInfo, List<SideEffectBase> sideEffects_OnDie, List<SideEffectBase> sideEffects_OnSummoned) : base(cardID, baseInfo)
    {
        UpgradeInfo = upgradeInfo;
        LifeInfo = lifeInfo;
        BattleInfo = battleInfo;
        SlotInfo = slotInfo;
        SideEffects_OnDie = sideEffects_OnDie;
        SideEffects_OnSummoned = sideEffects_OnSummoned;
    }

    public string GetCardDescShow()
    {
        string CardDescShow = BaseInfo.CardDescRaw;

        if (BattleInfo.BasicAttack != 0) CardDescShow += "攻击力 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicAttack) + "\n";
        if (BattleInfo.BasicArmor != 0) CardDescShow += "护甲 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicArmor) + "\n";
        if (BattleInfo.BasicShield != 0) CardDescShow += "护盾 " + BaseInfo.AddHightLightColorToText(BaseInfo.HightLightColor, "+" + BattleInfo.BasicShield) + "\n";


        if (SideEffects_OnDie.Count > 0)
        {
            CardDescShow += "亡语:";
            foreach (SideEffectBase se in SideEffects_OnDie)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
        }


        if (SideEffects_OnSummoned.Count > 0)
        {
            CardDescShow += "战吼:";
            foreach (SideEffectBase se in SideEffects_OnSummoned)
            {
                CardDescShow += se.GenerateDesc() + ";\n";
            }
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

        List<SideEffectBase> new_SideEffects_OnSummoned = new List<SideEffectBase>();
        foreach (SideEffectBase sideEffectBase in SideEffects_OnSummoned)
        {
            new_SideEffects_OnSummoned.Add((SideEffectBase) ((ICloneable) sideEffectBase).Clone());
        }

        CardInfo_Retinue cb = new CardInfo_Retinue(CardID, BaseInfo, UpgradeInfo, LifeInfo, BattleInfo, SlotInfo, new_SideEffects_OnDie, new_SideEffects_OnSummoned);
        return cb;
    }
}
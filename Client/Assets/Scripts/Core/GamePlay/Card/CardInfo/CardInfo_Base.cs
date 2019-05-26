using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using SideEffects;

public class CardInfo_Base : IClone<CardInfo_Base>
{
    public int CardID;
    public BaseInfo BaseInfo;
    public UpgradeInfo UpgradeInfo;
    public LifeInfo LifeInfo;
    public BattleInfo BattleInfo;
    public MechInfo MechInfo;
    public EquipInfo EquipInfo;
    public TargetInfo TargetInfo;
    public WeaponInfo WeaponInfo;
    public ShieldInfo ShieldInfo;
    public PackInfo PackInfo;
    public MAInfo MAInfo;

    public SideEffectBundle SideEffectBundle;

    public CardInfo_Base()
    {
    }

    protected CardInfo_Base(int cardID, BaseInfo baseInfo, UpgradeInfo upgradeInfo, SideEffectBundle sideEffectBundle)
    {
        CardID = cardID;
        BaseInfo = baseInfo;
        UpgradeInfo = upgradeInfo;
        SideEffectBundle = sideEffectBundle;
        TargetInfo.Initialize(this);
        Pro_Initialize();
    }

    protected void Pro_Initialize()
    {
        if (EquipInfo.SlotType != SlotTypes.None)
        {
            BaseInfo.DragPurpose = DragPurpose.Equip;
        }
        else if (TargetInfo.HasNoTarget || BaseInfo.CardType == CardTypes.Mech)
        {
            BaseInfo.DragPurpose = DragPurpose.Summon;
        }
        else
        {
            BaseInfo.DragPurpose = DragPurpose.Target;
        }
    }

    public virtual string GetCardDescShow()
    {
        string CardDescShow = "";
        CardDescShow += SideEffectBundle.GetSideEffectsDesc();
        return CardDescShow;
    }

    public virtual string GetCardColor()
    {
        return null;
    }

    public virtual float GetCardColorIntensity()
    {
        return 0f;
    }

    public string GetCardDescTextColor()
    {
        return AllColors.ColorDict[AllColors.ColorType.CardDescTextColor];
    }

    public virtual CardInfo_Base Clone()
    {
        return new CardInfo_Base(CardID, BaseInfo, UpgradeInfo, SideEffectBundle.Clone());
    }

    public void Serialize(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(CardID);
        BaseInfo.Serialize(writer);
        UpgradeInfo.Serialize(writer);
        LifeInfo.Serialize(writer);
        BattleInfo.Serialize(writer);
        MechInfo.Serialize(writer);
        EquipInfo.Serialize(writer);
        WeaponInfo.Serialize(writer);
        ShieldInfo.Serialize(writer);
        PackInfo.Serialize(writer);
        MAInfo.Serialize(writer);
        SideEffectBundle.Serialize(writer);
    }

    public static CardInfo_Base Deserialze(DataStream reader)
    {
        string myType = reader.ReadString8();
        Assembly assembly = Assembly.GetAssembly(typeof(CardInfo_Base)); // 获取当前程序集 
        CardInfo_Base newCardInfo_Base = (CardInfo_Base) assembly.CreateInstance(myType);

        newCardInfo_Base.CardID = reader.ReadSInt32();
        newCardInfo_Base.BaseInfo = BaseInfo.Deserialze(reader);
        newCardInfo_Base.UpgradeInfo = UpgradeInfo.Deserialze(reader);
        newCardInfo_Base.LifeInfo = LifeInfo.Deserialze(reader);
        newCardInfo_Base.BattleInfo = BattleInfo.Deserialze(reader);
        newCardInfo_Base.MechInfo = MechInfo.Deserialze(reader);
        newCardInfo_Base.EquipInfo = EquipInfo.Deserialze(reader);
        newCardInfo_Base.WeaponInfo = WeaponInfo.Deserialze(reader);
        newCardInfo_Base.ShieldInfo = ShieldInfo.Deserialze(reader);
        newCardInfo_Base.PackInfo = PackInfo.Deserialze(reader);
        newCardInfo_Base.MAInfo = MAInfo.Deserialze(reader);
        newCardInfo_Base.SideEffectBundle = global::SideEffectBundle.Deserialize(reader);
        return newCardInfo_Base;
    }

    public void BaseExportToXML(XmlElement allCard_ele)
    {
        XmlDocument doc = allCard_ele.OwnerDocument;
        XmlElement old_node = null;
        foreach (XmlElement card_node in allCard_ele.ChildNodes)
        {
            if (card_node.Attributes["id"].Value.Equals(CardID.ToString()))
            {
                old_node = card_node;
            }
        }

        if (old_node != null)
        {
            allCard_ele.RemoveChild(old_node);
        }

        XmlElement card_ele = doc.CreateElement("Card");
        allCard_ele.AppendChild(card_ele);
        card_ele.SetAttribute("id", CardID.ToString());

        XmlElement baseInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(baseInfo_ele);
        baseInfo_ele.SetAttribute("name", "baseInfo");
        baseInfo_ele.SetAttribute("pictureID", BaseInfo.PictureID.ToString());
        foreach (KeyValuePair<string, string> kv in BaseInfo.CardNames)
        {
            baseInfo_ele.SetAttribute("cardName_" + kv.Key, kv.Value);
        }

        baseInfo_ele.SetAttribute("isTemp", BaseInfo.IsTemp.ToString());
        baseInfo_ele.SetAttribute("isHide", BaseInfo.IsHide.ToString());
        baseInfo_ele.SetAttribute("metal", BaseInfo.Metal.ToString());
        baseInfo_ele.SetAttribute("energy", BaseInfo.Energy.ToString());
        baseInfo_ele.SetAttribute("coin", BaseInfo.Coin.ToString());
        baseInfo_ele.SetAttribute("limitNum", BaseInfo.LimitNum.ToString());
        baseInfo_ele.SetAttribute("cardRareLevel", BaseInfo.CardRareLevel.ToString());
        baseInfo_ele.SetAttribute("cardType", BaseInfo.CardType.ToString());

        XmlElement upgradeInfo_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(upgradeInfo_ele);
        upgradeInfo_ele.SetAttribute("name", "upgradeInfo");
        upgradeInfo_ele.SetAttribute("upgradeCardID", UpgradeInfo.UpgradeCardID.ToString());
        upgradeInfo_ele.SetAttribute("degradeCardID", UpgradeInfo.DegradeCardID.ToString());

        ChildrenExportToXML(card_ele);

        XmlElement sideEffectsBundle_ele = doc.CreateElement("CardInfo");
        card_ele.AppendChild(sideEffectsBundle_ele);
        sideEffectsBundle_ele.SetAttribute("name", "sideEffectsBundle");
        foreach (SideEffectExecute see in SideEffectBundle.SideEffectExecutes)
        {
            XmlElement sideEffectExecute_ele = doc.CreateElement("SideEffectExecute");
            sideEffectsBundle_ele.AppendChild(sideEffectExecute_ele);
            sideEffectExecute_ele.SetAttribute("ExecuteSettingTypes", see.ExecuteSettingType.ToString());
            if (see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.Others)
            {
                ExportExecuteSettingsToElement(see.M_ExecuteSetting, sideEffectExecute_ele);
            }

            foreach (SideEffectBase se in see.SideEffectBases)
            {
                XmlElement sideEffect_ele = doc.CreateElement("SideEffect");
                sideEffectExecute_ele.AppendChild(sideEffect_ele);
                ExportSideEffectBaseToElement(se, sideEffect_ele);

                if (se is AddPlayerBuff_Base addBuff_SE)
                {
                    XmlElement buff_ele = doc.CreateElement("Buff");
                    sideEffect_ele.AppendChild(buff_ele);
                    ExportSideEffectBaseToElement(addBuff_SE, buff_ele);
                    ExportExecuteSettingsToElement(addBuff_SE.AttachedBuffSEE.M_ExecuteSetting, buff_ele);
                    foreach (SideEffectBase buff_SubSE in addBuff_SE.AttachedBuffSEE.SideEffectBases[0].Sub_SideEffect)
                    {
                        XmlElement buff_SubSE_ele = doc.CreateElement("SideEffect");
                        buff_ele.AppendChild(buff_SubSE_ele);
                        ExportSideEffectBaseToElement(buff_SubSE, buff_SubSE_ele);
                    }
                }
            }
        }
    }

    private static void ExportExecuteSettingsToElement(SideEffectExecute.ExecuteSetting es, XmlElement ele)
    {
        ele.SetAttribute("triggerTime", es.TriggerTime.ToString());
        ele.SetAttribute("triggerRange", es.TriggerRange.ToString());
        ele.SetAttribute("triggerDelayTimes", es.TriggerDelayTimes.ToString());
        ele.SetAttribute("triggerTimes", es.TriggerTimes.ToString());
        ele.SetAttribute("removeTriggerTime", es.RemoveTriggerTime.ToString());
        ele.SetAttribute("removeTriggerRange", es.RemoveTriggerRange.ToString());
        ele.SetAttribute("removeTriggerTimes", es.RemoveTriggerTimes.ToString());
    }

    private static void ExportSideEffectBaseToElement(SideEffectBase se, XmlElement ele)
    {
        ele.SetAttribute("name", se.Name);
        foreach (SideEffectValue sev in se.M_SideEffectParam.SideEffectValues)
        {
            switch (sev)
            {
                case SideEffectValue_ConstInt sev_ConstInt:
                {
                    if (sev_ConstInt.EnumType == typeof(CardDeck))
                    {
                        ele.SetAttribute(sev.Name, sev_ConstInt.Value.ToString());
                    }
                    else if (sev_ConstInt.EnumType != null)
                    {
                        string enum_name = Enum.ToObject(sev_ConstInt.EnumType, sev_ConstInt.Value).ToString();
                        ele.SetAttribute(sev.Name, enum_name);
                    }
                    else
                    {
                        ele.SetAttribute(sev.Name, sev_ConstInt.Value.ToString());
                    }

                    break;
                }
                case SideEffectValue_MultipliedInt sev_MultipliedInt:
                {
                    ele.SetAttribute(sev.Name, sev_MultipliedInt.Value.ToString());
                    break;
                }
                case SideEffectValue_Bool sev_Bool:
                {
                    ele.SetAttribute(sev.Name, sev_Bool.Value.ToString());
                    break;
                }
                case SideEffectValue_String sev_String:
                {
                    ele.SetAttribute(sev.Name, sev_String.Value);
                    break;
                }
            }
        }
    }

    protected virtual void ChildrenExportToXML(XmlElement card_ele)
    {
    }

    public virtual string GetCardTypeDesc()
    {
        return null;
    }

    public static CardInfo_Base ConvertCardInfo(CardInfo_Base src, CardTypes cardType)
    {
        switch (src)
        {
            case CardInfo_Mech ci:
            {
                switch (cardType)
                {
                    case CardTypes.Mech:
                    {
                        return (CardInfo_Mech) ci.Clone();
                    }
                    case CardTypes.Equip:
                    {
                        CardInfo_Base res = new CardInfo_Equip(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new EquipInfo(SlotTypes.Weapon),
                            new WeaponInfo(1, 1, 1, WeaponTypes.Sword, false, false),
                            new ShieldInfo(),
                            new PackInfo(),
                            new MAInfo(),
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Equip;
                        return (CardInfo_Equip) res;
                    }
                    case CardTypes.Spell:
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Spell;
                        return (CardInfo_Spell) res;
                    }
                    case CardTypes.Energy:
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Energy;
                        return (CardInfo_Spell) res;
                    }
                }

                break;
            }
            case CardInfo_Equip ci:
            {
                switch (cardType)
                {
                    case CardTypes.Mech:
                    {
                        CardInfo_Base res = new CardInfo_Mech(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new LifeInfo(1, 1),
                            new BattleInfo(0, 0, 0),
                            new MechInfo(false, false, false, false, false, SlotTypes.None, SlotTypes.None, SlotTypes.None, SlotTypes.None),
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Mech;
                        return (CardInfo_Mech) res;
                    }
                    case CardTypes.Equip:
                    {
                        return (CardInfo_Equip) ci.Clone();
                    }
                    case CardTypes.Spell:
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Spell;
                        return (CardInfo_Spell) res;
                    }
                    case CardTypes.Energy:
                    {
                        CardInfo_Base res = new CardInfo_Spell(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Energy;
                        return (CardInfo_Spell) res;
                    }
                }

                break;
            }
            case CardInfo_Spell ci:
            {
                switch (cardType)
                {
                    case CardTypes.Mech:
                    {
                        CardInfo_Base res = new CardInfo_Mech(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new LifeInfo(1, 1),
                            new BattleInfo(0, 0, 0),
                            new MechInfo(false, false, false, false, false, SlotTypes.None, SlotTypes.None, SlotTypes.None, SlotTypes.None),
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Mech;
                        return (CardInfo_Mech) res;
                    }
                    case CardTypes.Equip:
                    {
                        CardInfo_Base res = new CardInfo_Equip(
                            ci.CardID, ci.BaseInfo, ci.UpgradeInfo,
                            new EquipInfo(SlotTypes.Weapon),
                            new WeaponInfo(1, 1, 1, WeaponTypes.Sword, false, false),
                            new ShieldInfo(),
                            new PackInfo(),
                            new MAInfo(),
                            ci.SideEffectBundle.Clone());
                        res.BaseInfo.CardType = CardTypes.Equip;
                        return (CardInfo_Equip) res;
                    }
                    case CardTypes.Spell:
                    {
                        CardInfo_Spell cis = (CardInfo_Spell) ci.Clone();
                        cis.BaseInfo.CardType = CardTypes.Spell;
                        return cis;
                    }
                    case CardTypes.Energy:
                    {
                        CardInfo_Spell cis = (CardInfo_Spell) ci.Clone();
                        cis.BaseInfo.CardType = CardTypes.Energy;
                        return cis;
                    }
                }

                break;
            }
        }

        return null;
    }
}
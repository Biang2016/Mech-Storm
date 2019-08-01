using System.Collections.Generic;
using UnityEngine;

public class AffixPanel : BaseUIForm
{
    private AffixPanel()
    {
    }

    [SerializeField] private Transform AffixContainer;
    [SerializeField] private Animator AffixPanelAnim;

    private List<Affix> Affixes = new List<Affix>();
    private HashSet<AffixType> AffixTypes = new HashSet<AffixType>();

    public bool IsShow => Affixes.Count != 0;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Fixed,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
    }

    public override void Hide()
    {
        HideAffixPanel();
    }

    /// <summary>
    /// 根据CardInfo和mechs来决定是否要显示AffixPanel
    /// </summary>
    /// <param name="cardInfos"></param>
    /// <param name="mechs"></param>
    /// <returns>是否显示</returns>
    public bool ShowAffixTips(List<CardInfo_Base> cardInfos, List<ModuleMech> mechs)
    {
        HashSet<AffixType> affixTypes = new HashSet<AffixType>();
        if (cardInfos != null)
        {
            foreach (CardInfo_Base ci in cardInfos)
            {
                GetAffixTypeByCardInfo(affixTypes, ci);
            }
        }

        if (mechs != null)
        {
            foreach (ModuleMech mech in mechs)
            {
                GetAffixTypeByMech(affixTypes, mech);
            }
        }

        ShowAffixPanel(affixTypes);

        return affixTypes.Count > 0;
    }

    public void ClearAllAffixes()
    {
        AffixTypes.Clear();
        foreach (Affix affix in Affixes)
        {
            affix.PoolRecycle();
        }

        Affixes.Clear();
    }

    private static void GetAffixTypeByCardInfo(HashSet<AffixType> affixTypes, CardInfo_Base cardInfo)
    {
        GetAffixTypeFromSideEffectBundle(affixTypes, cardInfo.SideEffectBundle);
        if (cardInfo.HasAuro)
        {
            affixTypes.Add(AffixType.Aura);
            GetAffixTypeFromSideEffectBundle(affixTypes, cardInfo.SideEffectBundle_BattleGroundAura);
        }

        if (cardInfo.MechInfo.IsFrenzy || cardInfo.WeaponInfo.IsFrenzy || cardInfo.PackInfo.IsFrenzy || cardInfo.MAInfo.IsFrenzy)
        {
            affixTypes.Add(AffixType.Frenzy);
        }

        if (cardInfo.MechInfo.IsDefense || cardInfo.ShieldInfo.IsDefense || cardInfo.PackInfo.IsDefense || cardInfo.MAInfo.IsDefense)
        {
            affixTypes.Add(AffixType.Defence);
        }

        if (cardInfo.MechInfo.IsSniper || cardInfo.PackInfo.IsSniper || cardInfo.MAInfo.IsSniper)
        {
            affixTypes.Add(AffixType.Sniper);
        }

        if (cardInfo.MechInfo.IsCharger)
        {
            affixTypes.Add(AffixType.Charger);
        }

        if (cardInfo.MechInfo.IsSentry || cardInfo.WeaponInfo.IsSentry)
        {
            affixTypes.Add(AffixType.Sentry);
        }

        if (cardInfo.BaseInfo.CardType == CardTypes.Equip && cardInfo.EquipInfo.SlotType == SlotTypes.Shield)
        {
            if (cardInfo.ShieldInfo.ShieldType == ShieldTypes.Armor)
            {
                affixTypes.Add(AffixType.Armor);
            }

            if (cardInfo.ShieldInfo.ShieldType == ShieldTypes.Shield)
            {
                affixTypes.Add(AffixType.Shield);
            }
        }

        if (cardInfo.BaseInfo.CardType == CardTypes.Equip && cardInfo.EquipInfo.SlotType == SlotTypes.MA)
        {
            affixTypes.Add(AffixType.MA);
        }

        if (cardInfo.BaseInfo.CardType == CardTypes.Mech && cardInfo.MechInfo.Slots[3] == SlotTypes.MA)
        {
            affixTypes.Add(AffixType.MA);
        }

        if (cardInfo.BattleInfo.BasicAttack != 0)
        {
            affixTypes.Add(AffixType.Attack);
        }

        if (cardInfo.BattleInfo.BasicArmor != 0)
        {
            affixTypes.Add(AffixType.Armor);
        }

        if (cardInfo.BattleInfo.BasicShield != 0)
        {
            affixTypes.Add(AffixType.Shield);
        }

        if (cardInfo.BaseInfo.CardType == CardTypes.Equip && cardInfo.EquipInfo.SlotType == SlotTypes.Weapon)
        {
            if (cardInfo.WeaponInfo.WeaponType == WeaponTypes.Sword)
            {
                affixTypes.Add(AffixType.Sword);
            }

            if (cardInfo.WeaponInfo.WeaponType == WeaponTypes.Gun)
            {
                affixTypes.Add(AffixType.Gun);
            }

            if (cardInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                affixTypes.Add(AffixType.SniperGun);
            }
        }
    }

    private static void GetAffixTypeFromSideEffectBundle(HashSet<AffixType> affixTypes, SideEffectBundle seb)
    {
        foreach (SideEffectExecute see in seb.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnPlayCard, SideEffectExecute.TriggerRange.Self))
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                if (se is Exile_Base)
                {
                    affixTypes.Add(AffixType.Disposable);
                }
            }
        }

        if (seb.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnMechDie, SideEffectExecute.TriggerRange.Self).Count != 0)
        {
            affixTypes.Add(AffixType.Die);
        }

        if (seb.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnMechSummon, SideEffectExecute.TriggerRange.Self).Count != 0)
        {
            affixTypes.Add(AffixType.BattleCry);
        }
    }

    private static void GetAffixTypeByMech(HashSet<AffixType> affixTypes, ModuleMech mech)
    {
        if (mech.M_ImmuneLeftRounds != 0)
        {
            affixTypes.Add(AffixType.Immune);
        }

        if (mech.M_InactivityRounds != 0)
        {
            affixTypes.Add(AffixType.Inactivity);
        }
    }

    public void ShowAffixPanel(HashSet<AffixType> affixTypes)
    {
        if (affixTypes.Count == 0)
        {
            AffixPanelAnim.SetTrigger("Hide");
        }
        else
        {
            ClearAllAffixes();
            AddAffixes(affixTypes);
            AffixPanelAnim.SetTrigger("Show");
        }
    }

    private void HideAffixPanel()
    {
        ClearAllAffixes();
        AffixPanelAnim.SetTrigger("Hide");
    }

    private void AddAffixes(HashSet<AffixType> affixTypes)
    {
        foreach (AffixType affixType in affixTypes)
        {
            AddAffix(affixType);
        }
    }

    private void AddAffix(AffixType affixType)
    {
        if (!AffixTypes.Contains(affixType))
        {
            AffixTypes.Add(affixType);
            Affix newAffix = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Affix].AllocateGameObject<Affix>(AffixContainer);
            newAffix.Initialize(affixType);
            Affixes.Add(newAffix);
        }
    }

    private void RemoveAffix(AffixType affixType)
    {
        if (AffixTypes.Contains(affixType))
        {
            AffixTypes.Remove(affixType);
            foreach (Affix affix in Affixes)
            {
                if (affix.AffixType == affixType)
                {
                    Affixes.Remove(affix);
                    affix.PoolRecycle();
                }
            }
        }
    }
}
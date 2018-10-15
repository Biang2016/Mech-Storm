using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixManager : MonoSingletion<AffixManager>
{
    [SerializeField] private Transform AffixPanel;
    [SerializeField] private VerticalLayoutGroup VerticalLayoutGroup;
    [SerializeField] private Animator AffixPanelAnim;

    private Boo.Lang.List<Affix> Affixs = new Boo.Lang.List<Affix>();
    private HashSet<AffixType> AffixTypes = new HashSet<AffixType>();

    void Start()
    {
    }

    private float hideAffixPanelTicker;
    private float hideAffixPanelInterval = 0.5f;

    void Update()
    {
        if (StartMenuManager.Instance.M_StateMachine.GetState() == StartMenuManager.StateMachine.States.Show)
        {
            if (hideAffixPanelTicker > hideAffixPanelInterval)
            {
                hideAffixPanelTicker = 0;
                HideAffixPanel();
            }
            else
            {
                hideAffixPanelTicker += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 根据CardInfo来决定是否要显示AffixPanel
    /// </summary>
    /// <param name="cardInfos"></param>
    /// <returns>是否显示</returns>
    public bool ShowAffixTips(List<CardInfo_Base> cardInfos)
    {
        HashSet<AffixType> affixTypes = new HashSet<AffixType>();
        foreach (CardInfo_Base ci in cardInfos)
        {
            GetAffixTypeByCardInfo(affixTypes, ci);
        }

        ShowAffixPanel(affixTypes);

        return affixTypes.Count > 0;
    }

    private void GetAffixTypeByCardInfo(HashSet<AffixType> affixTypes, CardInfo_Base cardInfo)
    {
        if (cardInfo.SideEffectBundle_OnBattleGround.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnRetinueDie, SideEffectBundle.TriggerRange.Self).Count != 0)
        {
            affixTypes.Add(AffixType.Die);
        }

        if (cardInfo.SideEffectBundle_OnBattleGround.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnRetinueSummon, SideEffectBundle.TriggerRange.Self).Count != 0)
        {
            affixTypes.Add(AffixType.BattleCry);
        }

        if (cardInfo.RetinueInfo.IsFrenzy || cardInfo.WeaponInfo.IsFrenzy || cardInfo.PackInfo.IsFrenzy || cardInfo.MAInfo.IsFrenzy)
        {
            affixTypes.Add(AffixType.Frenzy);
        }

        if (cardInfo.RetinueInfo.IsDefence || cardInfo.ShieldInfo.IsDefence || cardInfo.PackInfo.IsDefence || cardInfo.MAInfo.IsDefence)
        {
            affixTypes.Add(AffixType.Defence);
        }

        if (cardInfo.RetinueInfo.IsSniper || cardInfo.PackInfo.IsSniper || cardInfo.MAInfo.IsSniper)
        {
            affixTypes.Add(AffixType.Sniper);
        }

        if (cardInfo.RetinueInfo.IsCharger)
        {
            affixTypes.Add(AffixType.Charger);
        }

        if (cardInfo.WeaponInfo.IsSentry)
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

        if (cardInfo.BaseInfo.CardType == CardTypes.Retinue && cardInfo.RetinueInfo.Slot4 == SlotTypes.MA)
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


        if (cardInfo.BaseInfo.CardType == CardTypes.Equip && cardInfo.EquipInfo.SlotType == SlotTypes.Pack)
        {
            if (cardInfo.PackInfo.DodgeProp != 0)
            {
                affixTypes.Add(AffixType.Dodge);
            }
        }
    }

    private void ShowAffixPanel(HashSet<AffixType> affixTypes)
    {
        if (affixTypes.Count == 0)
        {
            AffixPanelAnim.SetTrigger("Hide");
        }
        else
        {
            ClearAllAffixs();
            AddAffixs(affixTypes);
            AffixPanelAnim.SetTrigger("Show");
        }
    }

    public void HideAffixPanel()
    {
        AffixPanelAnim.SetTrigger("Hide");
    }

    private void AddAffixs(HashSet<AffixType> affixTypes)
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
            Affix newAffix = GameObjectPoolManager.Instance.Pool_AffixPool.AllocateGameObject<Affix>(AffixPanel);
            newAffix.Initialize(affixType);
            Affixs.Add(newAffix);
        }
    }

    private void RemoveAffix(AffixType affixType)
    {
        if (AffixTypes.Contains(affixType))
        {
            AffixTypes.Remove(affixType);
            foreach (Affix affix in Affixs)
            {
                if (affix.AffixType == affixType)
                {
                    Affixs.Remove(affix);
                    affix.PoolRecycle();
                }
            }
        }
    }

    public void ClearAllAffixs()
    {
        AffixTypes.Clear();
        foreach (Affix affix in Affixs)
        {
            affix.PoolRecycle();
        }

        Affixs.Clear();
    }
}
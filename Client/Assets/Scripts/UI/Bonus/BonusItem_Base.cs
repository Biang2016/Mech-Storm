using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class BonusItem_Base : PoolObject
{
    public override void PoolRecycle()
    {
        BonusCardInfo = null;
        base.PoolRecycle();
    }

    [SerializeField] private Text ItemDesc;
    public Image ItemImage;
    public Bonus Bonus;

    public CardInfo_Base BonusCardInfo;

    public static BonusItem_Base InstantiateBonusItem(Bonus bonus, Transform parent)
    {
        BonusItem_Base bib;
        if (bonus.M_BonusType == Bonus.BonusType.UnlockCardByID)
        {
            bib = GameObjectPoolManager.Instance.Pool_BigBonusItemPool.AllocateGameObject<BonusItem_Base>(parent);
        }
        else
        {
            bib = GameObjectPoolManager.Instance.Pool_SmallBonusItemPool.AllocateGameObject<BonusItem_Base>(parent);
        }

        bib.Initialize(bonus);
        return bib;
    }

    public virtual void Initialize(Bonus bonus)
    {
        Bonus = bonus;
        ItemDesc.text = Bonus.GetDesc(GameManager.Instance.IsEnglish);
        if (bonus.M_BonusType == Bonus.BonusType.UnlockCardByID)
        {
            BonusCardInfo = AllCards.GetCard(bonus.Value);
        }
    }

    public virtual void OnHover()
    {
        if (BonusCardInfo != null)
        {
            AffixManager.Instance.ShowAffixTips(new List<CardInfo_Base> {BonusCardInfo}, null);
        }
    }

    public virtual void OnExit()
    {
        if (BonusCardInfo != null)
        {
            AffixManager.Instance.HideAffixPanel();
        }
    }
}
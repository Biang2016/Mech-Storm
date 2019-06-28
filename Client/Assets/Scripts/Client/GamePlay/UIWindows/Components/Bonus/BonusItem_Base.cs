using System.Collections.Generic;
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
        if (bonus.BonusType == Bonus.BonusTypes.UnlockCardByID)
        {
            bib = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BigBonusItem].AllocateGameObject<BonusItem_Base>(parent);
        }
        else
        {
            bib = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SmallBonusItem].AllocateGameObject<BonusItem_Base>(parent);
        }

        bib.Initialize(bonus);
        return bib;
    }

    public virtual void Initialize(Bonus bonus)
    {
        Bonus = bonus;
        ItemDesc.text = Bonus.GetDesc();
        if (bonus is Bonus_UnlockCardByID b_UnlockCardByID)
        {
            BonusCardInfo = AllCards.GetCard(b_UnlockCardByID.CardID);
        }
    }

    public virtual void OnHover()
    {
        if (BonusCardInfo != null)
        {
            UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {BonusCardInfo}, null);
        }
    }

    public virtual void OnExit()
    {
        if (BonusCardInfo != null)
        {
            UIManager.Instance.CloseUIForm<AffixPanel>();
        }
    }
}
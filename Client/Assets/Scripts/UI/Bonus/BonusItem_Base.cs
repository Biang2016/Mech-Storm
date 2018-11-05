using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class BonusItem_Base : PoolObject
{
    [SerializeField] private Text ItemDesc;
    [SerializeField] private Image ItemImage;
    public Bonus Bonus;

    public static BonusItem_Base InstantiateBonusItem(Bonus bonus, Transform parent)
    {
        BonusItem_Base bib;
        if (bonus.M_BonusType == Bonus.BonusType.UnlockCard)
        {
            bib = GameObjectPoolManager.Instance.Pool_BigBonusButtonPool.AllocateGameObject<BonusItem_Base>(parent);
        }
        else
        {
            bib = GameObjectPoolManager.Instance.Pool_SmallBonusButtonPool.AllocateGameObject<BonusItem_Base>(parent);
        }

        bib.Initialize(bonus);
        return bib;
    }

    public virtual void Initialize(Bonus bonus)
    {
        Bonus = bonus;
        ItemDesc.text = Bonus.GetDesc(GameManager.Instance.IsEnglish);
    }

    public delegate void BonusButtonClickHandler();

    public BonusButtonClickHandler OnButtonClick;
}
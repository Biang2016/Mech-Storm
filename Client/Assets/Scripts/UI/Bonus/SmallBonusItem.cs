using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class SmallBonusItem : BonusItem_Base
{
    [SerializeField] private Sprite AdjustDeckIcon;
    [SerializeField] private Sprite LifeIcon;
    [SerializeField] private Sprite EnergyIcon;
    [SerializeField] private Sprite BudgetIcon;


    public override void Initialize(Bonus bonus)
    {
        base.Initialize(bonus);
        switch (bonus.M_BonusType)
        {
            case Bonus.BonusType.AdjustDeck:
            {
                ItemImage.sprite = AdjustDeckIcon;
                ItemImage.transform.localScale = Vector3.one * 1f;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.LifeUpperLimit:
            {
                ItemImage.sprite = LifeIcon;
                ItemImage.color = GameManager.Instance.LifeIconColor;
                ItemImage.transform.localScale = Vector3.one * 0.6f;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.EnergyUpperLimit:
            {
                ItemImage.sprite = EnergyIcon;
                ItemImage.color = GameManager.Instance.EnergyIconColor;
                ItemImage.transform.localScale = Vector3.one * 0.8f;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.Budget:
            {
                ItemImage.sprite = BudgetIcon;
                ItemImage.color = Color.white;
                ItemImage.transform.localScale = Vector3.one * 0.8f;
                ItemImage.preserveAspect = true;
                break;
            }
        }
    }
}
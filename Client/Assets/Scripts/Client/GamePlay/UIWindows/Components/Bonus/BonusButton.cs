﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class BonusButton : PoolObject
{
    public override void PoolRecycle()
    {
        Reset();
        base.PoolRecycle();
    }

    public BonusGroup BonusGroup;

    [SerializeField] private Image BG_Image;
    [SerializeField] private Transform Container;
    [SerializeField] private Button Button;
    [SerializeField] private Image BorderSelected;
    [SerializeField] private Image Border;

    public List<BonusItem_Base> M_BonusItems = new List<BonusItem_Base>();

    public void Reset()
    {
        foreach (BonusItem_Base bonusItem in M_BonusItems)
        {
            bonusItem.PoolRecycle();
        }

        M_BonusItems.Clear();
        SetSelected(false);
        BG_Image.gameObject.SetActive(true);
        BorderSelected.gameObject.SetActive(true);
        Border.gameObject.SetActive(true);
        card = null;
        IsSelected = false;
    }

    public void Initialize(BonusGroup bonusGroup, UnityAction onClickAction)
    {
        Reset();

        BonusGroup = bonusGroup;
        if (BonusGroup.IsAlways)
        {
            Button.interactable = false;
            Button.enabled = false;
            Button.onClick.RemoveAllListeners();
        }
        else
        {
            Button.interactable = true;
            Button.enabled = true;
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(delegate
            {
                onClickAction();
                AudioManager.Instance.SoundPlay("sfx/OnStoryButtonClick");
            });
        }

        foreach (Bonus bonus in BonusGroup.Bonuses)
        {
            BonusItem_Base bi = BonusItem_Base.InstantiateBonusItem(bonus, Container);
            M_BonusItems.Add(bi);
        }

        if (M_BonusItems.Count == 1 && M_BonusItems[0] is BigBonusItem)
        {
            BigBonusItem bigBonusItem = (BigBonusItem) M_BonusItems[0];
            card = bigBonusItem.CurrentCard;
            card.BeDimColor();
            card.ShowCardBloom(false);
            BG_Image.gameObject.SetActive(false);
            BorderSelected.gameObject.SetActive(false);
            Border.gameObject.SetActive(false);
        }
    }

    private CardBase card = null;

    private bool IsSelected = false;

    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        BorderSelected.enabled = isSelected;
        card?.ShowCardBloom(isSelected);
        if (isSelected)
        {
            card?.BeBrightColor();
        }
        else
        {
            card?.BeDimColor();
        }
    }

    public void OnHover()
    {
        if (card)
        {
            UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {card.CardInfo}, null);
            card?.BeBrightColor();
        }
    }

    public void OnExit()
    {
        if (card)
        {
            UIManager.Instance.CloseUIForm<AffixPanel>();
            if (!IsSelected)
            {
                card?.BeDimColor();
            }
        }
    }
}
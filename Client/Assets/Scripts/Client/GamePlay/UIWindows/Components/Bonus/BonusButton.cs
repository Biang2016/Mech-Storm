using System.Collections.Generic;
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

    public void Initialize(BonusGroup bonusGroup, UnityAction onClickAction, HashSet<int> exceptionCardIDs, List<Bonus_BudgetLifeEnergyMixed.BudgetLifeEnergyComb> exceptionBudgetLifeEnergyComb)
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

        List<int> newBonusCardIDs = new List<int>();
        List<Bonus> removeBonuses = new List<Bonus>();
        List<Bonus> addBonuses = new List<Bonus>();

        foreach (Bonus bonus in BonusGroup.Bonuses)
        {
            if (bonus is Bonus_UnlockCardByID b_id)
            {
                if (exceptionCardIDs.Contains(b_id.CardID))
                {
                    removeBonuses.Add(bonus);
                }
                else
                {
                    exceptionCardIDs.Add(b_id.CardID);
                }
            }
        }

        foreach (Bonus bonus in BonusGroup.Bonuses)
        {
            if (bonus is Bonus_UnlockCardByLevelNum b_level)
            {
                CardInfo_Base ci = AllCards.GetRandomCardInfoByLevelNum(b_level.LevelNum, exceptionCardIDs);
                removeBonuses.Add(bonus);

                if (ci != null)
                {
                    exceptionCardIDs.Add(ci.CardID);
                    newBonusCardIDs.Add(ci.CardID);
                }
            }

            if (bonus is Bonus_BudgetLifeEnergyMixed b_mixed)
            {
                Bonus_BudgetLifeEnergyMixed.BudgetLifeEnergyComb comb = b_mixed.GetBudgetLifeEnergyComb(exceptionBudgetLifeEnergyComb);

                removeBonuses.Add(bonus);

                if (comb != null)
                {
                    exceptionBudgetLifeEnergyComb.Add(comb);
                    addBonuses.AddRange(comb.GenerateBonuses());
                }
            }
        }

        foreach (Bonus b in addBonuses)
        {
            BonusGroup.Bonuses.Add(b);
        }

        foreach (int newBonusCardID in newBonusCardIDs)
        {
            exceptionCardIDs.Add(newBonusCardID);
            BonusGroup.Bonuses.Add(new Bonus_UnlockCardByID(newBonusCardID));
        }

        foreach (Bonus removeBonus in removeBonuses)
        {
            BonusGroup.Bonuses.Remove(removeBonus);
        }

        if (BonusGroup.Bonuses.Count == 0)
        {
            int a = 0;
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
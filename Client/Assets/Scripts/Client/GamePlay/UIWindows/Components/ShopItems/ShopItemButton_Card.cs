using System.Collections.Generic;
using UnityEngine;

public class ShopItemButton_Card : ShopItemButton
{
    [SerializeField] private Transform CardContainer;
    [SerializeField] private Transform CardRotateSample;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        M_CardBase?.PoolRecycle();
        M_CardBase = null;
    }

    private CardBase M_CardBase;

    public override void Initialize(ShopItem shopItem)
    {
        base.Initialize(shopItem);
        ShopItem_Card si_card = (ShopItem_Card) shopItem;
        M_CardBase = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(si_card.GenerateCardID), CardContainer, CardBase.CardShowMode.CardReward);
        M_CardBase.transform.localPosition = CardRotateSample.localPosition;
        M_CardBase.transform.localRotation = CardRotateSample.localRotation;
        M_CardBase.transform.localScale = CardRotateSample.localScale;
        M_CardBase.CardOrder = 1;
        M_CardBase.BeBrightColor();
        M_CardBase.ShowCardBloom(false);
    }

    public void OnHover()
    {
        if (M_CardBase)
        {
            UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {M_CardBase.CardInfo}, null);
            M_CardBase.ShowCardBloom(true);
        }
    }

    public void OnExit()
    {
        if (M_CardBase)
        {
            UIManager.Instance.CloseUIForm<AffixPanel>();
            M_CardBase.ShowCardBloom(false);
        }
    }
}
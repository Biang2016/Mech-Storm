using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class BigBonusItem : BonusItem_Base
{
    public override void PoolRecycle()
    {
        if (CurrentCard != null) CurrentCard.PoolRecycle();
        base.PoolRecycle();
    }

    [SerializeField] private Transform CardContainer;
    [SerializeField] private Transform CardRotationSample;

    private CardBase CurrentCard;

    public override void Initialize(Bonus bonus)
    {
        base.Initialize(bonus);
        switch (bonus.M_BonusType)
        {
            case Bonus.BonusType.UnlockCard:
            {
                CurrentCard = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(bonus.Value), CardContainer, RoundManager.Instance.SelfClientPlayer, true);
                CurrentCard.transform.localScale = CardRotationSample.localScale;
                CurrentCard.transform.rotation = CardRotationSample.rotation;
                break;
            }
        }
    }
}
using UnityEngine;

class CardSelectWindowCardContainer : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        M_ChildCard?.PoolRecycle();
        M_ChildCard = null;
    }

    public CardBase M_ChildCard;

    public void Initialize(CardInfo_Base cardInfo)
    {
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, CardBase.CardShowMode.CardSelect);
        M_ChildCard = newCard;
        SetChildCardSize();
    }

    public void SetChildCardSize()
    {
        if (M_ChildCard) M_ChildCard.transform.localScale = Vector3.one * 13.5f;
    }
}
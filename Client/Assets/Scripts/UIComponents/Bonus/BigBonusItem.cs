using UnityEngine;
using UnityEngine.UI;

internal class BigBonusItem : BonusItem_Base
{
    public override void PoolRecycle()
    {
        if (CurrentCard != null) CurrentCard.PoolRecycle();
        CurrentCard = null;
        base.PoolRecycle();
    }

    [SerializeField] private Transform CardContainer;
    [SerializeField] private Transform CardRotationSample;
    [SerializeField] private Text UnlockText;
    [SerializeField] private Image UnlockImage;
    [SerializeField] private Image CardMask;

    public CardBase CurrentCard;

    void Awake()
    {
        UnlockText.text = LanguageManager.Instance.GetText("BigBonusItem_ToUnlockThisCard");
    }

    public override void Initialize(Bonus bonus)
    {
        base.Initialize(bonus);
        UnlockText.enabled = false;
        UnlockImage.enabled = false;
        CardMask.enabled = false;
        switch (bonus.M_BonusType)
        {
            case Bonus.BonusType.UnlockCardByID:
            {
                CurrentCard = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(bonus.BonusFinalValue), CardContainer, CardBase.CardShowMode.CardReward, RoundManager.Instance.SelfClientPlayer);
                CurrentCard.transform.localScale = CardRotationSample.localScale;
                CurrentCard.transform.rotation = CardRotationSample.rotation;
                CurrentCard.transform.position = CardRotationSample.position;
                CurrentCard.SetOrderInLayer(1);
                UnlockText.enabled = true;
                UnlockImage.enabled = true;
                CardMask.enabled = true;
                Color newColor = ClientUtils.HTMLColorToColor(CurrentCard.CardInfo.GetCardColor());
                UnlockImage.color = new Color(newColor.r / 1.5f, newColor.g / 1.5f, newColor.b / 1.5f);
                break;
            }
        }
    }
}
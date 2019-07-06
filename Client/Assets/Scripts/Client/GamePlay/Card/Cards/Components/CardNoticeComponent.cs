using TMPro;
using UnityEngine;

public class CardNoticeComponent : CardComponentBase
{
    [SerializeField] private GameObject BannerBlock;
    [SerializeField] private TextMeshPro BannerText;
    [SerializeField] private SpriteRenderer NewCardBanner;
    [SerializeField] private GameObject ArrowBlock;
    [SerializeField] private TextMeshPro ArrowText;
    [SerializeField] private SpriteRenderer Arrow;

    public enum BannerTypes
    {
        None,
        NewCard,
    }

    public enum ArrowTypes
    {
        None,
        Upgrade,
    }

    public void SetBannerType(BannerTypes bannerType)
    {
        switch (bannerType)
        {
            case BannerTypes.None:
                BannerBlock.SetActive(false);
                break;
            case BannerTypes.NewCard:
                BannerBlock.SetActive(true);
                LanguageManager.Instance.RegisterTextKey(BannerText, "CardBase_NewCardBanner");
                break;
        }
    }

    public void SetArrowType(ArrowTypes arrowType)
    {
        switch (arrowType)
        {
            case ArrowTypes.None:
                ArrowBlock.SetActive(false);
                break;
            case ArrowTypes.Upgrade:
                ArrowBlock.SetActive(true);
                ArrowText.text = LanguageManager.Instance.GetText("CardBase_Upgradable");
                break;
        }
    }

    void Awake()
    {
        BannerTextDefaultSortingOrder = BannerText.sortingOrder;
        BannerDefaultSortingOrder = NewCardBanner.sortingOrder;
        ArrowTextDefaultSortingOrder = ArrowText.sortingOrder;
        ArrowDefaultSortingOrder = Arrow.sortingOrder;
    }

    private int BannerTextDefaultSortingOrder;
    private int BannerDefaultSortingOrder;
    private int ArrowTextDefaultSortingOrder;
    private int ArrowDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        BannerText.sortingOrder = cardSortingIndex * 50 + BannerTextDefaultSortingOrder;
        NewCardBanner.sortingOrder = cardSortingIndex * 50 + BannerDefaultSortingOrder;
        ArrowText.sortingOrder = cardSortingIndex * 50 + ArrowTextDefaultSortingOrder;
        Arrow.sortingOrder = cardSortingIndex * 50 + ArrowDefaultSortingOrder;
    }
}
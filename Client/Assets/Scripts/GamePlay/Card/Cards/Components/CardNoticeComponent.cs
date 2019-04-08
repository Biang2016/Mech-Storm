using TMPro;
using UnityEngine;

public class CardNoticeComponent : CardComponentBase
{
    [SerializeField] private GameObject BannerBlock;
    [SerializeField] private TextMeshPro NewCardBannerText;
    [SerializeField] private SpriteRenderer NewCardBanner;
    [SerializeField] private GameObject UpgradeBlock;
    [SerializeField] private TextMeshPro UpgradeText;
    [SerializeField] private SpriteRenderer UpgradeArrow;

    public void SetBannerShow(bool isShow)
    {
        BannerBlock.SetActive(isShow);
    }

    public void SetUpgradeShow(bool isShow)
    {
        UpgradeBlock.SetActive(isShow);
    }

    public void SetNewCardBannerText(string bannerText)
    {
        NewCardBannerText.text = bannerText;
    }

    public void SetUpgradeText(string upgradeText)
    {
        UpgradeText.text = upgradeText;
    }

    public void SetCardBannerColor(Color color)
    {
        ClientUtils.ChangeColor(NewCardBanner, color);
    }

    public void SetUpgradeArrowColor(Color color)
    {
        ClientUtils.ChangeColor(UpgradeArrow, color);
    }

    void Awake()
    {
        NewCardBannerTextDefaultSortingOrder = NewCardBannerText.sortingOrder;
        NewCardBannerDefaultSortingOrder = NewCardBanner.sortingOrder;
        UpgradeTextDefaultSortingOrder = UpgradeText.sortingOrder;
        UpgradeArrowDefaultSortingOrder = UpgradeArrow.sortingOrder;
    }

    private int NewCardBannerTextDefaultSortingOrder;
    private int NewCardBannerDefaultSortingOrder;
    private int UpgradeTextDefaultSortingOrder;
    private int UpgradeArrowDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        NewCardBannerText.sortingOrder = cardSortingIndex * 50 + NewCardBannerTextDefaultSortingOrder;
        NewCardBanner.sortingOrder = cardSortingIndex * 50 + NewCardBannerDefaultSortingOrder;
        UpgradeText.sortingOrder = cardSortingIndex * 50 + UpgradeTextDefaultSortingOrder;
        UpgradeArrow.sortingOrder = cardSortingIndex * 50 + UpgradeArrowDefaultSortingOrder;
    }
}
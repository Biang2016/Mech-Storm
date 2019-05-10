using UnityEngine;
using UnityEngine.Rendering;

public class RetinueAttrShapesComponent : RetinueComponentBase
{
    [SerializeField] private MeshRenderer Defence;
    [SerializeField] private MeshRenderer DefenceHover;
    [SerializeField] private MeshRenderer Sentry;
    [SerializeField] private MeshRenderer SentryHover;
    [SerializeField] private MeshRenderer Sniper;
    [SerializeField] private MeshRenderer SniperHover;
    [SerializeField] private MeshRenderer Frenzy;
    [SerializeField] private MeshRenderer FrenzyHover;

    [SerializeField] private SortingGroup DefenceSG;
    [SerializeField] private SortingGroup DefenceHoverSG;
    [SerializeField] private SortingGroup SentrySG;
    [SerializeField] private SortingGroup SentryHoverSG;
    [SerializeField] private SortingGroup SniperSG;
    [SerializeField] private SortingGroup SniperHoverSG;
    [SerializeField] private SortingGroup FrenzySG;
    [SerializeField] private SortingGroup FrenzyHoverSG;

    void Awake()
    {
        HideAll();
        DefenceSortingOrder = DefenceSG.sortingOrder;
        SentrySortingOrder = SentrySG.sortingOrder;
        SniperSortingOrder = SniperSG.sortingOrder;
        FrenzySortingOrder = FrenzySG.sortingOrder;
    }

    public void HideAll()
    {
        ShowDefence(false);
        ShowSentry(false);
        ShowSniper(false);
        ShowFrenzy(false);
    }

    public void ShowDefence(bool isShow)
    {
        Defence.gameObject.SetActive(isShow);
        DefenceHover.gameObject.SetActive(false);
    }

    public void HoverDefence()
    {
        Defence.gameObject.SetActive(true);
        DefenceHover.gameObject.SetActive(true);
    }

    public void ShowSentry(bool isShow)
    {
        Sentry.gameObject.SetActive(isShow);
        SentryHover.gameObject.SetActive(false);
    }

    public void HoverSentry()
    {
        Sentry.gameObject.SetActive(true);
        SentryHover.gameObject.SetActive(true);
    }

    public void ShowSniper(bool isShow)
    {
        Sniper.gameObject.SetActive(isShow);
        SniperHover.gameObject.SetActive(false);
    }

    public void HoverSniper()
    {
        Sniper.gameObject.SetActive(true);
        SniperHover.gameObject.SetActive(true);
    }

    public void ShowFrenzy(bool isShow)
    {
        Frenzy.gameObject.SetActive(isShow);
        Frenzy.gameObject.SetActive(false);
    }

    public void HoverFrenzy()
    {
        Frenzy.gameObject.SetActive(true);
        Frenzy.gameObject.SetActive(true);
    }

    private int DefenceSortingOrder;
    private int SentrySortingOrder;
    private int SniperSortingOrder;
    private int FrenzySortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        DefenceSG.sortingOrder = cardSortingIndex * 50 + DefenceSortingOrder;
        DefenceHoverSG.sortingOrder = cardSortingIndex * 50 + DefenceSortingOrder;
        SentrySG.sortingOrder = cardSortingIndex * 50 + SentrySortingOrder;
        SentryHoverSG.sortingOrder = cardSortingIndex * 50 + SentrySortingOrder;
        SniperSG.sortingOrder = cardSortingIndex * 50 + SniperSortingOrder;
        SniperHoverSG.sortingOrder = cardSortingIndex * 50 + SniperSortingOrder;
        FrenzySG.sortingOrder = cardSortingIndex * 50 + FrenzySortingOrder;
        FrenzyHoverSG.sortingOrder = cardSortingIndex * 50 + FrenzySortingOrder;
    }
}
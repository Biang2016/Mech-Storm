using TMPro;
using UnityEngine;

public class CardSelectCountComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro SelectCountText;
    [SerializeField] private TextMeshPro SelectLimitText;
    [SerializeField] private TextMeshPro SlashText;

    public void SetSelectNum(int count)
    {
        SelectCountText.text = count.ToString();
    }

    public void SetSelectLimitNum(int limit)
    {
        SelectLimitText.text = limit.ToString();
    }

    void Awake()
    {
        SelectCountTextDefaultSortingOrder = SelectCountText.sortingOrder;
        SelectLimitTextDefaultSortingOrder = SelectLimitText.sortingOrder;
        SlashTextDefaultSortingOrder = SlashText.sortingOrder;
    }

    private int SelectCountTextDefaultSortingOrder;
    private int SelectLimitTextDefaultSortingOrder;
    private int SlashTextDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        SelectCountText.sortingOrder = cardSortingIndex * 50 + SelectCountTextDefaultSortingOrder;
        SelectLimitText.sortingOrder = cardSortingIndex * 50 + SelectLimitTextDefaultSortingOrder;
        SlashText.sortingOrder = cardSortingIndex * 50 + SlashTextDefaultSortingOrder;
    }
}
using TMPro;
using UnityEngine;

public class CardTypeComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro CardTypeText;

    public void SetText(string text)
    {
        CardTypeText.text = text;
    }

    void Awake()
    {
        CardTypeTextDefaultSortingOrder = CardTypeText.sortingOrder;
    }

    private int CardTypeTextDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardTypeText.sortingOrder = cardSortingIndex * 50 + CardTypeTextDefaultSortingOrder;
    }
}
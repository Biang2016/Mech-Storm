using TMPro;
using UnityEngine;

public class CardDescComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private SpriteRenderer CardNameBG;
    [SerializeField] private TextMeshPro CardDescText;
    [SerializeField] private SpriteRenderer CardDescBG;
    [SerializeField] private TextMeshPro CardTypeText;

    public void SetCardName(string cardName)
    {
        CardNameText.text = cardName;
    }

    public void SetDescText(string desc)
    {
        CardDescText.text = desc;
    }

    public void SetCardNameBGColor(Color color)
    {
        ClientUtils.ChangeColor(CardNameBG, color);
    }

    public void SetCardDescBGColor(Color color)
    {
        ClientUtils.ChangeColor(CardDescBG, color);
    }

    public void SetCardDescTextColor(Color color)
    {
        CardDescText.color = color;
    }

    public void SetCardTypeText(string text)
    {
        CardTypeText.text = text;
    }

    public void SetCardTypeTextColor(Color color)
    {
        CardTypeText.color = color;
    }

    void Awake()
    {
        CardNameTextDefaultSortingOrder = CardNameText.sortingOrder;
        CardNameBGDefaultSortingOrder = CardNameBG.sortingOrder;
        CardDescTextDefaultSortingOrder = CardDescText.sortingOrder;
        CardDescBGDefaultSortingOrder = CardDescBG.sortingOrder;
        CardTypeTextDefaultSortingOrder = CardTypeText.sortingOrder;
    }

    private int CardNameTextDefaultSortingOrder;
    private int CardNameBGDefaultSortingOrder;
    private int CardDescTextDefaultSortingOrder;
    private int CardDescBGDefaultSortingOrder;
    private int CardTypeTextDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardNameText.sortingOrder = cardSortingIndex * 50 + CardNameTextDefaultSortingOrder;
        CardNameBG.sortingOrder = cardSortingIndex * 50 + CardNameBGDefaultSortingOrder;
        CardDescText.sortingOrder = cardSortingIndex * 50 + CardDescTextDefaultSortingOrder;
        CardDescBG.sortingOrder = cardSortingIndex * 50 + CardDescBGDefaultSortingOrder;
        CardTypeText.sortingOrder = cardSortingIndex * 50 + CardTypeTextDefaultSortingOrder;
    }
}
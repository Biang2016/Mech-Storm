using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro CardNameText;
    [SerializeField] private SpriteRenderer CardNameBG;
    [SerializeField] private TextMeshPro CardDescText;
    [SerializeField] private SpriteRenderer CardDescBG;

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

    void Awake()
    {
        CardNameTextDefaultSortingOrder = CardNameText.sortingOrder;
        CardNameBGDefaultSortingOrder = CardNameBG.sortingOrder;
        CardDescTextDefaultSortingOrder = CardDescText.sortingOrder;
        CardDescBGDefaultSortingOrder = CardDescBG.sortingOrder;
    }

    private int CardNameTextDefaultSortingOrder;
    private int CardNameBGDefaultSortingOrder;
    private int CardDescTextDefaultSortingOrder;
    private int CardDescBGDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardNameText.sortingOrder = cardSortingIndex * 50 + CardNameTextDefaultSortingOrder;
        CardNameBG.sortingOrder = cardSortingIndex * 50 + CardNameBGDefaultSortingOrder;
        CardDescText.sortingOrder = cardSortingIndex * 50 + CardDescTextDefaultSortingOrder;
        CardDescBG.sortingOrder = cardSortingIndex * 50 + CardDescBGDefaultSortingOrder;
    }
}
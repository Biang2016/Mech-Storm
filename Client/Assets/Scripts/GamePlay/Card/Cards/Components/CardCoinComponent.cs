using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCoinComponent : CardComponentBase
{
    [SerializeField] private Transform UpperPivot;
    [SerializeField] private Transform LowerPivot;
    [SerializeField] private Transform Panel;
    [SerializeField] private TextMeshPro CoinText;
    [SerializeField] private SpriteRenderer CoinImage;
    [SerializeField] private SpriteRenderer CoinImageBG;

    public enum Position
    {
        Upper,
        Lower
    }

    public void SetPosition(Position pos)
    {
        switch (pos)
        {
            case Position.Upper:
                Panel.position = UpperPivot.position;
                break;
            case Position.Lower:
                Panel.position = LowerPivot.position;
                break;
        }
    }

    public void SetCoin(int coinNumber)
    {
        CoinText.text = coinNumber.ToString();
    }

    void Awake()
    {
        CoinTextDefaultSortingOrder = CoinText.sortingOrder;
        CoinImageDefaultSortingOrder = CoinImage.sortingOrder;
        CoinImageBGDefaultSortingOrder = CoinImageBG.sortingOrder;
    }

    private int CoinTextDefaultSortingOrder;
    private int CoinImageDefaultSortingOrder;
    private int CoinImageBGDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CoinText.sortingOrder = cardSortingIndex * 50 + CoinTextDefaultSortingOrder;
        CoinImage.sortingOrder = cardSortingIndex * 50 + CoinImageDefaultSortingOrder;
        CoinImageBG.sortingOrder = cardSortingIndex * 50 + CoinImageBGDefaultSortingOrder;
    }
}
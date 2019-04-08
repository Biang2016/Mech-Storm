using UnityEngine;

public class CardBackComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBack;
    [SerializeField] private MeshRenderer CardBloom;

    public void SetCardBackColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBack, color, intensity);
    }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
    }

    public void SetBloomShow(bool isShow)
    {
        CardBloom.gameObject.SetActive(isShow);
    }

    void Awake()
    {
        CardBackDefaultSortingOrder = CardBack.sortingOrder;
        CardBloomDefaultSortingOrder = CardBloom.sortingOrder;
    }

    private int CardBackDefaultSortingOrder;
    private int CardBloomDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardBack.sortingOrder = cardSortingIndex * 50 + CardBackDefaultSortingOrder;
        CardBack.sortingOrder = cardSortingIndex * 50 + CardBloomDefaultSortingOrder;
    }
}
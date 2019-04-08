using UnityEngine;
using UnityEngine.Rendering;

public class CardBackComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBack;
    [SerializeField] private MeshRenderer CardBloom;
    [SerializeField] private SortingGroup CardBackSG;
    [SerializeField] private SortingGroup CardBloomSG;

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
        CardBackDefaultSortingOrder = CardBackSG.sortingOrder;
        CardBloomDefaultSortingOrder = CardBloomSG.sortingOrder;
    }

    private int CardBackDefaultSortingOrder;
    private int CardBloomDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardBackSG.sortingOrder = cardSortingIndex * 50 + CardBackDefaultSortingOrder;
        CardBloomSG.sortingOrder = cardSortingIndex * 50 + CardBloomDefaultSortingOrder;
    }
}
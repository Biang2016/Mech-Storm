using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CardBackComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBack;
    [SerializeField] private MeshRenderer CardBloom;
    [SerializeField] private SortingGroup CardBackSG;
    [SerializeField] private SortingGroup CardBloomSG;

    public Color CardBackColor { get; set; }

    public float CardBackColorIntensity { get; set; }

    public void SetCardBackColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBack, color, intensity);
        CardBackColor = color;
        CardBackColorIntensity = intensity;
    }

    public Color CardBackBloomColor { get; set; }

    public float CardBackBloomColorIntensity { get; set; }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
        CardBackBloomColor = color;
        CardBackBloomColorIntensity = intensity;
    }

    public void SetBloomShow(bool isShow)
    {
        CardBloom.gameObject.SetActive(isShow);
    }

    void Awake()
    {
        CardBackColorIntensity = 1.5f;
        CardBackColor = CardBack.sharedMaterial.GetColor("_EmissionColor");

        CardBackBloomColorIntensity = 0f;
        CardBackBloomColor = CardBloom.sharedMaterial.GetColor("_EmissionColor");

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
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardLifeComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro LifeText;
    [SerializeField] private MeshRenderer LifeIcon;
    [SerializeField] private SortingGroup LifeIconSG;

    public void SetLife(int lifeValue)
    {
        LifeText.text = lifeValue.ToString();
    }

    public void ChangeLifeIconColor(Color color)
    {
        ClientUtils.ChangeColor(LifeIcon, color);
    }

    void Awake()
    {
        LifeTextDefaultSortingOrder = LifeText.sortingOrder;
        LifeIconDefaultSortingOrder = LifeIconSG.sortingOrder;
    }

    private int LifeTextDefaultSortingOrder;
    private int LifeIconDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        LifeText.sortingOrder = cardSortingIndex * 50 + LifeTextDefaultSortingOrder;
        LifeIconSG.sortingOrder = cardSortingIndex * 50 + LifeIconDefaultSortingOrder;
    }
}
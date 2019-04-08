using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardLifeComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro LifeText;
    [SerializeField] private MeshRenderer LifeIcon;

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
        LifeIconDefaultSortingOrder = LifeIcon.sortingOrder;
    }

    private int LifeTextDefaultSortingOrder;
    private int LifeIconDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        LifeText.sortingOrder = cardSortingIndex * 50 + LifeTextDefaultSortingOrder;
        LifeIcon.sortingOrder = cardSortingIndex * 50 + LifeIconDefaultSortingOrder;
    }
}
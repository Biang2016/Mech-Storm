using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class RetinueLifeIconComponent : RetinueComponentBase
{
    [SerializeField] private TextMeshPro LifeText;
    [SerializeField] private MeshRenderer LifeIcon;
    [SerializeField] private Animator LifeIconAnim;
    [SerializeField] private SortingGroup LifeIconSG;

    private int m_LifeValue = -1;

    public void SetLife(int lifeValue)
    {
        if (m_LifeValue != lifeValue)
        {
            LifeIconAnim.SetTrigger("Jump");
            m_LifeValue = lifeValue;
        }

        LifeText.text = lifeValue.ToString();
    }

    public void ChangeLifeIconColor(Color color)
    {
        ClientUtils.ChangeColor(LifeIcon, color);
    }

    void Awake()
    {
        SetLife(0);
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
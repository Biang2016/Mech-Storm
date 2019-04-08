using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCostIconComponent : CardComponentBase
{
    [SerializeField] private GameObject EnergyBlock;
    [SerializeField] private TextMeshPro EnergyText;
    [SerializeField] private MeshRenderer EnergyIcon;
    [SerializeField] private GameObject MetalBlock;
    [SerializeField] private TextMeshPro MetalText;
    [SerializeField] private MeshRenderer MetalIcon;

    public void SetEnergy(int energyValue)
    {
        EnergyBlock.SetActive(energyValue != 0);
        EnergyText.text = energyValue.ToString();
    }

    public void SetMetal(int metalValue)
    {
        MetalBlock.SetActive(metalValue != 0);
        MetalText.text = metalValue.ToString();
    }

    void Awake()
    {
        EnergyTextDefaultSortingOrder = EnergyText.sortingOrder;
        EnergyIconDefaultSortingOrder = EnergyIcon.sortingOrder;
        MetalTextDefaultSortingOrder = MetalText.sortingOrder;
        MetalIconDefaultSortingOrder = MetalIcon.sortingOrder;
    }

    private int EnergyTextDefaultSortingOrder;
    private int EnergyIconDefaultSortingOrder;
    private int MetalTextDefaultSortingOrder;
    private int MetalIconDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        EnergyText.sortingOrder = cardSortingIndex * 50 + EnergyTextDefaultSortingOrder;
        EnergyIcon.sortingOrder = cardSortingIndex * 50 + EnergyIconDefaultSortingOrder;
        MetalText.sortingOrder = cardSortingIndex * 50 + MetalTextDefaultSortingOrder;
        MetalIcon.sortingOrder = cardSortingIndex * 50 + MetalIconDefaultSortingOrder;
    }
}
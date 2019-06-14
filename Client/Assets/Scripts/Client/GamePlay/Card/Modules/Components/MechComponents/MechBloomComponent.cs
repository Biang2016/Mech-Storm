using UnityEngine;
using UnityEngine.Rendering;

public class MechBloomComponent : MechComponentBase
{
    [SerializeField] private MeshRenderer OnHoverBloom;
    [SerializeField] private MeshRenderer CanAttackBloom;
    [SerializeField] private MeshRenderer SideEffectTriggerBloom;
    [SerializeField] private SortingGroup OnHoverBloomSG;
    [SerializeField] private SortingGroup CanAttackBloomSG;
    [SerializeField] private SortingGroup SideEffectTriggerBloomSG;

    public Color OnHoverBloomColor { get; set; }

    public float OnHoverBloomColorIntensity { get; set; }

    public void SetOnHoverBloomColor(Color color, float intensity = 1.0f)
    {
        ClientUtils.ChangeColor(OnHoverBloom, color, intensity);
        OnHoverBloomColor = color;
        OnHoverBloomColorIntensity = intensity;
    }

    public Color CanAttackBloomColor { get; set; }

    public float CanAttackBloomColorIntensity { get; set; }

    public void SetCanAttackBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CanAttackBloom, color, intensity);
        CanAttackBloomColor = color;
        CanAttackBloomColorIntensity = intensity;
    }

    public Color SideEffectTriggerBloomColor { get; set; }

    public float SideEffectTriggerBloomColorIntensity { get; set; }

    public void SetSideEffectTriggerBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(SideEffectTriggerBloom, color, intensity);
        SideEffectTriggerBloomColor = color;
        SideEffectTriggerBloomColorIntensity = intensity;
    }

    public void SetOnHoverBloomShow(bool isShow)
    {
        OnHoverBloom.gameObject.SetActive(isShow);
    }

    public void SetCanAttackBloomShow(bool isShow)
    {
        CanAttackBloom.gameObject.SetActive(isShow);
    }

    public void SetSideEffectTriggerBloomShow(bool isShow)
    {
        SideEffectTriggerBloom.gameObject.SetActive(isShow);
    }

    void Awake()
    {
        Reset();
        OnHoverBloomColorIntensity = 1.5f;
        CanAttackBloomColorIntensity = 1.5f;
        SideEffectTriggerBloomColorIntensity = 1.5f;
        OnHoverBloomColor = OnHoverBloom.sharedMaterial.GetColor("_EmissionColor");
        CanAttackBloomColor = CanAttackBloom.sharedMaterial.GetColor("_EmissionColor");
        SideEffectTriggerBloomColor = SideEffectTriggerBloom.sharedMaterial.GetColor("_EmissionColor");
        OnHoverBloomDefaultSortingOrder = OnHoverBloomSG.sortingOrder;
        CanAttackBloomDefaultSortingOrder = CanAttackBloomSG.sortingOrder;
        SideEffectTriggerBloomDefaultSortingOrder = SideEffectTriggerBloomSG.sortingOrder;
    }

    private int OnHoverBloomDefaultSortingOrder;
    private int CanAttackBloomDefaultSortingOrder;
    private int SideEffectTriggerBloomDefaultSortingOrder;

    protected override void Child_Initialize()
    {
        SetOnHoverBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.MechOnEnemyHoverBloomColor), 2f);
        SetCanAttackBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.MechBloomColor), 2f);
    }

    protected override void Reset()
    {
        SetOnHoverBloomShow(false);
        SetCanAttackBloomShow(false);
        SetSideEffectTriggerBloomShow(false);
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        OnHoverBloomSG.sortingOrder = cardSortingIndex * 50 + OnHoverBloomDefaultSortingOrder;
        CanAttackBloomSG.sortingOrder = cardSortingIndex * 50 + CanAttackBloomDefaultSortingOrder;
        SideEffectTriggerBloomSG.sortingOrder = cardSortingIndex * 50 + SideEffectTriggerBloomDefaultSortingOrder;
    }
}
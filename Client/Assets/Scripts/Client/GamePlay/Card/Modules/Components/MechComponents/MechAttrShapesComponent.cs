using UnityEngine;
using UnityEngine.Rendering;

public class MechAttrShapesComponent : MechComponentBase
{
    [SerializeField] private MeshRenderer Defender;
    [SerializeField] private MeshRenderer DefenderHover;
    [SerializeField] private MeshRenderer Sentry;
    [SerializeField] private MeshRenderer SentryHover;
    [SerializeField] private MeshRenderer Sniper;
    [SerializeField] private MeshRenderer SniperHover;
    [SerializeField] private MeshRenderer Frenzy;
    [SerializeField] private MeshRenderer FrenzyHover;

    [SerializeField] private SortingGroup DefenderSG;
    [SerializeField] private SortingGroup DefenderHoverSG;
    [SerializeField] private SortingGroup SentrySG;
    [SerializeField] private SortingGroup SentryHoverSG;
    [SerializeField] private SortingGroup SniperSG;
    [SerializeField] private SortingGroup SniperHoverSG;
    [SerializeField] private SortingGroup FrenzySG;
    [SerializeField] private SortingGroup FrenzyHoverSG;

    protected override void Child_Initialize()
    {
        OnAttrShapeShow();
    }

    public void OnMousePressEnterImmediately()
    {
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleMech mr = DragManager.Instance.CurrentDrag_ModuleMech;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null && Mech.CheckModuleMechCanAttackMe(mr))
            {
                OnAttrShapeHoverShow();
            }
            else if (cs != null && Mech.CheckCardSpellCanTarget(cs))
            {
                OnAttrShapeHoverShow();
            }
        }
    }

    protected override void Reset()
    {
        ShowDefender(false);
        ShowSentry(false);
        ShowSniper(false);
        ShowFrenzy(false);
    }

    public void OnAttrShapeShow()
    {
        ShowDefender(Mech.IsDefender);
        ShowSentry(Mech.IsSentry);
        ShowSniper(Mech.IsSniper);
        ShowFrenzy(Mech.IsFrenzy);
    }

    public void OnAttrShapeHoverShow()
    {
        HoverDefender(Mech.IsDefender);
        HoverSentry(Mech.IsSentry);
        HoverSniper(Mech.IsSniper);
        HoverFrenzy(Mech.IsFrenzy);
    }

    void Awake()
    {
        Reset();
        DefenceSortingOrder = DefenderSG.sortingOrder;
        SentrySortingOrder = SentrySG.sortingOrder;
        SniperSortingOrder = SniperSG.sortingOrder;
        FrenzySortingOrder = FrenzySG.sortingOrder;
    }

    public void ShowDefender(bool isShow)
    {
        Defender.gameObject.SetActive(isShow);
        DefenderHover.gameObject.SetActive(false);
    }

    public void HoverDefender(bool isDenfender)
    {
        if (isDenfender)
        {
            Defender.gameObject.SetActive(false);
            DefenderHover.gameObject.SetActive(true);
        }
    }

    public void ShowSentry(bool isShow)
    {
        Sentry.gameObject.SetActive(isShow);
        SentryHover.gameObject.SetActive(false);
    }

    public void HoverSentry(bool isSentry)
    {
        if (isSentry)
        {
            Sentry.gameObject.SetActive(false);
            SentryHover.gameObject.SetActive(true);
        }
    }

    public void ShowSniper(bool isShow)
    {
        Sniper.gameObject.SetActive(isShow);
        SniperHover.gameObject.SetActive(false);
    }

    public void HoverSniper(bool isSniper)
    {
        if (isSniper)
        {
            Sniper.gameObject.SetActive(false);
            SniperHover.gameObject.SetActive(true);
        }
    }

    public void ShowFrenzy(bool isShow)
    {
        Frenzy.gameObject.SetActive(isShow);
        FrenzyHover.gameObject.SetActive(false);
    }

    public void HoverFrenzy(bool isFrenzy)
    {
        if (isFrenzy)
        {
            Frenzy.gameObject.SetActive(false);
            FrenzyHover.gameObject.SetActive(true);
        }
    }

    private int DefenceSortingOrder;
    private int SentrySortingOrder;
    private int SniperSortingOrder;
    private int FrenzySortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        DefenderSG.sortingOrder = cardSortingIndex * 50 + DefenceSortingOrder;
        DefenderHoverSG.sortingOrder = cardSortingIndex * 50 + DefenceSortingOrder;
        SentrySG.sortingOrder = cardSortingIndex * 50 + SentrySortingOrder;
        SentryHoverSG.sortingOrder = cardSortingIndex * 50 + SentrySortingOrder;
        SniperSG.sortingOrder = cardSortingIndex * 50 + SniperSortingOrder;
        SniperHoverSG.sortingOrder = cardSortingIndex * 50 + SniperSortingOrder;
        FrenzySG.sortingOrder = cardSortingIndex * 50 + FrenzySortingOrder;
        FrenzyHoverSG.sortingOrder = cardSortingIndex * 50 + FrenzySortingOrder;
    }
}
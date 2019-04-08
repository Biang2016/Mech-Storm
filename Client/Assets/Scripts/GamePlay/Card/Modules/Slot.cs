using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleRetinue M_ModuleRetinue;

    [SerializeField] private MeshRenderer SlotLight;
    [SerializeField] private MeshRenderer SlotBloom;
    [SerializeField] private SortingGroup SlotLightSG;
    [SerializeField] private SortingGroup SlotBloomSG;

    void Awake()
    {
        SlotLightDefaultSortingOrder = SlotLightSG.sortingOrder;
        SlotBloomDefaultSortingOrder = SlotBloomSG.sortingOrder;
    }

    private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            ClientUtils.ChangeSlotColor(SlotLight, value);
            ClientUtils.ChangeSlotColor(SlotBloom, value);
            SlotLight.gameObject.SetActive(value != SlotTypes.None);
            SlotBloom.gameObject.SetActive(value != SlotTypes.None);
        }
    }

    public void ShowSlotBloom(bool isSniper = false)
    {
        SlotBloom.gameObject.SetActive(true);

        if (isSniper && M_ModuleRetinue)
        {
            if (MSlotTypes == SlotTypes.Weapon && M_ModuleRetinue.CardInfo.RetinueInfo.IsSniper)
            {
                M_ModuleRetinue.ShowSniperTipText();
            }
        }
    }

    public void HideSlotBloom()
    {
        SlotBloom.gameObject.SetActive(false);
        M_ModuleRetinue?.HideSniperTipText();
    }

    public void ShowSlotLight(bool isShow)
    {
        SlotLight.gameObject.SetActive(isShow);
    }

    private int SlotLightDefaultSortingOrder;
    private int SlotBloomDefaultSortingOrder;

    public void SetSortingLayer(int index)
    {
        SlotLightSG.sortingOrder = index * 50 + SlotLightDefaultSortingOrder;
        SlotBloomSG.sortingOrder = index * 50 + SlotBloomDefaultSortingOrder;
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            CardEquip cw = DragManager.Instance.CurrentDrag_CardEquip;
            if (cw && cw.ClientPlayer == ClientPlayer)
            {
                if (_mSlotTypes != SlotTypes.None) ShowSlotBloom();
            }
        }
    }

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover1End()
    {
        HideSlotBloom();
    }

    public void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover2End()
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        HideSlotBloom();
    }
}
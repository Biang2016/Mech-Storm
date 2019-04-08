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
            if (value != SlotTypes.None)
            {
                SlotLight.enabled = true;
            }
            else
            {
                SlotLight.enabled = false;
            }
        }
    }

    public void ShowHoverGO(bool isSniper = false)
    {
        if (SlotBloom)
        {
            SlotBloom.enabled = true;
        }

        if (isSniper && M_ModuleRetinue)
        {
            if (MSlotTypes == SlotTypes.Weapon && M_ModuleRetinue.CardInfo.RetinueInfo.IsSniper)
            {
                M_ModuleRetinue.ShowSniperTipText();
            }
        }
    }

    public void HideHoverShowGO()
    {
        if (SlotBloom)
        {
            SlotBloom.enabled = false;
        }

        if (M_ModuleRetinue) M_ModuleRetinue.HideSniperTipText();
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
                if (_mSlotTypes != SlotTypes.None) ShowHoverGO();
            }
        }
    }

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover1End()
    {
        HideHoverShowGO();
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
        HideHoverShowGO();
    }
}
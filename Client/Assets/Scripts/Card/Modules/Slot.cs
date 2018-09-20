using UnityEngine;

public class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleRetinue M_ModuleRetinue;

    [SerializeField] private Renderer My_Renderer;
    [SerializeField] private Renderer OnHoverShowBloom;
    [SerializeField] private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            ClientUtils.ChangeSlotColor(My_Renderer, value);
            ClientUtils.ChangeSlotColor(OnHoverShowBloom, value);
            if (value != SlotTypes.None)
            {
                My_Renderer.enabled = true;
            }
            else
            {
                My_Renderer.enabled = false;
            }
        }
    }

    public void ShowHoverGO()
    {
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = true;
        }
    }

    public void HideHoverShowGO()
    {
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = false;
        }
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            CardEquip cw = DragManager.Instance.CurrentDrag_CardEquip;
            if (cw && cw.ClientPlayer == ClientPlayer)
            {
                ShowHoverGO();
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
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleRetinue M_ModuleRetinue;

    [SerializeField] private RawImage My_Renderer;
    [SerializeField] private RawImage OnHoverShowBloom;
    private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            ClientUtils.ChangeSlotColor(My_Renderer, value, 1.5f);
            ClientUtils.ChangeSlotColor(OnHoverShowBloom, value, 1.5f);
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

    public void ShowHoverGO(bool isSniper = false)
    {
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = true;
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
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = false;
        }

        if (M_ModuleRetinue) M_ModuleRetinue.HideSniperTipText();
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
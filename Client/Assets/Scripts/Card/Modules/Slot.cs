﻿using UnityEngine;

public class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleRetinue M_ModuleRetinue;

    [SerializeField] private Renderer OnHoverShowBloom;
    [SerializeField] private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            ChangeSlotColor(value);
            if (value != SlotTypes.None)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void ChangeSlotColor(SlotTypes slotTypes)
    {
        Renderer rd = GetComponent<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        switch (slotTypes)
        {
            case SlotTypes.Weapon:
                mpb.SetColor("_Color", GameManager.Instance.Slot1Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot1Color);
                break;
            case SlotTypes.Shield:
                mpb.SetColor("_Color", GameManager.Instance.Slot2Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot2Color);
                break;
            case SlotTypes.Pack:
                mpb.SetColor("_Color", GameManager.Instance.Slot3Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot3Color);
                break;
            case SlotTypes.MA:
                mpb.SetColor("_Color", GameManager.Instance.Slot4Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot4Color);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }

        rd.SetPropertyBlock(mpb);
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
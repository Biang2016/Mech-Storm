using UnityEngine;
using System.Collections;
using System;

internal class SlotAnchor : MonoBehaviour, IMouseHoverComponent
{
    public Slot M_Slot;
    internal ModuleRetinue M_ModuleRetinue;

    private void Awake()
    {
        M_Slot = GetComponent<Slot>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    [SerializeField]private Renderer OnHoverShowBloom;

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
        if (DragManager.DM.CurrentDrag)
        {
            switch (M_Slot.MSlotTypes)
            {
                case SlotTypes.Weapon:
                    CardWeapon cw = DragManager.DM.CurrentDrag_CardWeapon;
                    if (cw && cw.ClientPlayer == M_Slot.ClientPlayer)
                    {
                        ShowHoverGO();
                    }

                    break;
                case SlotTypes.Shield:
                    CardShield cs = DragManager.DM.CurrentDrag_CardShield;
                    if (cs && cs.ClientPlayer == M_Slot.ClientPlayer)
                    {
                        ShowHoverGO();
                    }

                    break;
            }
        }
    }

    public void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseEnter(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseOver()
    {
    }

    public void MouseHoverComponent_OnMouseLeave()
    {
    }

    public void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        HideHoverShowGO();
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        HideHoverShowGO();
    }
}
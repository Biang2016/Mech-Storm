using UnityEngine;
using System.Collections;
using System;

public class SlotAnchor : MonoBehaviour,IMouseHoverComponent
{
    public Player Player;
    public SlotType M_SlotType = SlotType.None;
    internal ModuleRetinue M_ModuleRetinue;

    private void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    public GameObject OnHoverShowGO;

    public void ShowHoverGO()
    {
        if (OnHoverShowGO) {
            OnHoverShowGO.SetActive(true);
        }
    }

    public void HideHoverShowGO()
    {
        if (OnHoverShowGO) {
            OnHoverShowGO.SetActive(false);
        }
    }


    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.DM.CurrentDrag)
        {
            switch (M_SlotType)
            {
                case SlotType.Weapon:
                    CardWeapon cw = DragManager.DM.CurrentDrag.GetComponent<CardWeapon>();
                    if (cw&&cw.Player==Player)
                    {
                        ShowHoverGO();
                    }

                    break;
                case SlotType.Shield:
                    CardShield cs = DragManager.DM.CurrentDrag.GetComponent<CardShield>();
                    if (cs&&cs.Player==Player)
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

public enum SlotType
{
    None=0,
    Weapon=1,
    Shield=2,
    Pack=3,
    MA=4,
}
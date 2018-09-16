using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ship : MonoBehaviour,IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;

    [SerializeField]private GameObject ShipBG;

    [SerializeField] private TextMesh DamageNumberPreviewTextMesh;
    [SerializeField] private TextMesh DamageNumberPreviewBGTextMesh;

    void Awake()
    {
        ShipBG.SetActive(false);
        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        ShipBG.SetActive(true);
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.Instance.CurrentDrag_ModuleRetinue;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null)
            {
                if (mr.ClientPlayer != ClientPlayer && mr != this)
                {
                    if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                    {
                        ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                    }

                    DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                    DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                }
            }
            else if (cs != null)
            {
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
            }
        }
    }

    public void MouseHoverComponent_OnHoverBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
    }

    public void MouseHoverComponent_OnMouseLeave()
    {
    }

    public void MouseHoverComponent_OnHoverEnd()
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        ShipBG.SetActive(false);
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }

        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
    }
}
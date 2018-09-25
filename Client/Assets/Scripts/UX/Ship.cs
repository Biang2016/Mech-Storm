using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ship : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private GameObject ShipBG;

    [SerializeField] private TextMesh DamageNumberPreviewTextMesh;
    [SerializeField] private TextMesh DamageNumberPreviewBGTextMesh;

    void Awake()
    {
        ShipBG.SetActive(false);
        if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = "";
        if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = "";
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.Instance.CurrentDrag_ModuleRetinue;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null && CheckModuleRetinueCanAttack(mr))
            {
                ShipBG.SetActive(true);
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
            }
            else if (cs != null && CheckCardSpellCanTarget(cs))
            {
                ShipBG.SetActive(true);
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
            }
        }
    }

    private bool CheckModuleRetinueCanAttack(ModuleRetinue retinue)
    {
        //Todo 嘲讽类随从等逻辑
        if (retinue.ClientPlayer == ClientPlayer)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return card.targetRetinueRange == TargetSideEffect.TargetRange.All ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.Ships ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.SelfShip;
        }
        else
        {
            return card.targetRetinueRange == TargetSideEffect.TargetRange.All ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.Ships ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.EnemyShip;
        }
    }

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover1End()
    {
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
        ShipBG.SetActive(false);
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }

        if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = "";
        if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = "";
    }
}
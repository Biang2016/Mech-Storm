using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MouseHoverUI : MonoBehaviour, IMouseHoverComponent
{
    [SerializeField] private Animator Anim;

    private bool isHover;


    public void MouseHoverComponent_OnHoverBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHoverEnd()
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
        if (isHover) return;
        Anim.SetTrigger("OnHover");
        isHover = true;
        AudioManager.Instance.SoundPlay("sfx/SwitchButton");
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
        if (!isHover) return;
        Anim.SetTrigger("Reset");
        isHover = false;
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MouseHoverUI : MonoBehaviour, IMouseHoverComponent
{
    [SerializeField] private Animator Anim;
    [SerializeField] private string SFX;
    [SerializeField] [Range(0, 1)] private float volumn;

    private bool isHover;


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
        if (isHover) return;
        Anim.SetTrigger("OnHover");
        isHover = true;
        AudioManager.Instance.SoundPlay("sfx/" + SFX, volumn);
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
        if (!isHover) return;
        Anim.SetTrigger("OnExit");
        isHover = false;
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
    }
}
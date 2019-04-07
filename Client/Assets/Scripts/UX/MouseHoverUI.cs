using UnityEngine;

internal class MouseHoverUI : MonoBehaviour, IMouseHoverComponent
{
    [SerializeField] private Animator Anim;
    [SerializeField] private string SFX;
    [SerializeField] [Range(0, 1)] private float volume;

    private bool isHover;

    public MouseHover1Begin MouseHover1BeginHandler;
    public MouseHover1End MouseHover1EndHandler;
    public MouseHover2Begin MouseHover2BeginHandler;
    public MouseHover2End MouseHover2EndHandler;
    public MouseFocusBegin MouseFocusBeginHandler;
    public MouseFocusEnd MouseFocusEndHandler;
    public MousePressEnter MousePressEnterHandler;
    public MousePressLeave MousePressLeaveHandler;

    public delegate void MouseHover1Begin();

    public delegate void MouseHover1End();

    public delegate void MouseHover2Begin();

    public delegate void MouseHover2End();

    public delegate void MouseFocusBegin();

    public delegate void MouseFocusEnd();

    public delegate void MousePressEnter();

    public delegate void MousePressLeave();

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        if (MouseHover1BeginHandler != null) MouseHover1BeginHandler();
    }

    public void MouseHoverComponent_OnHover1End()
    {
        if (MouseHover1EndHandler != null) MouseHover1EndHandler();
    }

    public void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
        if (MouseHover2BeginHandler != null) MouseHover2BeginHandler();
    }

    public void MouseHoverComponent_OnHover2End()
    {
        if (MouseHover2EndHandler != null) MouseHover2EndHandler();
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
        if (isHover) return;
        if (MouseFocusBeginHandler != null) MouseFocusBeginHandler();
        Anim.SetTrigger("OnHover");
        isHover = true;
        AudioManager.Instance.SoundPlay("sfx/" + SFX, volume);
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
        if (!isHover) return;
        if (MouseFocusEndHandler != null) MouseFocusEndHandler();
        Anim.SetTrigger("OnExit");
        isHover = false;
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (MousePressEnterHandler != null) MousePressEnterHandler();
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        if (MousePressLeaveHandler != null) MousePressLeaveHandler();
    }
}
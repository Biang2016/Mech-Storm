using UnityEngine;

public class MouseHoverComponent : MonoBehaviour
{
    /// <summary>
    /// 鼠标悬停功能，应用于模块等
    /// 实现悬停过程中卡牌展示的功能
    /// </summary>
    private IMouseHoverComponent caller;


    void Awake()
    {
        caller = GetComponent<IMouseHoverComponent>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private bool isOnFocus;

    internal bool IsOnFocus
    {
        get { return isOnFocus; }

        set
        {
            if (!isOnFocus && value)
            {
                Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isOnFocus = value;
                caller.MouseHoverComponent_OnFocusBegin(cameraPosition);
            }

            else if (isOnFocus && !value)
            {
                isOnFocus = value;
                caller.MouseHoverComponent_OnFocusEnd();
            }
        }
    }

    private bool isOnHover1;

    internal bool IsOnHover1
    {
        get { return isOnHover1; }

        set
        {
            if (!isOnHover1 && value)
            {
                Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isOnHover1 = value;
                caller.MouseHoverComponent_OnHover1Begin(cameraPosition);
            }

            else if (isOnHover1 && !value)
            {
                isOnHover1 = value;
                caller.MouseHoverComponent_OnHover1End();
            }
        }
    }

    private bool isOnHover2;

    internal bool IsOnHover2
    {
        get { return isOnHover2; }

        set
        {
            if (!isOnHover2 && value)
            {
                Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isOnHover2 = value;
                caller.MouseHoverComponent_OnHover2Begin(cameraPosition);
            }

            else if (isOnHover2 && !value)
            {
                isOnHover2 = value;
                caller.MouseHoverComponent_OnHover2End();
            }
        }
    }

    private bool isOnPressHover;

    internal bool IsOnPressHover
    {
        get { return isOnPressHover; }

        set
        {
            if (!isOnPressHover && value)
            {
                Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isOnPressHover = value;
                caller.MouseHoverComponent_OnMousePressEnterImmediately(cameraPosition);
            }

            else if (isOnPressHover && !value)
            {
                isOnPressHover = value;
                caller.MouseHoverComponent_OnMousePressLeaveImmediately();
            }
        }
    }
}

public interface IMouseHoverComponent
{
    /// <summary>
    /// 此接口用于将除了MouseHoverComponent通用效果之外的效果还给调用者自行处理
    /// </summary>

    void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition);
    void MouseHoverComponent_OnHover1End();
    void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition);
    void MouseHoverComponent_OnHover2End();
    void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition);
    void MouseHoverComponent_OnFocusEnd();
    void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition);
    void MouseHoverComponent_OnMousePressLeaveImmediately();
}

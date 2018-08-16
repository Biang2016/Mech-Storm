using UnityEngine;
using System.Collections;

/// <summary>
/// 鼠标悬停管理器
/// </summary>
internal class MouseHoverManager : MonoSingletion<MouseHoverManager>
{
    private MouseHoverManager()
    {
    }

    int cardsLayer;
    int modulesLayer;
    int retinuesLayer;
    int slotsLayer;

    void Awake()
    {
        cardsLayer = 1 << LayerMask.NameToLayer("Cards");
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
        state = MHM_States.None;
        previousState = MHM_States.None;
    }

    void Start()
    {
        hi_CardHover = new HoverImmediately(cardsLayer);
        hi_ModulesHoverShowBloom = new HoverImmediately(modulesLayer);
        phi_SlotsPressHoverShowBloom = new PressHoverImmediately(slotsLayer);

        hd_ModulesFocusShowPreview = new Focus(modulesLayer | retinuesLayer, GameManager.Instance.RetinueDetailPreviewDelaySeconds, 100f);

        hi_RetinueHoverShowTargetedBloom = new HoverImmediately(retinuesLayer);
        hd_RetinuePressHoverShowTargetedBloom = new PressHoverImmediately(retinuesLayer);
    }

    public enum MHM_States
    {
        None = 0, //禁用
        Normal = 1,
        DragEquipment = 2, //拖动装备牌过程中
        DragRetinueToRetinue = 3, //随从拖动攻击随从
        DragSpellToRetinue = 4, //法术牌拖动瞄准随从
        SummonRetinueTargetOnRetinue = 5, //召唤带目标的随从时，选择目标期间
    }

    private MHM_States state;
    private MHM_States previousState;

    public void SetState(MHM_States newState)
    {
        if (state != newState)
        {
            switch (state)
            {
                case MHM_States.Normal:
                    hi_CardHover.Release();
                    hi_ModulesHoverShowBloom.Release();
                    hd_ModulesFocusShowPreview.Release();
                    break;
                case MHM_States.DragEquipment:
                    phi_SlotsPressHoverShowBloom.Release();
                    break;
                case MHM_States.DragRetinueToRetinue:
                    hd_RetinuePressHoverShowTargetedBloom.Release();
                    break;
                case MHM_States.DragSpellToRetinue:
                    hd_RetinuePressHoverShowTargetedBloom.Release();
                    break;
                case MHM_States.SummonRetinueTargetOnRetinue:
                    hi_RetinueHoverShowTargetedBloom.Release();
                    break;
            }

            previousState = state;
            state = newState;
            Debug.Log(state.ToString());
        }
    }

    public void ReturnToPreviousState()
    {
        SetState(previousState);
    }

    private HoverImmediately hi_CardHover; //当鼠标移到牌上
    private HoverImmediately hi_ModulesHoverShowBloom; //当鼠标移到装备上时显示轮廓荧光
    private HoverImmediately hi_RetinueHoverShowTargetedBloom; //当鼠标移到到随从上时显示被瞄准的轮廓荧光

    private Focus hd_ModulesFocusShowPreview; //当鼠标移到随从上一定时间后显示卡牌详情

    private PressHoverImmediately phi_SlotsPressHoverShowBloom; //当鼠标拖动装备牌到Slot装备位上时，显示Slot轮廓荧光
    private PressHoverImmediately hd_RetinuePressHoverShowTargetedBloom; //当鼠标拖拽到随从上时显示被瞄准的轮廓荧光

    void Update()
    {
        if (MainMenuManager.Instance.MainMenuState == MainMenuManager.MainMenuStates.Show) return;
        if (SelectCardDeckManager.Instance.SelectCardDeckState == SelectCardDeckManager.SelectCardDeckStates.Show) return;

        if (DragManager.Instance.CurrentDrag == null && DragManager.Instance.CurrentSummonPreviewRetinue == null) SetState(MHM_States.Normal);

        switch (state)
        {
            case MHM_States.Normal:
                hi_CardHover.Check<CardBase>();
                hi_ModulesHoverShowBloom.Check<ModuleBase>();
                hd_ModulesFocusShowPreview.Check<ModuleBase>();
                break;
            case MHM_States.DragEquipment:
                phi_SlotsPressHoverShowBloom.Check<Slot>();
                break;
            case MHM_States.DragRetinueToRetinue:
                hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                break;
            case MHM_States.DragSpellToRetinue:
                hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                break;
            case MHM_States.SummonRetinueTargetOnRetinue:
                hi_RetinueHoverShowTargetedBloom.Check<ModuleRetinue>();
                break;
        }
    }


    abstract class HoverActionBase
    {
        protected HoverActionBase(int layer)
        {
            Layer = layer;
        }

        protected int Layer;

        public abstract void Check<T>() where T : Component;
        public abstract void Release();
    }

    //判定鼠标未按下时的Hover，立即生效
    class HoverImmediately : HoverActionBase
    {
        public HoverImmediately(int layer) : base(layer)
        {
        }

        MouseHoverComponent currentHover; //鼠标悬停目标，立即生效

        public override void Check<T>()
        {
            if (!Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, Layer);
                Debug.DrawLine(ray.origin - 3 * ray.direction, ray.origin + 5 * ray.direction, Color.white);
                if (raycast.collider != null)
                {
                    MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                    if (mouseHoverComponent)
                    {
                        if (mouseHoverComponent.GetComponent<T>())
                        {
                            if (currentHover && currentHover != mouseHoverComponent)
                            {
                                Release();
                            }

                            currentHover = mouseHoverComponent;
                            currentHover.IsOnHover = true;
                        }
                    }
                    else
                    {
                        if (currentHover)
                        {
                            Release();
                        }
                    }
                }
                else
                {
                    if (currentHover)
                    {
                        Release();
                    }
                }
            }
            else
            {
                if (currentHover)
                {
                    Release();
                }
            }
        }

        public override void Release()
        {
            if (currentHover)
            {
                currentHover.IsOnHover = false;
                currentHover = null;
            }
        }
    }


    //判定鼠标按下时的Hover，立即生效
    class PressHoverImmediately : HoverActionBase
    {
        public PressHoverImmediately(int layer) : base(layer)
        {
        }

        MouseHoverComponent currentPressHover; //鼠标拖动至的目标，立即生效

        public override void Check<T>()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, Layer);
                Debug.DrawLine(ray.origin - 3 * ray.direction, ray.origin + 5 * ray.direction, Color.yellow);
                if (raycast.collider != null)
                {
                    MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                    if (mouseHoverComponent)
                    {
                        if (mouseHoverComponent.GetComponent<T>())
                        {
                            if (currentPressHover && currentPressHover != mouseHoverComponent)
                            {
                                Release();
                            }
                        }

                        currentPressHover = mouseHoverComponent;
                        currentPressHover.IsOnPressHover = true;
                    }
                    else
                    {
                        if (currentPressHover)
                        {
                            Release();
                        }
                    }
                }
                else
                {
                    if (currentPressHover)
                    {
                        Release();
                    }
                }
            }
            else
            {
                if (currentPressHover)
                {
                    Release();
                }
            }
        }

        public override void Release()
        {
            if (currentPressHover)
            {
                currentPressHover.IsOnPressHover = false;
                currentPressHover = null;
            }
        }
    }


    //判定鼠标未按下时的Focus，停留一定时间生效
    class Focus : HoverActionBase
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;
        MouseHoverComponent currentFocus; //鼠标悬停目标，超过一定时间且移动幅度不太大时生效

        public Focus(int layer, float delaySeconds, float mouseSpeedThreshold) : base(layer)
        {
            DelaySeconds = delaySeconds;
            MouseSpeedThreshold = mouseSpeedThreshold;
        }

        private float DelaySeconds;
        private float MouseSpeedThreshold;

        public override void Check<T>()
        {
            if (!Input.GetMouseButton(0))
            {
                Vector3 mouseCurrentPosition = Input.mousePosition;
                if ((mouseCurrentPosition - mouseLastPosition).magnitude / Time.deltaTime > MouseSpeedThreshold)
                {
                    //鼠标过快移动
                    mouseStopTimeTicker = 0;
                    mouseLastPosition = mouseCurrentPosition;
                    if (currentFocus)
                    {
                        Release();
                    }

                    return;
                }
                else
                {
                    mouseLastPosition = mouseCurrentPosition;
                    mouseStopTimeTicker += Time.deltaTime;
                    if (mouseStopTimeTicker > DelaySeconds)
                    {
                        mouseStopTimeTicker = 0;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit raycast;
                        Physics.Raycast(ray, out raycast, 10f, Layer);
                        Debug.DrawLine(ray.origin - 3 * ray.direction, ray.origin + 5 * ray.direction, Color.red, 1f);
                        if (raycast.collider != null)
                        {
                            MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                            if (mouseHoverComponent.GetComponent<T>())
                            {
                                if (mouseHoverComponent)
                                {
                                    if (currentFocus && currentFocus != mouseHoverComponent)
                                    {
                                        Release();
                                    }

                                    currentFocus = mouseHoverComponent;
                                    currentFocus.IsOnFocus = true;
                                }
                                else
                                {
                                    if (currentFocus)
                                    {
                                        Release();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (currentFocus)
                            {
                                Release();
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentFocus)
                {
                    Release();
                }
            }
        }

        public override void Release()
        {
            if (currentFocus)
            {
                currentFocus.IsOnFocus = false;
                currentFocus = null;
            }
        }
    }
}
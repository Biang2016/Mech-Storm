using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    int startUILayer;
    int exitUILayer;
    int cardSelectLayer;
    int selectCardDeckCanvasLayer;

    void Awake()
    {
        cardsLayer = 1 << LayerMask.NameToLayer("Cards");
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
        startUILayer = 1 << LayerMask.NameToLayer("StartMenuUI");
        exitUILayer = 1 << LayerMask.NameToLayer("ExitMenuUI");
        cardSelectLayer = 1 << LayerMask.NameToLayer("CardSelect");
        selectCardDeckCanvasLayer = 1 << LayerMask.NameToLayer("SelectCardDeckCanvas");
        state = MHM_States.None;
        previousState = MHM_States.None;
    }

    void Start()
    {
        hi_StartMenuUIButtonHover = new HoverImmediately(startUILayer, GameManager.Instance.ForeGroundCamera);
        hi_ExitMenuUIButtonHover = new HoverImmediately(exitUILayer, GameManager.Instance.ForeGroundCamera);

        hi_CardHover = new HoverImmediately(cardsLayer, GameManager.Instance.BattleGroundCamera);
        hi_ModulesHoverShowBloom = new HoverImmediately(modulesLayer, GameManager.Instance.BattleGroundCamera);

        phi_SlotsPressHoverShowBloom = new PressHoverImmediately(slotsLayer, GameManager.Instance.BattleGroundCamera);

        hd_ModulesFocusShowPreview = new Focus(modulesLayer | retinuesLayer, GameManager.Instance.BattleGroundCamera, GameManager.Instance.RetinueDetailPreviewDelaySeconds, 100f);

        hi_RetinueHoverShowTargetedBloom = new HoverImmediately(retinuesLayer, GameManager.Instance.BattleGroundCamera);
        hd_RetinuePressHoverShowTargetedBloom = new PressHoverImmediately(retinuesLayer, GameManager.Instance.BattleGroundCamera);
    }

    public enum MHM_States
    {
        None, //禁用
        ExitMenu, //Esc菜单
        StartMenu, //开始界面
        SelectCardWindow, //选卡界面
        BattleNormal, //战斗一般状态
        DragEquipment, //拖动装备牌过程中
        DragRetinueToRetinue, //随从拖动攻击随从
        DragSpellToRetinue, //法术牌拖动瞄准随从
        SummonRetinueTargetOnRetinue, //召唤带目标的随从时，选择目标期间
    }

    private MHM_States state;
    private MHM_States previousState;

    public void SetState(MHM_States newState)
    {
        if (state != newState)
        {
            switch (state)
            {
                case MHM_States.None:
                    break;
                case MHM_States.ExitMenu:
                    hi_ExitMenuUIButtonHover.Release();
                    break;
                case MHM_States.StartMenu:
                    hi_StartMenuUIButtonHover.Release();
                    break;
                case MHM_States.SelectCardWindow:
                    break;
                case MHM_States.BattleNormal:
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

    private HoverImmediately hi_StartMenuUIButtonHover; //当鼠标移到UI按钮上时显示动画
    private HoverImmediately hi_ExitMenuUIButtonHover; //当鼠标移到UI按钮上时显示动画

    private HoverImmediately hi_CardHover; //当鼠标移到牌上
    private HoverImmediately hi_ModulesHoverShowBloom; //当鼠标移到装备上时显示轮廓荧光
    private HoverImmediately hi_RetinueHoverShowTargetedBloom; //当鼠标移到到随从上时显示被瞄准的轮廓荧光
    private Focus hd_ModulesFocusShowPreview; //当鼠标移到随从上一定时间后显示卡牌详情
    private PressHoverImmediately phi_SlotsPressHoverShowBloom; //当鼠标拖动装备牌到Slot装备位上时，显示Slot轮廓荧光
    private PressHoverImmediately hd_RetinuePressHoverShowTargetedBloom; //当鼠标拖拽到随从上时显示被瞄准的轮廓荧光

    void Update()
    {
        if (Client.Instance.Proxy != null && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing && DragManager.Instance.CurrentDrag == null && DragManager.Instance.CurrentSummonPreviewRetinue == null) SetState(MHM_States.BattleNormal);

        switch (state)
        {
            case MHM_States.None:
                break;
            case MHM_States.StartMenu:
                hi_StartMenuUIButtonHover.Check<Button>();
                break;
            case MHM_States.ExitMenu:
                hi_ExitMenuUIButtonHover.Check<Button>();
                break;
            case MHM_States.SelectCardWindow:
                break;
            case MHM_States.BattleNormal:
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
        protected HoverActionBase(int layer, Camera camera)
        {
            Layer = layer;
            Camera = camera;
        }

        protected int Layer;
        protected Camera Camera;

        public abstract void Check<T>() where T : Component;
        public abstract void Release();
    }

    //判定鼠标未按下时的Hover，立即生效
    class HoverImmediately : HoverActionBase
    {
        public HoverImmediately(int layer, Camera camera) : base(layer, camera)
        {
        }

        MouseHoverComponent currentHover; //鼠标悬停目标，立即生效

        public override void Check<T>()
        {
            if (!Input.GetMouseButton(0))
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, Layer);
                Debug.DrawLine(ray.origin, ray.origin + 10 * ray.direction.normalized, Color.white);
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
        public PressHoverImmediately(int layer, Camera camera) : base(layer, camera)
        {
        }

        MouseHoverComponent currentPressHover; //鼠标拖动至的目标，立即生效

        public override void Check<T>()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, Layer);
                Debug.DrawLine(ray.origin, ray.origin + 10 * ray.direction.normalized, Color.yellow);
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

        public Focus(int layer, Camera camera, float delaySeconds, float mouseSpeedThreshold) : base(layer, camera)
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
                        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit raycast;
                        Physics.Raycast(ray, out raycast, 10f, Layer);
                        Debug.DrawLine(ray.origin, ray.origin + 10 * ray.direction.normalized, Color.red, 1f);
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
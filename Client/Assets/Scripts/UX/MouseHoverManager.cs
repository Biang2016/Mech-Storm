using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标悬停管理器
/// </summary>
public class MouseHoverManager : MonoSingleton<MouseHoverManager>
{
    private MouseHoverManager()
    {
    }

    int uiLayer;
    int cardsLayer;
    int modulesLayer;
    int retinuesLayer;
    int shipsLayer;
    int slotsLayer;
    int cardSelectLayer;

    void Awake()
    {
        uiLayer = 1 << LayerMask.NameToLayer("UI");
        cardsLayer = 1 << LayerMask.NameToLayer("Cards");
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        shipsLayer = 1 << LayerMask.NameToLayer("Ships");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
        cardSelectLayer = 1 << LayerMask.NameToLayer("CardSelect");
        M_StateMachine = new StateMachine();
    }

    void Update()
    {
        M_StateMachine.Update();
    }

    private Focus hi_MouseFocusUIHover; //UI

    private Hover1 hi_CardSelectHover; //选卡界面，当鼠标移到选的牌上放大

    private Hover1 hi_CardHover; //当鼠标移到牌上
    private PressHoverImmediately hi_CardPressHover; //当鼠标按住牌上
    private Hover1 hi_CardFocus; //当鼠标聚焦牌上
    private Hover1 hi_ModulesHoverShowBloom; //当鼠标移到装备上时显示轮廓荧光
    private Hover1 hi_RetinueHoverShowTargetedBloom; //当鼠标移到到机甲上时显示被瞄准的轮廓荧光
    private Hover2 hd_ModulesFocusShowPreview; //当鼠标移到机甲上一定时间后显示卡牌详情
    private PressHoverImmediately phi_SlotsPressHoverShowBloom_Target; //当Slot装备位被鼠标拖动瞄准时，显示Slot轮廓荧光
    private PressHoverImmediately phi_SlotsPressHoverShowBloom; //当鼠标拖动装备牌到Slot装备位上时，显示Slot轮廓荧光
    private PressHoverImmediately hd_RetinuePressHoverShowTargetedBloom; //当鼠标拖拽到机甲上时显示被瞄准的轮廓荧光
    private PressHoverImmediately hd_ShipPressHoverShowTargetedBloom; //当鼠标拖拽到战舰上时显示被瞄准的轮廓荧光

    void Start()
    {
        hi_MouseFocusUIHover = new Focus(uiLayer, GameManager.Instance.SelectCardWindowBackCamera, 0, -1f);

        hi_CardSelectHover = new Hover1(cardSelectLayer, GameManager.Instance.SelectCardWindowBackCamera);

        hi_CardHover = new Hover1(cardsLayer, GameManager.Instance.BattleGroundCamera);
        hi_CardPressHover = new PressHoverImmediately(cardsLayer, GameManager.Instance.BattleGroundCamera);
        hi_CardFocus = new Hover1(cardsLayer, GameManager.Instance.BattleGroundCamera, 0, -1f);
        hi_ModulesHoverShowBloom = new Hover1(modulesLayer, GameManager.Instance.BattleGroundCamera);

        phi_SlotsPressHoverShowBloom = new PressHoverImmediately(slotsLayer, GameManager.Instance.BattleGroundCamera);
        phi_SlotsPressHoverShowBloom_Target = new PressHoverImmediately(modulesLayer, GameManager.Instance.BattleGroundCamera);

        hd_ModulesFocusShowPreview = new Hover2(modulesLayer | retinuesLayer, GameManager.Instance.BattleGroundCamera, GameManager.Instance.RetinueDetailPreviewDelaySeconds, 100f);

        hi_RetinueHoverShowTargetedBloom = new Hover1(retinuesLayer, GameManager.Instance.BattleGroundCamera);
        hd_RetinuePressHoverShowTargetedBloom = new PressHoverImmediately(retinuesLayer, GameManager.Instance.BattleGroundCamera);
        hd_ShipPressHoverShowTargetedBloom = new PressHoverImmediately(shipsLayer, GameManager.Instance.BattleGroundCamera);
    }

    public StateMachine M_StateMachine;

    public class StateMachine
    {
        public StateMachine()
        {
            state = States.None;
            previousState = States.None;
        }

        public enum States
        {
            None, //禁用
            StartMenu, //开始界面
            ExitMenu, //Exit菜单
            SelectCardWindow, //选卡界面
            SelectCardWindow_ReadOnly, //选卡界面_战斗内
            BattleNormal, //战斗一般状态
            DragEquipment, //拖动装备牌过程中
            DragRetinueTo, //机甲拖动攻击
            DragSpellTo, //法术牌拖动瞄准
            SummonRetinueTargetOn, //召唤带目标的机甲时，选择目标期间
        }

        public static HashSet<States> OutGameState = new HashSet<States>
        {
            States.StartMenu,
            States.ExitMenu,
            States.SelectCardWindow,
        };

        public static HashSet<States> InGameState = new HashSet<States>
        {
            States.SelectCardWindow_ReadOnly,
            States.BattleNormal,
            States.DragEquipment,
            States.DragRetinueTo,
            States.DragSpellTo,
            States.SummonRetinueTargetOn,
        };

        private States state;
        private States previousState;

        public void SetState(States newState)
        {
            //if (BattleResultPanel.Instance.IsShow && state == States.StartMenu) return;
            if (state != newState)
            {
                Instance.hi_MouseFocusUIHover.Release();
                switch (state)
                {
                    case States.None:
                        break;
                    case States.StartMenu:
                        Instance.hi_CardSelectHover.Release();
                        break;
                    case States.ExitMenu:
                        Instance.hi_CardSelectHover.Release();
                        break;
                    case States.SelectCardWindow:
                        break;
                    case States.BattleNormal:
                        Instance.hi_CardHover.Release();
                        Instance.hi_CardFocus.Release();
                        Instance.hi_CardPressHover.Release();
                        Instance.hd_ModulesFocusShowPreview.Release();
                        break;
                    case States.DragEquipment:
                        Instance.phi_SlotsPressHoverShowBloom.Release();
                        break;
                    case States.DragRetinueTo:
                        Instance.hd_RetinuePressHoverShowTargetedBloom.Release();
                        Instance.hd_ShipPressHoverShowTargetedBloom.Release();
                        break;
                    case States.DragSpellTo:
                        Instance.hd_RetinuePressHoverShowTargetedBloom.Release();
                        Instance.hd_ShipPressHoverShowTargetedBloom.Release();
                        Instance.phi_SlotsPressHoverShowBloom_Target.Release();
                        break;
                    case States.SummonRetinueTargetOn:
                        Instance.hi_RetinueHoverShowTargetedBloom.Release();
                        break;
                }

                previousState = state;
                state = newState;

                Debug.Log("MHM state: " + state.ToString());
            }
        }

        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        public States GetState()
        {
            return state;
        }

        public void Update()
        {
            //if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
            if (InGameState.Contains(state) && DragManager.Instance.CurrentDrag == null && DragManager.Instance.CurrentSummonPreviewRetinue == null)
            {
                if (state != States.SelectCardWindow_ReadOnly) Instance.M_StateMachine.SetState(States.BattleNormal);
            }

            Instance.hi_MouseFocusUIHover.Check<MouseHoverUI>();
            switch (state)
            {
                case States.None:
                    break;
                case States.StartMenu:
                    break;
                case States.ExitMenu:
                    break;
                case States.SelectCardWindow:
                    Instance.hi_CardSelectHover.Check<CardBase>();
                    break;
                case States.BattleNormal:
                    Instance.hi_CardFocus.Check<CardBase>();
                    Instance.hi_CardHover.Check<CardBase>();
                    Instance.hi_CardPressHover.Check<CardBase>();
                    Instance.hi_ModulesHoverShowBloom.Check<ModuleBase>();
                    Instance.hd_ModulesFocusShowPreview.Check<ModuleBase>();
                    break;
                case States.DragEquipment:
                    Instance.phi_SlotsPressHoverShowBloom.Check<Slot>();
                    break;
                case States.DragRetinueTo:
                    Instance.hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                    Instance.hd_ShipPressHoverShowTargetedBloom.Check<Ship>();
                    break;
                case States.DragSpellTo:
                    Instance.hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                    Instance.hd_ShipPressHoverShowTargetedBloom.Check<Ship>();
                    Instance.phi_SlotsPressHoverShowBloom_Target.Check<ModuleEquip>();
                    break;
                case States.SummonRetinueTargetOn:
                    Instance.hi_RetinueHoverShowTargetedBloom.Check<ModuleRetinue>();
                    break;
            }
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

        protected MouseHoverComponent currentTarget; //当前目标

        public abstract void Check<T>() where T : Component;
        public abstract void Release();
    }

    //判定鼠标按下时的Hover，立即生效
    class PressHoverImmediately : HoverActionBase
    {
        public PressHoverImmediately(int layer, Camera camera) : base(layer, camera)
        {
        }

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
                            if (currentTarget && currentTarget != mouseHoverComponent)
                            {
                                Release();
                            }
                        }

                        currentTarget = mouseHoverComponent;
                        currentTarget.IsOnPressHover = true;
                    }
                    else
                    {
                        if (currentTarget)
                        {
                            Release();
                        }
                    }
                }
                else
                {
                    if (currentTarget)
                    {
                        Release();
                    }
                }
            }
            else
            {
                if (currentTarget)
                {
                    Release();
                }

                StateMachine.States curState = Instance.M_StateMachine.GetState();
                if (StateMachine.InGameState.Contains(curState) && curState != StateMachine.States.SelectCardWindow_ReadOnly) Instance.M_StateMachine.SetState(StateMachine.States.BattleNormal);
            }
        }

        public override void Release()
        {
            if (currentTarget)
            {
                currentTarget.IsOnPressHover = false;
                currentTarget = null;
            }
        }
    }

    //判定鼠标未按下时的Hover，停留一定时间生效
    class Hover1 : HoverActionBase
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;
        private bool needCheckSpeed = false;

        public Hover1(int layer, Camera camera, float delaySeconds = 0f, float mouseSpeedThreshold = -1) : base(layer, camera)
        {
            DelaySeconds = delaySeconds;
            needCheckSpeed = !mouseSpeedThreshold.Equals(-1f);
            MouseSpeedThreshold = mouseSpeedThreshold;
        }

        private float DelaySeconds;
        private float MouseSpeedThreshold;

        public override void Check<T>()
        {
            if (!Input.GetMouseButton(0))
            {
                Vector3 mouseCurrentPosition = Input.mousePosition;
                if (needCheckSpeed && (mouseCurrentPosition - mouseLastPosition).magnitude / Time.deltaTime > MouseSpeedThreshold)
                {
                    //鼠标过快移动
                    mouseStopTimeTicker = 0;
                    mouseLastPosition = mouseCurrentPosition;
                    if (currentTarget)
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
                            if (mouseHoverComponent && mouseHoverComponent.GetComponent<T>())
                            {
                                if (currentTarget && currentTarget != mouseHoverComponent)
                                {
                                    Release();
                                }

                                currentTarget = mouseHoverComponent;
                                currentTarget.IsOnHover1 = true;
                            }
                            else
                            {
                                if (currentTarget)
                                {
                                    Release();
                                }
                            }
                        }
                        else
                        {
                            if (currentTarget)
                            {
                                Release();
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentTarget)
                {
                    Release();
                }
            }
        }

        public override void Release()
        {
            if (currentTarget)
            {
                currentTarget.IsOnHover1 = false;
                currentTarget = null;
            }
        }
    }

    //判定鼠标未按下时的Hover，停留一定时间生效
    class Hover2 : HoverActionBase
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;
        private bool needCheckSpeed = false;

        public Hover2(int layer, Camera camera, float delaySeconds = 0f, float mouseSpeedThreshold = -1) : base(layer, camera)
        {
            DelaySeconds = delaySeconds;
            needCheckSpeed = !mouseSpeedThreshold.Equals(-1f);
            MouseSpeedThreshold = mouseSpeedThreshold;
        }

        private float DelaySeconds;
        private float MouseSpeedThreshold;

        public override void Check<T>()
        {
            if (!Input.GetMouseButton(0))
            {
                Vector3 mouseCurrentPosition = Input.mousePosition;
                if (needCheckSpeed && (mouseCurrentPosition - mouseLastPosition).magnitude / Time.deltaTime > MouseSpeedThreshold)
                {
                    //鼠标过快移动
                    mouseStopTimeTicker = 0;
                    mouseLastPosition = mouseCurrentPosition;
                    if (currentTarget)
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
                            if (mouseHoverComponent && mouseHoverComponent.GetComponent<T>())
                            {
                                if (currentTarget && currentTarget != mouseHoverComponent)
                                {
                                    Release();
                                }

                                currentTarget = mouseHoverComponent;
                                currentTarget.IsOnHover2 = true;
                            }
                            else
                            {
                                if (currentTarget)
                                {
                                    Release();
                                }
                            }
                        }
                        else
                        {
                            if (currentTarget)
                            {
                                Release();
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentTarget)
                {
                    Release();
                }
            }
        }

        public override void Release()
        {
            if (currentTarget)
            {
                currentTarget.IsOnHover2 = false;
                currentTarget = null;
            }
        }
    }

    //判定鼠标的Focus，停留一定时间生效
    class Focus : HoverActionBase
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;
        private bool needCheckSpeed = false;

        public Focus(int layer, Camera camera, float delaySeconds, float mouseSpeedThreshold) : base(layer, camera)
        {
            DelaySeconds = delaySeconds;
            needCheckSpeed = !mouseSpeedThreshold.Equals(-1f);
            MouseSpeedThreshold = mouseSpeedThreshold;
        }

        private float DelaySeconds;
        private float MouseSpeedThreshold;

        public override void Check<T>()
        {
            Vector3 mouseCurrentPosition = Input.mousePosition;
            if (needCheckSpeed && (mouseCurrentPosition - mouseLastPosition).magnitude / Time.deltaTime > MouseSpeedThreshold)
            {
                //鼠标过快移动
                mouseStopTimeTicker = 0;
                mouseLastPosition = mouseCurrentPosition;
                if (currentTarget)
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
                        if (mouseHoverComponent && mouseHoverComponent.GetComponent<T>())
                        {
                            if (currentTarget && currentTarget != mouseHoverComponent)
                            {
                                Release();
                            }

                            currentTarget = mouseHoverComponent;
                            currentTarget.IsOnFocus = true;
                        }
                        else
                        {
                            if (currentTarget)
                            {
                                Release();
                            }
                        }
                    }
                    else
                    {
                        if (currentTarget)
                        {
                            Release();
                        }
                    }
                }
            }
        }

        public override void Release()
        {
            if (currentTarget)
            {
                currentTarget.IsOnFocus = false;
                currentTarget = null;
            }
        }

        public void PrintCurrentTarget()
        {
            Debug.Log(currentTarget);
        }
    }
}
﻿using UnityEngine;

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
    int cardSelectLayer;

    void Awake()
    {
        cardsLayer = 1 << LayerMask.NameToLayer("Cards");
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
        cardSelectLayer = 1 << LayerMask.NameToLayer("CardSelect");
        M_StateMachine = new StateMachine();
    }

    void Update()
    {
        M_StateMachine.Update();
    }

    private HoverImmediately hi_CardSelectHover; //选卡界面，当鼠标移到牌上牌放大

    private HoverImmediately hi_CardHover; //当鼠标移到牌上
    private HoverImmediately hi_ModulesHoverShowBloom; //当鼠标移到装备上时显示轮廓荧光
    private HoverImmediately hi_RetinueHoverShowTargetedBloom; //当鼠标移到到随从上时显示被瞄准的轮廓荧光
    private Focus hd_ModulesFocusShowPreview; //当鼠标移到随从上一定时间后显示卡牌详情
    private PressHoverImmediately phi_SlotsPressHoverShowBloom; //当鼠标拖动装备牌到Slot装备位上时，显示Slot轮廓荧光
    private PressHoverImmediately hd_RetinuePressHoverShowTargetedBloom; //当鼠标拖拽到随从上时显示被瞄准的轮廓荧光

    void Start()
    {
        hi_CardSelectHover = new HoverImmediately(cardSelectLayer, GameManager.Instance.SelectCardWindowBackCamera);

        hi_CardHover = new HoverImmediately(cardsLayer, GameManager.Instance.BattleGroundCamera);
        hi_ModulesHoverShowBloom = new HoverImmediately(modulesLayer, GameManager.Instance.BattleGroundCamera);

        phi_SlotsPressHoverShowBloom = new PressHoverImmediately(slotsLayer, GameManager.Instance.BattleGroundCamera);

        hd_ModulesFocusShowPreview = new Focus(modulesLayer | retinuesLayer, GameManager.Instance.BattleGroundCamera, GameManager.Instance.RetinueDetailPreviewDelaySeconds, 100f);

        hi_RetinueHoverShowTargetedBloom = new HoverImmediately(retinuesLayer, GameManager.Instance.BattleGroundCamera);
        hd_RetinuePressHoverShowTargetedBloom = new PressHoverImmediately(retinuesLayer, GameManager.Instance.BattleGroundCamera);
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
            Menu, //开始界面和Exit菜单
            SelectCardWindow, //选卡界面
            BattleNormal, //战斗一般状态
            DragEquipment, //拖动装备牌过程中
            DragRetinueToRetinue, //随从拖动攻击随从
            DragSpellToRetinue, //法术牌拖动瞄准随从
            SummonRetinueTargetOnRetinue, //召唤带目标的随从时，选择目标期间
        }

        private States state;
        private States previousState;

        public void SetState(States newState)
        {
            if (state != newState)
            {
                switch (state)
                {
                    case States.None:
                        break;
                    case States.Menu:
                        Instance.hi_CardSelectHover.Release();
                        break;
                    case States.SelectCardWindow:
                        break;
                    case States.BattleNormal:
                        Instance.hi_CardHover.Release();
                        Instance.hi_ModulesHoverShowBloom.Release();
                        Instance.hd_ModulesFocusShowPreview.Release();
                        break;
                    case States.DragEquipment:
                        Instance.phi_SlotsPressHoverShowBloom.Release();
                        break;
                    case States.DragRetinueToRetinue:
                        Instance.hd_RetinuePressHoverShowTargetedBloom.Release();
                        break;
                    case States.DragSpellToRetinue:
                        Instance.hd_RetinuePressHoverShowTargetedBloom.Release();
                        break;
                    case States.SummonRetinueTargetOnRetinue:
                        Instance.hi_RetinueHoverShowTargetedBloom.Release();
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

        public States GetState()
        {
            return state;
        }

        public void Update()
        {
            if (Client.Instance.IsPlaying() && DragManager.Instance.CurrentDrag == null && DragManager.Instance.CurrentSummonPreviewRetinue == null) SetState(States.BattleNormal);
            switch (state)
            {
                case States.None:
                    break;
                case States.Menu:
                    break;
                case States.SelectCardWindow:
                    Instance.hi_CardSelectHover.Check<CardBase>();
                    break;
                case States.BattleNormal:
                    Instance.hi_CardHover.Check<CardBase>();
                    Instance.hi_ModulesHoverShowBloom.Check<ModuleBase>();
                    Instance.hd_ModulesFocusShowPreview.Check<ModuleBase>();
                    break;
                case States.DragEquipment:
                    Instance.phi_SlotsPressHoverShowBloom.Check<Slot>();
                    break;
                case States.DragRetinueToRetinue:
                    Instance.hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                    break;
                case States.DragSpellToRetinue:
                    Instance.hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
                    break;
                case States.SummonRetinueTargetOnRetinue:
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

    //判定鼠标未按下时的Hover，立即生效
    class HoverImmediately : HoverActionBase
    {
        public HoverImmediately(int layer, Camera camera) : base(layer, camera)
        {
        }

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
                            if (currentTarget && currentTarget != mouseHoverComponent)
                            {
                                Release();
                            }

                            currentTarget = mouseHoverComponent;
                            currentTarget.IsOnHover = true;
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
                currentTarget.IsOnHover = false;
                currentTarget = null;
            }
        }
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


    //判定鼠标未按下时的Focus，停留一定时间生效
    class Focus : HoverActionBase
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;

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
                            if (mouseHoverComponent.GetComponent<T>())
                            {
                                if (mouseHoverComponent)
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
                currentTarget.IsOnFocus = false;
                currentTarget = null;
            }
        }
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// 鼠标放在某个物体上的效果管理器
/// 每次只允许一个对象有效果
/// </summary>
internal class MouseHoverManager : MonoBehaviour
{
    void Awake()
    {
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
    }

    void Start()
    {
        hi_ModulesHoverShowBloom = new HoverImmediately(modulesLayer);
        phi_SlotsPressHoverShowBloom = new PressHoverImmediately(slotsLayer);
        hd_ModulesFocusShowPreview = new Focus(modulesLayer | retinuesLayer, GameManager.GM.RetinueDetailPreviewDelaySeconds,100f);
        hd_RetinuePressHoverShowTargetedBloom = new PressHoverImmediately(retinuesLayer);
    }

    int modulesLayer;
    int retinuesLayer;
    int slotsLayer;

    static MouseHoverComponent currentHover; //鼠标悬停目标，立即生效
    static MouseHoverComponent currentPressHover; //鼠标拖动至的目标，立即生效
    static MouseHoverComponent currentFocus; //鼠标悬停目标，超过一定时间且移动幅度不太大时生效

    private HoverImmediately hi_ModulesHoverShowBloom; //当鼠标移到装备上时显示轮廓荧光
    private PressHoverImmediately phi_SlotsPressHoverShowBloom; //当鼠标拖动装备牌到Slot装备位上时，显示Slot轮廓荧光
    private Focus hd_ModulesFocusShowPreview; //当鼠标移到随从上一定时间后显示卡牌详情
    private PressHoverImmediately hd_RetinuePressHoverShowTargetedBloom; //当鼠标拖拽到随从上时显示被瞄准的轮廓荧光

    void Update()
    {
        hi_ModulesHoverShowBloom.Check<ModuleBase>();
        phi_SlotsPressHoverShowBloom.Check<SlotAnchor>();
        hd_ModulesFocusShowPreview.Check<ModuleBase>();

        if (DragManager.DM.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.DM.CurrentDrag.GetComponent<ModuleRetinue>();
            if (mr)
            {
                hd_RetinuePressHoverShowTargetedBloom.Check<ModuleRetinue>();
            }
        }
    }

    //判定鼠标未按下时的Hover，立即生效
    class HoverImmediately
    {
        public HoverImmediately(int layer)
        {
            Layer = layer;
        }

        private int Layer;

        public void Check<T>() where T : Component
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
                                currentHover.IsOnHover = false;
                            }

                            currentHover = mouseHoverComponent;
                            currentHover.IsOnHover = true;
                        }
                    }
                    else
                    {
                        if (currentHover)
                        {
                            currentHover.IsOnHover = false;
                            currentHover = null;
                        }
                    }
                }
                else
                {
                    if (currentHover)
                    {
                        currentHover.IsOnHover = false;
                        currentHover = null;
                    }
                }
            }
            else
            {
                if (currentHover)
                {
                    currentHover.IsOnHover = false;
                    currentHover = null;
                }
            }
        }
    }


    //判定鼠标按下时的Hover，立即生效
    class PressHoverImmediately
    {
        public PressHoverImmediately(int layer)
        {
            Layer = layer;
        }

        private int Layer;

        public void Check<T>() where T : Component
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
                                currentPressHover.IsOnPressHover = false;
                            }
                        }

                        currentPressHover = mouseHoverComponent;
                        currentPressHover.IsOnPressHover = true;
                    }
                    else
                    {
                        if (currentPressHover)
                        {
                            currentPressHover.IsOnPressHover = false;
                            currentPressHover = null;
                        }
                    }
                }
                else
                {
                    if (currentPressHover)
                    {
                        currentPressHover.IsOnPressHover = false;
                        currentPressHover = null;
                    }
                }
            }
            else
            {
                if (currentPressHover)
                {
                    currentPressHover.IsOnPressHover = false;
                    currentPressHover = null;
                }
            }
        }
    }


    //判定鼠标未按下时的Focus，停留一定时间生效
    class Focus
    {
        Vector3 mouseLastPosition;
        private float mouseStopTimeTicker = 0;

        public Focus(int layer, float delaySeconds, float mouseSpeedThreshold)
        {
            Layer = layer;
            DelaySeconds = delaySeconds;
            MouseSpeedThreshold = mouseSpeedThreshold;
        }

        private int Layer;
        private float DelaySeconds;
        private float MouseSpeedThreshold;

        public void Check<T>() where T : Component
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
                        currentFocus.IsOnFocus = false;
                        currentFocus = null;
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
                                        currentFocus.IsOnFocus = false;
                                    }

                                    currentFocus = mouseHoverComponent;
                                    currentFocus.IsOnFocus = true;
                                }
                                else
                                {
                                    if (currentFocus)
                                    {
                                        currentFocus.IsOnFocus = false;
                                        currentFocus = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (currentFocus)
                            {
                                currentFocus.IsOnFocus = false;
                                currentFocus = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentFocus)
                {
                    currentFocus.IsOnFocus = false;
                    currentFocus = null;
                }
            }
        }
    }
}
using UnityEngine;
using System.Collections;

public class MouseHoverManager : MonoBehaviour {
    /// <summary>
    /// 鼠标放在某个物体上的效果管理器
    /// 每次只允许一个对象有效果
    /// </summary>


    void Awake()
    {
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
    }

    void Start()
    {

    }

    int modulesLayer;
    int retinuesLayer;
    int slotsLayer;


    public float mouseStopTimeThreshold = 1.0f;
    public float mouseSpeedThreshold = 10f;
    float mouseStopTimeTicker = 0;

    Vector3 mouseLastPosition;

    MouseHoverComponent currentHover;//鼠标悬停目标，立即生效
    MouseHoverComponent currentPressHover;//鼠标拖动至的目标，立即生效
    MouseHoverComponent currentFocus;//鼠标悬停目标，如果超过一定时间，就开始显示Detail


    void Update()
    {
        //判定鼠标未按下时的Hover，立即生效
        if (!Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, modulesLayer);
            if (raycast.collider != null) {
                MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                if (mouseHoverComponent) {
                    if (currentHover && currentHover != mouseHoverComponent) {
                        currentHover.IsOnHover = false;
                    }
                    currentHover = mouseHoverComponent;
                    currentHover.IsOnHover = true;
                } else {
                    if (currentHover) {
                        currentHover.IsOnHover = false;
                        currentHover = null;
                    }
                }
            } else {
                if (currentHover) {
                    currentHover.IsOnHover = false;
                    currentHover = null;
                }
            }
        } else {
            if (currentHover) {
                currentHover.IsOnHover = false;
                currentHover = null;
            }
        }

        //判定鼠标按下时的Hover，立即生效
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, slotsLayer);
            if (raycast.collider != null) {
                MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                if (mouseHoverComponent) {
                    if (currentPressHover && currentPressHover != mouseHoverComponent) {
                        currentPressHover.IsOnPressHover = false;
                    }
                    currentPressHover = mouseHoverComponent;
                    currentPressHover.IsOnPressHover = true;
                } else {
                    if (currentPressHover) {
                        currentPressHover.IsOnPressHover = false;
                        currentPressHover = null;
                    }
                }
            } else {
                if (currentPressHover) {
                    currentPressHover.IsOnPressHover = false;
                    currentPressHover = null;
                }
            }
        } else {
            if (currentPressHover) {
                currentPressHover.IsOnPressHover = false;
                currentPressHover = null;
            }
        }

        // 判定鼠标未按下时的Focus，停留一定时间生效
        if (!Input.GetMouseButton(0)) {
            Vector3 mouseCurrentPosition = Input.mousePosition;
            if ((mouseCurrentPosition - mouseLastPosition).magnitude / Time.deltaTime > mouseSpeedThreshold) {//鼠标过快移动
                mouseStopTimeTicker = 0;
                mouseLastPosition = mouseCurrentPosition;
                if (currentFocus) {
                    currentFocus.IsOnFocus = false;
                    currentFocus = null;
                }
                return;
            } else {
                mouseLastPosition = mouseCurrentPosition;
                mouseStopTimeTicker += Time.deltaTime;
                if (mouseStopTimeTicker > mouseStopTimeThreshold) {
                    mouseStopTimeTicker = 0;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit raycast;
                    Physics.Raycast(ray, out raycast, 10f, modulesLayer | retinuesLayer);
                    if (raycast.collider != null) {
                        MouseHoverComponent mouseHoverComponent = raycast.collider.gameObject.GetComponent<MouseHoverComponent>();
                        if (mouseHoverComponent) {
                            if (currentFocus && currentFocus != mouseHoverComponent) {
                                currentFocus.IsOnFocus = false;
                            }
                            currentFocus = mouseHoverComponent;
                            currentFocus.IsOnFocus = true;
                        } else {
                            if (currentFocus) {
                                currentFocus.IsOnFocus = false;
                                currentFocus = null;
                            }
                        }
                    } else {
                        if (currentFocus) {
                            currentFocus.IsOnFocus = false;
                            currentFocus = null;
                        }
                    }
                }
            }
        } else {
            if (currentFocus) {
                currentFocus.IsOnFocus = false;
                currentFocus = null;
            }
        }
    }
}
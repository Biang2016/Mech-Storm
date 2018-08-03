using UnityEngine;
using System.Collections;

internal class DragManager : MonoBehaviour
{
    /// <summary>
    /// 一次拖动一个卡牌、随从、英雄
    /// </summary>

    private static DragManager dm;
    public static DragManager DM {
        get {
            if (!dm) {
                dm = FindObjectOfType(typeof(DragManager)) as DragManager;
            }
            return dm;
        }
    }

    void Awake()
    {
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinueLayer = 1 << LayerMask.NameToLayer("Retinues");
        cardLayer = 1 << LayerMask.NameToLayer("Cards");
    }

    void Start()
    {

    }

    int modulesLayer;
    int retinueLayer;
    int cardLayer;

    internal DragComponent CurrentDrag;
    internal Arrow CurrentArrow;

    void Update()
    {
        if (SelectCardDeckManager.SCDM.IsShowing()) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (!CurrentDrag) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, cardLayer | retinueLayer | modulesLayer);
                if (raycast.collider != null) {
                    ColliderReplace colliderReplace = raycast.collider.gameObject.GetComponent<ColliderReplace>();
                    if (colliderReplace) {
                        CurrentDrag = colliderReplace.MyCallerCard.GetComponent<DragComponent>();
                    } else {
                        CurrentDrag = raycast.collider.gameObject.GetComponent<DragComponent>();
                    }
                    CurrentDrag.IsOnDrag = true;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (CurrentDrag)
            {
                CurrentDrag.IsOnDrag = false;
                CurrentDrag = null;
            }
        }
    }
}

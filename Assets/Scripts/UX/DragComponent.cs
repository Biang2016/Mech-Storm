using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DragComponent : MonoBehaviour
{
    /// <summary>
    /// 可拖动功能，应用于卡片、人物、技能、随从、
    /// 实现拖动过程中卡牌跟随鼠标的一段动画
    /// 如果被拖动物体带target效果，则此组件只实现最初的卡牌跟随鼠标动画，由targetArrow终结此过程
    /// </summary>
    int retinuesLayer;

    int boardAreasLayer;
    int slotsLayer;
    IDragComponent caller;

    void Awake()
    {
        retinuesLayer = 1 << LayerMask.NameToLayer("Retinues");
        boardAreasLayer = 1 << LayerMask.NameToLayer("BoardAreas");
        slotsLayer = 1 << LayerMask.NameToLayer("Slots");
        caller = GetComponent<IDragComponent>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (!canDrag) return;
        if (IsOnDrag)
        {
            caller.DragComponent_OnMousePressed(checkAreas(), checkMoveToSlot(), checkMoveTorRetinue()); //将鼠标悬停的区域告知拖动对象主体
            Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isBegin)
            {
                dragBeginPosition = cameraPosition;
                dragBeginQuaternion = transform.rotation;
                isBegin = false;
            }


            switch (dragPurpose)
            {
                case DragPurpose.Equip:
                    if ((transform.position - dragBeginPosition).magnitude < dragDistance) //拖拽物体本身 
                    {
                        transform.position = transform.position + cameraPosition - dragLastPosition;
                        dragLastPosition = cameraPosition;
                    }
                    else //拖拽一定距离产生效果
                    {
                        if (!DragManager.DM.CurrentArrow || !(DragManager.DM.CurrentArrow is ArrowArrow)) DragManager.DM.CurrentArrow = GameObjectPoolManager.GOPM.Pool_ArrowArrowPool.AllocateGameObject(DragManager.DM.transform).GetComponent<ArrowArrow>();
                        DragManager.DM.CurrentArrow.Render(dragBeginPosition, cameraPosition);
                        caller.DragComponnet_DragOutEffects();
                    }

                    break;
                case DragPurpose.Summon:
                    transform.position = transform.position + cameraPosition - dragLastPosition;
                    dragLastPosition = cameraPosition;
                    break;
                case DragPurpose.Attack:
                    if (!DragManager.DM.CurrentArrow || !(DragManager.DM.CurrentArrow is ArrowAiming)) DragManager.DM.CurrentArrow = GameObjectPoolManager.GOPM.Pool_ArrowAimingPool.AllocateGameObject(DragManager.DM.transform).GetComponent<ArrowAiming>();
                    DragManager.DM.CurrentArrow.Render(dragBeginPosition, cameraPosition);
                    caller.DragComponnet_DragOutEffects();
                    break;
            }
        }
    }

    bool canDrag;
    private DragPurpose dragPurpose;
    float dragDistance;
    private bool isOnDrag = false;
    bool isBegin = true;
    Vector3 dragBeginPosition;
    Quaternion dragBeginQuaternion;
    Vector3 dragLastPosition;

    public bool IsOnDrag
    {
        get { return isOnDrag; }

        set
        {
            CardBase possibleCard = GetComponent<CardBase>();
            ModuleBase possibleModuleBase = GetComponent<ModuleBase>();
            if (possibleCard && possibleCard.Player == RoundManager.RM.CurrentPlayer || possibleModuleBase && possibleModuleBase.Player == RoundManager.RM.CurrentPlayer)
            {
                if (value) //鼠标按下
                {
                    caller.DragComponent_SetStates(ref canDrag, ref dragPurpose);
                    if (canDrag)
                    {
                        dragLastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        caller.DragComponent_OnMouseDown();
                        dragDistance = caller.DragComponnet_DragDistance();
                        isOnDrag = value;
                    }
                    else
                    {
                        isOnDrag = false;
                        DragManager.DM.CurrentDrag = null;
                    }
                }
                else //鼠标放开
                {
                    if (canDrag)
                    {
                        //将鼠标放开的区域告知拖动对象主体，并提供拖动起始姿态信息以供还原
                        caller.DragComponent_OnMouseUp(checkAreas(), checkMoveToSlot(), checkMoveTorRetinue(), dragLastPosition, dragBeginPosition, dragBeginQuaternion);
                        dragLastPosition = Vector3.zero;
                        isBegin = true;
                        if (DragManager.DM.CurrentArrow) DragManager.DM.CurrentArrow.PoolRecycle();
                        isOnDrag = value;
                    }
                    else
                    {
                        isOnDrag = false;
                        DragManager.DM.CurrentDrag = null;
                    }
                }
            }
        }
    }

    BoardAreaTypes checkAreas() //检查鼠标悬停在哪个区域
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, boardAreasLayer);
        if (raycast.collider != null)
        {
            if (raycast.collider.gameObject.CompareTag("SelfHandArea"))
            {
                return BoardAreaTypes.SelfHandArea;
            }

            if (raycast.collider.gameObject.CompareTag("EnemyHandArea"))
            {
                return BoardAreaTypes.EnemyHandArea;
            }

            if (raycast.collider.gameObject.CompareTag("SelfBattleGroundArea"))
            {
                return BoardAreaTypes.SelfBattleGroundArea;
            }

            if (raycast.collider.gameObject.CompareTag("EnemyBattleGroundArea"))
            {
                return BoardAreaTypes.EnemyBattleGroundArea;
            }
        }

        return BoardAreaTypes.Others;
    }

    private List<SlotAnchor> checkMoveToSlot() //检查鼠标悬停在哪个类型的Slot上
    {
        var res = new List<SlotAnchor>();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var raycasts = Physics.RaycastAll(ray, 10f, slotsLayer);
        foreach (var rh in raycasts)
        {
            var sa = rh.collider.gameObject.GetComponent<SlotAnchor>();
            res.Add(sa);
        }

        return res;
    }


    private ModuleRetinue checkMoveTorRetinue() //检查鼠标悬停在哪个Retinue上
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, retinuesLayer);
        if (raycast.collider != null)
        {
            var mr = raycast.collider.gameObject.GetComponent<ModuleRetinue>();
            if (mr) return mr;
        }

        return null;
    }
}

public interface IDragComponent
{
    /// <summary>
    ///     此接口用于将除了DragComponent通用效果之外的效果还给调用者自行处理
    /// </summary>
    void DragComponent_OnMouseDown();

    /// <summary>
    ///     传达鼠标左键按住拖动时的鼠标位置信息
    /// </summary>
    /// <param name="boardAreaType">移动到了战场的哪个区域</param>
    /// <param name="slotAnchors">移动到了哪个slot上</param>
    /// <param name="moduleRetinue">移动到了哪个随从上</param>
    void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors,
        ModuleRetinue moduleRetinue);

    /// <summary>
    ///     传达鼠标左键松开时的鼠标位置信息
    /// </summary>
    /// <param name="boardAreaType">移动到了战场的哪个区域</param>
    /// <param name="slotAnchors">移动到了哪个slot上</param>
    /// <param name="moduleRetinue">移动到了哪个随从上</param>
    /// <param name="dragLastPosition">移动的最后位置</param>
    /// <param name="dragBeginPosition">移动的初始位置</param>
    /// <param name="dragBeginQuaternion">被拖动对象的初始旋转</param>
    void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors,
        ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition,
        Quaternion dragBeginQuaternion);

    void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose);
    float DragComponnet_DragDistance();
    void DragComponnet_DragOutEffects();
}

public enum DragPurpose
{
    None = 0,
    Summon = 1,
    Equip = 2,
    Attack = 3,
}
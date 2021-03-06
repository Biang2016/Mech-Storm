using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拖动组件，应用于卡片、人物、技能、机甲、
/// 实现拖动过程中卡牌跟随鼠标的一段动画
/// </summary>
public class DragComponent : MonoBehaviour
{
    IDragComponent caller;

    private CardBase possibleCard;
    private ModuleBase possibleModuleBase;

    void Awake()
    {
        caller = GetComponent<IDragComponent>();
    }

    private bool canDrag;
    private bool isOnDrag = false;
    private DragPurpose dragPurpose;
    private float dragDistance;

    private bool isBegin = true;
    private Vector3 dragBeginPosition;
    private Quaternion dragBeginQuaternion;
    private Vector3 dragLastPosition;

    void Update()
    {
        if (!canDrag) return;
        if (IsOnDrag)
        {
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
                        if (!DragManager.Instance.CurrentArrow || !(DragManager.Instance.CurrentArrow is ArrowArrow))
                            DragManager.Instance.CurrentArrow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ArrowArrow].AllocateGameObject<ArrowArrow>(DragManager.Instance.transform);
                        DragManager.Instance.CurrentArrow.Render(dragBeginPosition, cameraPosition);
                    }

                    caller.DragComponent_OnMousePressed(BoardAreaTypes.Others, checkMoveToSlot(), checkMoveToMech(), dragLastPosition); //将鼠标悬停的区域告知拖动对象主体
                    break;
                case DragPurpose.Summon:
                    transform.position = transform.position + cameraPosition - dragLastPosition;
                    dragLastPosition = cameraPosition;
                    caller.DragComponent_OnMousePressed(CheckAreas(), null, null, dragLastPosition); //将鼠标悬停的区域告知拖动对象主体
                    break;
                case DragPurpose.Target:
                    if ((transform.position - dragBeginPosition).magnitude < dragDistance) //拖拽物体本身 
                    {
                        transform.position = transform.position + cameraPosition - dragLastPosition;
                        dragLastPosition = cameraPosition;
                    }
                    else //拖拽一定距离产生效果
                    {
                        if (!DragManager.Instance.CurrentArrow || !(DragManager.Instance.CurrentArrow is ArrowAiming))
                            DragManager.Instance.CurrentArrow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ArrowAiming].AllocateGameObject<ArrowAiming>(DragManager.Instance.transform);
                        DragManager.Instance.CurrentArrow.Render(dragBeginPosition, cameraPosition);
                    }

                    caller.DragComponent_OnMousePressed(BoardAreaTypes.Others, null, checkMoveToMech(), dragLastPosition); //将鼠标悬停的区域告知拖动对象主体
                    break;
            }
        }
    }

    public bool IsOnDrag
    {
        get { return isOnDrag; }

        set
        {
            if (value) //鼠标按下
            {
                caller.DragComponent_SetStates(ref canDrag, ref dragPurpose);
                if (canDrag)
                {
                    dragLastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    caller.DragComponent_OnMouseDown();
                    dragDistance = caller.DragComponent_DragDistance();
                    isOnDrag = value;
                    if (dragPurpose != DragPurpose.Summon) caller.DragComponent_DragOutEffects();
                }
                else
                {
                    isOnDrag = false;
                    DragManager.Instance.CurrentDrag = null;
                }
            }
            else //鼠标放开
            {
                if (canDrag)
                {
                    //将鼠标放开的区域告知拖动对象主体，并提供拖动起始姿态信息以供还原
                    caller.DragComponent_OnMouseUp(CheckAreas(), checkMoveToSlot(), checkMoveToMech(), checkMoveToShip(), dragLastPosition, dragBeginPosition, dragBeginQuaternion);
                    dragLastPosition = Vector3.zero;
                    isBegin = true;
                    if (DragManager.Instance.CurrentArrow) DragManager.Instance.CurrentArrow.PoolRecycle();
                    isOnDrag = value;
                    if ((MouseHoverManager.Instance.M_StateMachine.GetState() & MouseHoverManager.StateMachine.States.BattleSpecial) != 0)
                    {
                        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
                    }
                }
                else
                {
                    isOnDrag = false;
                    DragManager.Instance.CurrentDrag = null;
                }
            }
        }
    }

    public static BoardAreaTypes CheckAreas() //检查鼠标悬停在哪个区域
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_BoardAreas);
        if (raycast.collider != null)
        {
            if (raycast.collider.gameObject.CompareTag("HandArea"))
            {
                return raycast.collider.transform.parent.GetComponent<HandManager>().ClientPlayer.BattlePlayer.HandArea;
            }

            if (raycast.collider.gameObject.CompareTag("BattleGroundArea"))
            {
                return raycast.collider.gameObject.GetComponent<BattleGroundManager>().ClientPlayer.BattlePlayer.BattleGroundArea;
            }
        }

        return BoardAreaTypes.Others;
    }

    private List<Slot> checkMoveToSlot() //检查鼠标悬停在哪个类型的Slot上
    {
        List<Slot> res = new List<Slot>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycasts = Physics.RaycastAll(ray, 10f, GameManager.Instance.Layer_Slots);
        foreach (RaycastHit rh in raycasts)
        {
            Slot sa = rh.collider.gameObject.GetComponent<Slot>();
            res.Add(sa);
        }

        return res;
    }

    private ModuleMech checkMoveToMech() //检查鼠标悬停在哪个Mech上
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Mechs);
        if (raycast.collider != null)
        {
            ModuleMech mr = raycast.collider.gameObject.GetComponent<ModuleMech>();
            if (mr) return mr;
        }

        return null;
    }

    private Ship checkMoveToShip() //检查鼠标悬停在哪个战舰上
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Ships);
        if (raycast.collider != null)
        {
            Ship mr = raycast.collider.gameObject.GetComponent<Ship>();
            if (mr) return mr;
        }

        return null;
    }
}

internal interface IDragComponent
{
    /// <summary>
    ///     此接口用于将除了DragComponent通用效果之外的效果还给调用者自行处理
    /// </summary>
    void DragComponent_OnMouseDown();

    /// <summary>
    ///     传达鼠标左键按住拖动时的鼠标位置信息
    /// </summary>
    /// <param name="boardAreaType">移动到了战场的哪个区域</param>
    /// <param name="slots">移动到了哪个slot上</param>
    /// <param name="moduleMech">移动到了哪个机甲上</param>
    /// <param name="dragLastPosition"></param>
    void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots,
        ModuleMech moduleMech, Vector3 dragLastPosition);

    /// <summary>
    ///     传达鼠标左键松开时的鼠标位置信息
    /// </summary>
    /// <param name="boardAreaType">移动到了战场的哪个区域</param>
    /// <param name="slots">移动到了哪个slot上</param>
    /// <param name="moduleMech">移动到了哪个机甲上</param>
    /// <param name="ship">移动到了哪个战舰</param>
    /// <param name="dragLastPosition">移动的最后位置</param>
    /// <param name="dragBeginPosition">移动的初始位置</param>
    /// <param name="dragBeginQuaternion">被拖动对象的初始旋转</param>
    void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots,
        ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition,
        Quaternion dragBeginQuaternion);

    void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose);
    float DragComponent_DragDistance();
    void DragComponent_DragOutEffects();
}
using UnityEngine;

/// <summary>
/// 鼠标拖拽管理器
/// </summary>
public class DragManager : MonoSingleton<DragManager>
{
    private DragManager()
    {
    }

    void Awake()
    {
    }

    internal Arrow CurrentArrow;

    internal CardMech CurrentDrag_CardMech;
    internal CardEquip CurrentDrag_CardEquip;
    internal CardSpell CurrentDrag_CardSpell;
    internal ModuleMech CurrentDrag_ModuleMech;

    private DragComponent currentDrag;

    internal DragComponent CurrentDrag
    {
        get { return currentDrag; }
        set
        {
            currentDrag = value;
            if (currentDrag == null)
            {
                CurrentDrag_CardMech = null;
                CurrentDrag_CardEquip = null;
                CurrentDrag_CardSpell = null;
                CurrentDrag_ModuleMech = null;
            }
            else
            {
                CurrentDrag_CardMech = currentDrag.GetComponent<CardMech>();
                CurrentDrag_CardEquip = currentDrag.GetComponent<CardEquip>();
                CurrentDrag_CardSpell = currentDrag.GetComponent<CardSpell>();
                CurrentDrag_ModuleMech = currentDrag.GetComponent<ModuleMech>();

                if (CurrentDrag_CardEquip && CurrentDrag_CardEquip.Usable && CurrentDrag_CardEquip.ClientPlayer.BattlePlayer.HandManager.CurrentFocusCard == CurrentDrag_CardEquip)
                {
                    MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.DragEquipment);
                }
                else if (CurrentDrag_CardSpell && CurrentDrag_CardSpell.Usable && CurrentDrag_CardSpell.ClientPlayer.BattlePlayer.HandManager.CurrentFocusCard == CurrentDrag_CardSpell)
                {
                    MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.DragSpellTo);
                }
                else if (CurrentDrag_ModuleMech)
                {
                    MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.DragMechTo);
                }
            }
        }
    }

    public int DragOutDamage = 0; //鼠标拖动时附带的预计伤害

    void Update()
    {
        //if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
        //if (ExitMenuPanel.Instance.M_StateMachine.GetState() == ExitMenuPanel.StateMachine.States.Show) ResetCurrentDrag();
        //if (SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.Show) ResetCurrentDrag();
        if (!Client.Instance.IsPlaying())
        {
            ResetCurrentDrag();
            return;
        }

        //if (BattleResultPanel.Instance.IsShow) ResetCurrentDrag();
        if (!IsSummonPreview)
        {
            CommonDrag();
        }
        else
        {
            SummonPreviewDrag();
        }
    }

    private void CommonDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!CurrentDrag)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Cards | GameManager.Instance.Layer_Mechs | GameManager.Instance.Layer_Modules);
                if (raycast.collider != null)
                {
                    ColliderReplace colliderReplace = raycast.collider.gameObject.GetComponent<ColliderReplace>();
                    if (colliderReplace)
                    {
                        CurrentDrag = colliderReplace.MyCallerCard.GetComponent<DragComponent>();
                    }
                    else
                    {
                        CurrentDrag = raycast.collider.gameObject.GetComponent<DragComponent>();
                    }

                    CurrentDrag.IsOnDrag = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetCurrentDrag();
        }
    }

    private void ResetCurrentDrag()
    {
        if (CurrentDrag)
        {
            CurrentDrag.IsOnDrag = false;
            CurrentDrag = null;
        }
    }

    # region 召唤机甲指定目标预览

    internal bool IsSummonPreview;
    internal bool IsArrowShowBegin;
    internal ModuleMech CurrentSummonPreviewMech;
    public BattleGroundManager.SummonMechTarget SummonMechTargetHandler;
    public TargetRange SummonMechTargetRange;

    public const int TARGET_SELECT_NONE = -2;

    public void StartArrowAiming(ModuleMech mech, TargetRange targetRange)
    {
        IsSummonPreview = true;
        CurrentSummonPreviewMech = mech;
        SummonMechTargetRange = targetRange;
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SummonMechTargetOn);
    }

    private void SummonPreviewDrag()
    {
        if (IsArrowShowBegin)
        {
            if (!CurrentArrow || !(CurrentArrow is ArrowAiming)) CurrentArrow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ArrowAiming].AllocateGameObject<ArrowAiming>(transform);
            Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CurrentArrow.Render(CurrentSummonPreviewMech.transform.position, cameraPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Mechs);
            if (raycast.collider == null) //没有选中目标，则撤销
            {
                SummonMechTargetHandler(-2);
                if (CurrentArrow) CurrentArrow.PoolRecycle();
                IsSummonPreview = false;
                IsArrowShowBegin = false;
            }
            else
            {
                ModuleMech mech = raycast.collider.GetComponent<ModuleMech>();
                if (mech == null)
                {
                    SummonMechTargetHandler(-2);
                    if (CurrentArrow) CurrentArrow.PoolRecycle();
                    IsSummonPreview = false;
                    IsArrowShowBegin = false;
                }
                else
                {
                    if (RoundManager.Instance.SelfClientPlayer.BattlePlayer.BattleGroundManager.CurrentSummonPreviewMech == mech //不可指向自己
                        || RoundManager.Instance.SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(mech) //不可是死亡对象
                        || RoundManager.Instance.EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(mech)) //不可是死亡对象
                    {
                        SummonMechTargetHandler(-2);
                        if (CurrentArrow) CurrentArrow.PoolRecycle();
                        IsSummonPreview = false;
                        IsArrowShowBegin = false;
                    }
                    else
                    {
                        int targetMechID = mech.M_MechID;
                        bool isClientMechTempId = false;
                        if (mech.M_MechID == -1) //如果该机甲还未从服务器取得ID，则用tempID
                        {
                            targetMechID = mech.M_ClientTempMechID;
                            isClientMechTempId = true;
                        }

                        switch (SummonMechTargetRange)
                        {
                            case TargetRange.None:
                                SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.Mechs:
                                SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                break;
                            case TargetRange.SelfMechs:
                                if (mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.EnemyMechs:
                                if (mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.Heroes:
                                if (!mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.SelfHeroes:
                                if (mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.EnemyHeroes:
                                if (mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.Soldiers:
                                if (mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.SelfSoldiers:
                                if (mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.EnemySoldiers:
                                if (mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && mech.CardInfo.MechInfo.IsSoldier) SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                else SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.SelfShip:
                                SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.EnemyShip:
                                SummonMechTargetHandler(-2);
                                break;
                            case TargetRange.AllLife:
                                SummonMechTargetHandler(targetMechID, isClientMechTempId);
                                break;
                        }
                    }

                    if (CurrentArrow) CurrentArrow.PoolRecycle();
                    IsSummonPreview = false;
                    IsArrowShowBegin = false;
                }
            }

            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
        {
            SummonMechTargetHandler(-2);
            if (CurrentArrow) CurrentArrow.PoolRecycle();
            IsSummonPreview = false;
            IsArrowShowBegin = false;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        }
    }

    #endregion
}
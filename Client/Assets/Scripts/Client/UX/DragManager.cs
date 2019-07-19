using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        if (!Client.Instance.IsPlaying())
        {
            ResetCurrentDrag();
            return;
        }

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

    public void ResetCurrentDrag()
    {
        if (CurrentDrag)
        {
            CurrentDrag.IsOnDrag = false;
            CurrentDrag = null;
        }
    }

    public bool IsCanceling = false;

    public void CancelCurrentDrag()
    {
        IsCanceling = true;
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
    public TargetRange SummonMechTargetRange;

    public void StartSummonMechTargetArrowAiming(ModuleMech mech, TargetRange targetRange)
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

        UnityAction<List<int>, List<bool>> summonConfirmAction = RoundManager.Instance.CurrentClientPlayer.BattlePlayer.BattleGroundManager.SummonMechTargetConfirm;
        if (IsCanceling)
        {
            summonConfirmAction(null, null);
            CancelSummonPreview();
            IsCanceling = false;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Mechs);
            if (raycast.collider == null) //没有选中目标，则撤销
            {
                summonConfirmAction(null, null);
                CancelSummonPreview();
            }
            else
            {
                ModuleMech mech = raycast.collider.GetComponent<ModuleMech>();
                if (mech == null)
                {
                    summonConfirmAction(null, null);
                    CancelSummonPreview();
                }
                else
                {
                    if (RoundManager.Instance.SelfClientPlayer.BattlePlayer.BattleGroundManager.CurrentSummonPreviewMech == mech //不可指向自己
                        || RoundManager.Instance.SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(mech) //不可是死亡对象
                        || RoundManager.Instance.EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(mech)) //不可是死亡对象
                    {
                        summonConfirmAction(null, null);
                        CancelSummonPreview();
                    }
                    else
                    {
                        bool isTargetMechTempID = mech.M_ClientTempMechID != (int) Const.SpecialMechID.ClientTempMechIDNormal;
                        int targetMechID = isTargetMechTempID ? mech.M_ClientTempMechID : mech.M_MechID;

                        bool isTargetValid = false;
                        switch (SummonMechTargetRange)
                        {
                            case TargetRange.None:
                                isTargetValid = false;
                                break;
                            case TargetRange.Mechs:
                                isTargetValid = true;
                                break;
                            case TargetRange.SelfMechs:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer;
                                break;
                            case TargetRange.EnemyMechs:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer;
                                break;
                            case TargetRange.Heroes:
                                isTargetValid = !mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.SelfHeroes:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.EnemyHeroes:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.Soldiers:
                                isTargetValid = mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.SelfSoldiers:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.EnemySoldiers:
                                isTargetValid = mech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && mech.CardInfo.MechInfo.IsSoldier;
                                break;
                            case TargetRange.SelfShip:
                                isTargetValid = false;
                                break;
                            case TargetRange.EnemyShip:
                                isTargetValid = false;
                                break;
                            case TargetRange.AllLife:
                                isTargetValid = true;
                                break;
                        }

                        if (isTargetValid)
                        {
                            summonConfirmAction(new List<int> {targetMechID}, new List<bool> {isTargetMechTempID});
                        }
                        else
                        {
                            summonConfirmAction(null, null);
                        }
                    }

                    CancelSummonPreview();
                }
            }

            if ((MouseHoverManager.Instance.M_StateMachine.GetState() & MouseHoverManager.StateMachine.States.BattleSpecial) != 0)
            {
                MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
        {
            summonConfirmAction(null, null);
            CancelSummonPreview();
            if ((MouseHoverManager.Instance.M_StateMachine.GetState() & MouseHoverManager.StateMachine.States.BattleSpecial) != 0)
            {
                MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
            }
        }
    }

    private void CancelSummonPreview()
    {
        if (CurrentArrow) CurrentArrow.PoolRecycle();
        IsSummonPreview = false;
        IsArrowShowBegin = false;
    }

    #endregion
}
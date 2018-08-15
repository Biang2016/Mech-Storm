using UnityEngine;
using System.Collections;

/// <summary>
/// 鼠标拖拽管理器
/// </summary>
internal class DragManager : MonoSingletion<DragManager>
{
    private DragManager()
    {
    }

    void Awake()
    {
        modulesLayer = 1 << LayerMask.NameToLayer("Modules");
        retinueLayer = 1 << LayerMask.NameToLayer("Retinues");
        cardLayer = 1 << LayerMask.NameToLayer("Cards");
    }

    int modulesLayer;
    int retinueLayer;
    int cardLayer;

    internal Arrow CurrentArrow;

    private DragComponent currentDrag;

    internal DragComponent CurrentDrag
    {
        get { return currentDrag; }
        set
        {
            currentDrag = value;
            if (currentDrag == null)
            {
                CurrentDrag_CardRetinue = null;
                CurrentDrag_CardWeapon = null;
                CurrentDrag_CardShield = null;
                CurrentDrag_CardSpell = null;
                CurrentDrag_ModuleRetinue = null;
            }
            else
            {
                CurrentDrag_CardRetinue = currentDrag.GetComponent<CardRetinue>();
                CurrentDrag_CardWeapon = currentDrag.GetComponent<CardWeapon>();
                CurrentDrag_CardShield = currentDrag.GetComponent<CardShield>();
                CurrentDrag_CardSpell = currentDrag.GetComponent<CardSpell>();
                CurrentDrag_ModuleRetinue = currentDrag.GetComponent<ModuleRetinue>();

                if (CurrentDrag_CardWeapon || CurrentDrag_CardShield)
                {
                    MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.DragEquipment);
                }
                else if (CurrentDrag_CardSpell)
                {
                    MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.DragSpellToRetinue);
                }
                else if (CurrentDrag_ModuleRetinue)
                {
                    MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.DragRetinueToRetinue);
                }
            }
        }
    }

    internal CardRetinue CurrentDrag_CardRetinue;
    internal CardWeapon CurrentDrag_CardWeapon;
    internal CardShield CurrentDrag_CardShield;
    internal CardSpell CurrentDrag_CardSpell;
    internal ModuleRetinue CurrentDrag_ModuleRetinue;


    void Update()
    {
        if (SelectCardDeckManager.Instance.IsShowing()) return;
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
                Physics.Raycast(ray, out raycast, 10f, cardLayer | retinueLayer | modulesLayer);
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
            if (CurrentDrag)
            {
                CurrentDrag.IsOnDrag = false;
                CurrentDrag = null;
            }
        }
    }

    # region 召唤随从指定目标预览

    internal bool IsSummonPreview;
    internal ModuleRetinue CurrentSummonPreviewRetinue;
    public BattleGroundManager.SummonRetinueTarget SummonRetinueTargetHandler;
    public TargetSideEffect.TargetRange SummonRetinueTargetRange;

    public const int TARGET_SELECT_NONE = -2;

    public void StartArrowAiming(ModuleRetinue retinue, TargetSideEffect.TargetRange targetRange)
    {
        IsSummonPreview = true;
        CurrentSummonPreviewRetinue = retinue;
        SummonRetinueTargetRange = targetRange;
        MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.SummonRetinueTargetOnRetinue);
    }

    private void SummonPreviewDrag()
    {
        if (!CurrentArrow || !(CurrentArrow is ArrowAiming)) CurrentArrow = GameObjectPoolManager.Instance.Pool_ArrowAimingPool.AllocateGameObject(transform).GetComponent<ArrowAiming>();
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CurrentArrow.Render(CurrentSummonPreviewRetinue.transform.position, cameraPosition);
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, retinueLayer);
            if (raycast.collider == null) //没有选中目标，则撤销
            {
                SummonRetinueTargetHandler(-2);
                CurrentArrow.PoolRecycle();
                IsSummonPreview = false;
            }
            else
            {
                ModuleRetinue retinue = raycast.collider.GetComponent<ModuleRetinue>();
                if (retinue == null)
                {
                    SummonRetinueTargetHandler(-2);
                    CurrentArrow.PoolRecycle();
                    IsSummonPreview = false;
                }
                else
                {
                    if (RoundManager.Instance.SelfClientPlayer.MyBattleGroundManager.CurrentSummonPreviewRetinue == retinue) //不可指向自己
                    {
                        SummonRetinueTargetHandler(-2);
                        CurrentArrow.PoolRecycle();
                        IsSummonPreview = false;
                    }
                    else
                    {
                        int targetRetinueID = retinue.M_RetinueID;
                        bool isClientRetinueTempId = false;
                        if (retinue.M_RetinueID == -1) //如果该随从还未从服务器取得ID，则用tempID
                        {
                            targetRetinueID = retinue.M_ClientTempRetinueID;
                            isClientRetinueTempId = true;
                        }

                        switch (SummonRetinueTargetRange)
                        {
                            case TargetSideEffect.TargetRange.None:
                                SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.SelfBattleGround:
                                if (retinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.EnemyBattleGround:
                                if (retinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.SelfSodiers:
                                if (retinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer && retinue.CardInfo.BattleInfo.IsSodier) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.EnemySodiers:
                                if (retinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && retinue.CardInfo.BattleInfo.IsSodier) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.SelfHeros:
                                if (retinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !retinue.CardInfo.BattleInfo.IsSodier) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.EnemyHeros:
                                if (retinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !retinue.CardInfo.BattleInfo.IsSodier) SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                else SummonRetinueTargetHandler(-2);
                                break;
                            case TargetSideEffect.TargetRange.All:
                                SummonRetinueTargetHandler(targetRetinueID, isClientRetinueTempId);
                                break;
                        }
                    }


                    CurrentArrow.PoolRecycle();
                    IsSummonPreview = false;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
        {
            SummonRetinueTargetHandler(-2);
            CurrentArrow.PoolRecycle();
            IsSummonPreview = false;
        }
    }

    #endregion
}
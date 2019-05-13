using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private DragComponent DragComponent;
    [SerializeField] private MouseHoverComponent MouseHoverComponent;
    [SerializeField] private BoxCollider BoxCollider;

    public override void PoolRecycle()
    {
        if (DragComponent)
        {
            DragComponent.enabled = true;
        }

        if (MouseHoverComponent)
        {
            MouseHoverComponent.enabled = true;
        }

        if (BoxCollider)
        {
            BoxCollider.enabled = true;
        }

        if (this is ModuleWeapon)
        {
            ((ModuleWeapon) this).SetNoPreview();
        }

        if (this is ModuleShield)
        {
            ((ModuleShield) this).SetNoPreview();
        }

        if (this is ModulePack)
        {
            ((ModulePack) this).SetNoPreview();
        }

        if (this is ModuleMA)
        {
            ((ModuleMA) this).SetNoPreview();
        }

        base.PoolRecycle();
    }

    internal CardInfo_Base CardInfo; //卡牌原始数值信息
    private bool isDead;

    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        ChangeColor(ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()));
        BeBrightColor();
    }

    protected virtual void InitializeComponents()
    {
        CardStarsComponent.SetStarNumber(CardInfo.UpgradeInfo.CardLevel, CardInfo.UpgradeInfo.CardLevelMax);
    }

    #region 各模块

    public CardStarsComponent CardStarsComponent;

    public Renderer MainBoardRenderer;
    protected float MainboardEmissionIntensity = 0f;

    protected virtual void ChangeColor(Color color)
    {
        ClientUtils.ChangeColor(MainBoardRenderer, color);
    }

    protected void BeDimColor()
    {
        ChangeColor(Color.gray);
    }

    protected void BeBrightColor()
    {
        ClientUtils.ChangeColor(MainBoardRenderer, ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()));
    }

    #endregion

    #region 模块交互

    #region SE

    public virtual void OnShowEffects(SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange)
    {
    }

    #endregion

    public virtual void DragComponent_OnMouseDown()
    {
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Vector3 dragLastPosition)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
    }

    public virtual void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = ClientPlayer == RoundManager.Instance.SelfClientPlayer;
        dragPurpose = CardInfo.BaseInfo.DragPurpose;
    }

    public virtual float DragComponent_DragDistance()
    {
        return 0;
    }

    public virtual void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnHover1End()
    {
    }

    public virtual void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
        if (DragManager.Instance.IsSummonPreview) return;
        BattleManager.Instance.ShowCardDetailInBattleManager.ShowCardDetail(this);
        RootManager.Instance.StartBlurBackGround();
    }

    public virtual void MouseHoverComponent_OnHover2End()
    {
        if (DragManager.Instance.IsSummonPreview) return;
        BattleManager.Instance.ShowCardDetailInBattleManager.HideCardDetail();
        RootManager.Instance.StopBlurBackGround();
    }

    public virtual void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnFocusEnd()
    {
    }

    public virtual void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
    }

    public virtual void DragComponent_SetHasTarget(ref bool hasTarget)
    {
        hasTarget = true;
    }

    public virtual void DragComponent_DragOutEffects()
    {
    }

    #endregion
}
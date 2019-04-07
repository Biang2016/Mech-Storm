using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal int GameObjectID;
    protected GameObjectPool gameObjectPool;
    internal ClientPlayer ClientPlayer;

    public override void PoolRecycle()
    {
        GameObjectID = -1;
        if (GetComponent<DragComponent>())
        {
            GetComponent<DragComponent>().enabled = true;
        }

        if (GetComponent<MouseHoverComponent>())
        {
            GetComponent<MouseHoverComponent>().enabled = true;
        }

        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().enabled = true;
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

        gameObject.SetActive(true);
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
        MainboardEmissionIntensity = CardInfo.GetCardColorIntensity();
        ChangeColor(ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()));
        Stars = cardInfo.UpgradeInfo.CardLevel;
        BeBrightColor();
    }

    #region 各模块

    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    [SerializeField] protected int stars;

    public virtual int Stars
    {
        get { return stars; }

        set
        {
            stars = value;
            switch (value)
            {
                case 0:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    break;
                case 1:
                    if (Star1) Star1.SetActive(true);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    break;
                case 2:
                    if (Star1) Star1.SetActive(true);
                    if (Star2) Star2.SetActive(true);
                    if (Star3) Star3.SetActive(false);
                    break;
                case 3:
                    if (Star1) Star1.SetActive(true);
                    if (Star2) Star2.SetActive(true);
                    if (Star3) Star3.SetActive(true);
                    break;
                default: break;
            }
        }
    }

    public Renderer MainBoardRenderer;
    protected float MainboardEmissionIntensity = 0f;

    public virtual void ChangeColor(Color color)
    {
        ClientUtils.ChangeColor(MainBoardRenderer, color);
    }

    public void BeDimColor()
    {
        ChangeColor(Color.gray);
    }

    public void BeBrightColor()
    {
        ClientUtils.ChangeColor(MainBoardRenderer, ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()));
    }

    #endregion

    #region 模块交互

    #region SE

    public virtual void OnShowEffects(SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange)
    {
    }

    #endregion

    public virtual void DragComponent_OnMouseDown()
    {
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
    }

    public virtual void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = ClientPlayer == RoundManager.Instance.SelfClientPlayer;
        dragPurpose = CardInfo.BaseInfo.DragPurpose;
    }

    public virtual float DragComponnet_DragDistance()
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
        ShowCardDetailInBattleManager.Instance.ShowCardDetail(this);
        GameManager.Instance.StartBlurBackGround();
    }

    public virtual void MouseHoverComponent_OnHover2End()
    {
        if (DragManager.Instance.IsSummonPreview) return;
        ShowCardDetailInBattleManager.Instance.HideCardDetail();
        if (UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().IsReadOnly) return;
        GameManager.Instance.StopBlurBackGround();
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

    public virtual void DragComponnet_DragOutEffects()
    {
    }

    #endregion
}
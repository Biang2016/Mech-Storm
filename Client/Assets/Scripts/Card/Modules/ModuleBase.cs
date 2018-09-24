using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour, IGameObjectPool, IDragComponent, IMouseHoverComponent
{
    internal int GameObjectID;
    protected GameObjectPool gameObjectPool;
    internal ClientPlayer ClientPlayer;

    public virtual void PoolRecycle()
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
        gameObjectPool.RecycleGameObject(gameObject);
    }

    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    //工具，初始化数字块
    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block, char firstSign)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
        }
    }

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        ChangeColor(ClientUtils.HTMLColorToColor(CardInfo.BaseInfo.GetCardColor()));
        Stars = cardInfo.UpgradeInfo.CardLevel;
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
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(true);
                    if (Star3) Star3.SetActive(false);
                    break;
                case 3:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(true);
                    break;
                default: break;
            }
        }
    }

    [SerializeField] private Renderer MainBoardRenderer;

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
        ClientUtils.ChangeColor(MainBoardRenderer, ClientUtils.HTMLColorToColor(CardInfo.BaseInfo.GetCardColor()));
    }

    #endregion


    #region 模块交互

    private CardBase detailCard;
    private CardBase detailCard_Weapon;
    private CardBase detailCard_Shield;
    private CardBase detailCard_Pack;
    private CardBase detailCard_MA;

    private void ShowCardDetail() //鼠标悬停放大查看原卡牌信息
    {
        switch (CardInfo.BaseInfo.CardType)
        {
            case CardTypes.Retinue:
                detailCard = (CardRetinue) CardBase.InstantiateCardByCardInfo(CardInfo, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                detailCard.transform.localScale = Vector3.one * GameManager.Instance.DetailRetinueCardSize;
                detailCard.transform.position = new Vector3(0, 8f, 0);
                detailCard.transform.Translate(Vector3.left * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.GetComponent<DragComponent>().enabled = false;
                detailCard.BeBrightColor();

                CardRetinue cardRetinue = (CardRetinue) detailCard;
                cardRetinue.SetCanvasSortingOrder(2);
                cardRetinue.ShowAllSlotHover();

                if (((ModuleRetinue) this).M_Weapon)
                {
                    if (!cardRetinue.Weapon)
                    {
                        cardRetinue.Weapon = GameObjectPoolManager.Instance.Pool_ModuleWeaponDetailPool.AllocateGameObject(cardRetinue.transform).GetComponent<ModuleWeapon>();
                    }

                    CardInfo_Base cw = ((ModuleRetinue) this).M_Weapon.CardInfo;
                    cardRetinue.Weapon.M_ModuleRetinue = (ModuleRetinue) this;
                    cardRetinue.Weapon.Initiate(((ModuleRetinue) this).M_Weapon.GetCurrentCardInfo(), ClientPlayer);
                    cardRetinue.Weapon.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Weapon.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Weapon.SetPreview();

                    detailCard_Weapon = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                    detailCard_Weapon.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Weapon.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Weapon.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Weapon.transform.Translate(Vector3.back * 3f);
                    detailCard_Weapon.transform.Translate(Vector3.up * 5f);
                    detailCard_Weapon.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Weapon.BeBrightColor();
                }

                if (((ModuleRetinue) this).M_Shield)
                {
                    if (!cardRetinue.Shield)
                    {
                        cardRetinue.Shield = GameObjectPoolManager.Instance.Pool_ModuleShieldDetailPool.AllocateGameObject(cardRetinue.transform).GetComponent<ModuleShield>();
                    }

                    CardInfo_Base cw = ((ModuleRetinue) this).M_Shield.CardInfo;
                    cardRetinue.Shield.M_ModuleRetinue = (ModuleRetinue) this;
                    cardRetinue.Shield.Initiate(((ModuleRetinue) this).M_Shield.GetCurrentCardInfo(), ClientPlayer);
                    cardRetinue.Shield.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Shield.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Shield.SetPreview();

                    detailCard_Shield = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                    detailCard_Shield.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Shield.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Shield.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Shield.transform.Translate(Vector3.forward * 3f);
                    detailCard_Shield.transform.Translate(Vector3.up * 5f);
                    detailCard_Shield.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Shield.BeBrightColor();
                }

                if (((ModuleRetinue) this).M_Pack)
                {
                    if (!cardRetinue.Pack)
                    {
                        cardRetinue.Pack = GameObjectPoolManager.Instance.Pool_ModulePackDetailPool.AllocateGameObject(cardRetinue.transform).GetComponent<ModulePack>();
                    }

                    CardInfo_Base cw = ((ModuleRetinue) this).M_Pack.CardInfo;
                    cardRetinue.Pack.M_ModuleRetinue = (ModuleRetinue) this;
                    cardRetinue.Pack.Initiate(((ModuleRetinue) this).M_Pack.GetCurrentCardInfo(), ClientPlayer);
                    cardRetinue.Pack.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Pack.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Pack.SetPreview();

                    detailCard_Pack = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                    detailCard_Pack.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Pack.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Pack.transform.Translate(Vector3.left * 10.5f);
                    detailCard_Pack.transform.Translate(Vector3.back * 3f);
                    detailCard_Pack.transform.Translate(Vector3.up * 5f);
                    detailCard_Pack.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Pack.BeBrightColor();
                }

                if (((ModuleRetinue) this).M_MA)
                {
                    if (!cardRetinue.MA)
                    {
                        cardRetinue.MA = GameObjectPoolManager.Instance.Pool_ModuleMADetailPool.AllocateGameObject(cardRetinue.transform).GetComponent<ModuleMA>();
                    }

                    CardInfo_Base cw = ((ModuleRetinue) this).M_MA.CardInfo;
                    cardRetinue.MA.M_ModuleRetinue = (ModuleRetinue) this;
                    cardRetinue.MA.Initiate(((ModuleRetinue) this).M_MA.GetCurrentCardInfo(), ClientPlayer);
                    cardRetinue.MA.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.MA.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.MA.SetPreview();

                    detailCard_MA = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                    detailCard_MA.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_MA.transform.position = new Vector3(0, 2f, 0);
                    detailCard_MA.transform.Translate(Vector3.left * 10.5f);
                    detailCard_MA.transform.Translate(Vector3.forward * 3f);
                    detailCard_MA.transform.Translate(Vector3.up * 5f);
                    detailCard_MA.GetComponent<BoxCollider>().enabled = false;
                    detailCard_MA.BeBrightColor();
                }

                break;
            case CardTypes.Equip:
                detailCard = (CardEquip) CardBase.InstantiateCardByCardInfo(CardInfo, GameBoardManager.Instance.CardDetailPreview.transform, ClientPlayer, false);
                detailCard.transform.localScale = Vector3.one * GameManager.Instance.DetailSingleCardSize;
                detailCard.transform.position = new Vector3(0, 2f, 0);
                detailCard.transform.Translate(Vector3.left * 3.5f);
                detailCard.transform.Translate(Vector3.up * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.BeBrightColor();
                break;
            default:
                break;
        }
    }

    private void HideCardDetail()
    {
        if (detailCard)
        {
            if (detailCard is CardRetinue)
            {
                ((CardRetinue) detailCard).SetCanvasSortingOrder(0);
            }

            detailCard.PoolRecycle();
            detailCard = null;
        }

        if (detailCard_Weapon)
        {
            detailCard_Weapon.PoolRecycle();
            detailCard_Weapon = null;
        }

        if (detailCard_Shield)
        {
            detailCard_Shield.PoolRecycle();
            detailCard_Shield = null;
        }

        if (detailCard_Pack)
        {
            detailCard_Pack.PoolRecycle();
            detailCard_Pack = null;
        }

        if (detailCard_MA)
        {
            detailCard_MA.PoolRecycle();
            detailCard_MA = null;
        }
    }

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
        ShowCardDetail();
        GameManager.Instance.StartBlurBackGround();
    }

    public virtual void MouseHoverComponent_OnHover2End()
    {
        if (DragManager.Instance.IsSummonPreview) return;
        HideCardDetail();
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
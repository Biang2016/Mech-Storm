using System.Collections.Generic;
using UnityEngine;

internal class CardBase : MonoBehaviour, IGameObjectPool, IDragComponent
{
    protected GameObjectPool gameObjectPool;
    internal Player Player;

    public virtual void PoolRecycle()
    {
        ResetColliderAndReplace();
        if (GetComponent<DragComponent>())
        {
            GetComponent<DragComponent>().enabled = true;
        }

        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().enabled = true;
        }

        gameObjectPool.RecycleGameObject(gameObject);
    }

    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    protected virtual void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 卡牌上各模块

    public Renderer MainBoardRenderer;
    public GameObject CardBloom;
    public GameObject Block_Cost;
    protected GameObject GoNumberSet_Cost;
    protected CardNumberSet CardNumberSet_Cost;
    public Renderer PictureBoxRenderer;

    public void ChangeColor(Color newColor)
    {
        if (MainBoardRenderer)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            MainBoardRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_EmissionColor", newColor);
            MainBoardRenderer.SetPropertyBlock(mpb);
        }
    }

    # endregion

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, Player player)
    {
        CardBase newCard;
        switch (cardInfo.CardType)
        {
            case CardTypes.Retinue:
                newCard = GameObjectPoolManager.GOPM.Pool_RetinueCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                break;
            case CardTypes.Weapon:
                newCard = GameObjectPoolManager.GOPM.Pool_WeaponCardPool.AllocateGameObject(parent).GetComponent<CardWeapon>();
                break;
            case CardTypes.Shield:
                newCard = GameObjectPoolManager.GOPM.Pool_ShieldCardPool.AllocateGameObject(parent).GetComponent<CardShield>();
                break;
            default:
                newCard = GameObjectPoolManager.GOPM.Pool_RetinueCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                break;
        }

        newCard.Initiate(cardInfo, player);
        newCard.ChangeColor(cardInfo.CardColor);
        return newCard;
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.GOPM.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign);
        }
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block, char firstSign)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.GOPM.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign);
        }
    }

    public virtual void Initiate(CardInfo_Base cardInfo, Player player)
    {
        Player = player;
        CardInfo = cardInfo;
        initiateNumbers(ref GoNumberSet_Cost, ref CardNumberSet_Cost, NumberSize.Big, CardNumberSet.TextAlign.Center, Block_Cost);
        M_Cost = CardInfo.Cost;
        CardPictureManager.ChangePicture(PictureBoxRenderer, CardInfo.CardID);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);
    }


    #region 卡牌数值变化

    private int m_Cost;

    public int M_Cost
    {
        get { return m_Cost; }
        set
        {
            m_Cost = value;
            CardNumberSet_Cost.Number = value;
        }
    }

    #endregion

    #region 卡牌交互

    internal ColliderReplace myColliderReplace;
    internal BoxCollider myCollider;

    private bool usable;

    internal bool Usable
    {
        get { return usable; }

        set
        {
            usable = value;
            if (CardBloom) CardBloom.SetActive(value);
        }
    }


    public virtual void OnBeginRound()
    {
    }

    public virtual void OnEndRound()
    {
    }

    internal void ResetColliderAndReplace()
    {
        if (myCollider)
        {
            myCollider.enabled = true;
        }

        if (myColliderReplace)
        {
            GameObjectPoolManager.GOPM.Pool_ColliderReplacePool.RecycleGameObject(myColliderReplace.gameObject);
            myColliderReplace = null;
        }
    }

    private void OnMouseEnter()
    {
        Player.MyHandManager.CardOnMouseEnter(this); //通知手牌管理器该聚焦本牌了
    }

    public void DragComponent_OnMouseDown()
    {
        Player.MyHandManager.BeginDrag();
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors,
        ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition,
        Quaternion dragBeginQuaternion)
    {
        Player.MyHandManager.EndDrag();
        Player.UseCost(M_Cost);
    }

    public virtual void DragComponent_SetStates(ref bool canDrag, ref bool hasTarget)
    {
        canDrag = Usable;
        hasTarget = CardInfo.HasTarget;
    }

    public virtual float DragComponnet_DragDistance()
    {
        return 1f;
    }

    public void DragComponnet_DragOutEffects()
    {
        transform.position = GameObjectPool.GameObjectPoolPosition;
    }

    #endregion

    #region  Utils

    public static void RepairDisplayCardOutOfView(CardBase targetCard)//检查卡牌是否在视野外，如果是则复位
    {

    }

    #endregion
}
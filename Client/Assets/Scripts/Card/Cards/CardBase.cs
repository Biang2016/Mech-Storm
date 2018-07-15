using System.Collections.Generic;
using UnityEngine;

internal abstract class CardBase : MonoBehaviour, IGameObjectPool, IDragComponent
{
    protected GameObjectPool gameObjectPool;
    internal ClientPlayer ClientPlayer;

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


    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, ClientPlayer clientPlayer)
    {
        CardBase newCard;
        switch (cardInfo.BaseInfo.CardType)
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

        newCard.Initiate(cardInfo, clientPlayer);
        newCard.ChangeColor(HTMLColorToColor(cardInfo.BaseInfo.CardColor));
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

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        initiateNumbers(ref GoNumberSet_Cost, ref CardNumberSet_Cost, NumberSize.Big, CardNumberSet.TextAlign.Center, Block_Cost);
        M_Cost = CardInfo.BaseInfo.Cost;
        CardPictureManager.ChangePicture(PictureBoxRenderer, CardInfo.CardID);
        ChangeCardBloomColor(GameManager.GM.CardBloomColor);
        Stars = cardInfo.UpgradeInfo.CardLevel;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);
    }


    #region 属性

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

    private int m_CardPlaceIndex;

    public int M_CardPlaceIndex
    {
        get { return m_CardPlaceIndex; }
        set { m_CardPlaceIndex = value; }
    }

    #endregion

    #region 卡牌上各模块

    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    public GameObject Star4;

    [SerializeField] protected int stars;

    public int Stars
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
                    if (Star4) Star4.SetActive(false);
                    break;
                case 1:
                    if (Star1) Star1.SetActive(true);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 2:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(true);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 3:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(true);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 4:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(true);
                    break;
                default: break;
            }
        }
    }

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
            mpb.SetColor("_Color", newColor);
            mpb.SetColor("_EmissionColor", newColor);
            MainBoardRenderer.SetPropertyBlock(mpb);
        }
    }

    private void ChangeCardBloomColor(Color color)
    {
        if (CardBloom)
        {
            Renderer rd = CardBloom.GetComponent<Renderer>();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rd.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", color);
            mpb.SetColor("_EmissionColor", color);
            rd.SetPropertyBlock(mpb);
        }
    }

    # endregion

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
        ClientPlayer.MyHandManager.CardOnMouseEnter(this); //通知手牌管理器该聚焦本牌了
    }

    public void DragComponent_OnMouseDown()
    {
        ClientPlayer.MyHandManager.BeginDrag();
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        ClientPlayer.MyHandManager.EndDrag();
    }

    public virtual void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = Usable;
        dragPurpose = CardInfo.BaseInfo.DragPurpose;
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

    public static void RepairDisplayCardOutOfView(CardBase targetCard) //检查卡牌是否在视野外，如果是则复位
    {
    }

    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    #endregion
}
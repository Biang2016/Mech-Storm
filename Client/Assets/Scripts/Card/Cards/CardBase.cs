using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal abstract class CardBase : MonoBehaviour, IGameObjectPool, IDragComponent, IMouseHoverComponent
{
    protected GameObjectPool gameObjectPool;
    internal ClientPlayer ClientPlayer;
    protected bool IsCardSelect;

    public virtual void PoolRecycle()
    {
        if (!IsCardSelect)
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

            CanBecomeBigger = true;
            Usable = false;
            gameObjectPool.RecycleGameObject(gameObject);
            transform.localScale = Vector3.one * 2;
            transform.rotation = Quaternion.Euler(0, -180, 0);
            DragComponent.enabled = true;
        }
        else
        {
            gameObjectPool.RecycleGameObject(gameObject);
        }

        gameObject.SetActive(true);
        CardBloom.SetActive(false);
    }


    internal CardInfo_Base CardInfo; //卡牌原始数值信息
    internal DragComponent DragComponent;

    protected virtual void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        DragComponent = GetComponent<DragComponent>();
    }

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, ClientPlayer clientPlayer, bool isCardSelect)
    {
        CardBase newCard;
        if (!isCardSelect)
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.GOPM.Pool_RetinueCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_RetinueCardPool;
                    break;
                case CardTypes.Weapon:
                    newCard = GameObjectPoolManager.GOPM.Pool_WeaponCardPool.AllocateGameObject(parent).GetComponent<CardWeapon>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_WeaponCardPool;
                    break;
                case CardTypes.Shield:
                    newCard = GameObjectPoolManager.GOPM.Pool_ShieldCardPool.AllocateGameObject(parent).GetComponent<CardShield>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_ShieldCardPool;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.GOPM.Pool_SpellCardPool.AllocateGameObject(parent).GetComponent<CardSpell>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_SpellCardPool;
                    break;
                default:
                    newCard = GameObjectPoolManager.GOPM.Pool_RetinueCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_RetinueCardPool;
                    break;
            }

            newCard.ChangeCardBloomColor(GameManager.Instance.CardBloomColor);
        }
        else
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.GOPM.Pool_RetinueSelectCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_RetinueSelectCardPool;
                    break;
                case CardTypes.Weapon:
                    newCard = GameObjectPoolManager.GOPM.Pool_WeaponSelectCardPool.AllocateGameObject(parent).GetComponent<CardWeapon>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_WeaponSelectCardPool;
                    break;
                case CardTypes.Shield:
                    newCard = GameObjectPoolManager.GOPM.Pool_ShieldSelectCardPool.AllocateGameObject(parent).GetComponent<CardShield>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_ShieldSelectCardPool;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.GOPM.Pool_SpellSelectCardPool.AllocateGameObject(parent).GetComponent<CardSpell>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_SpellSelectCardPool;
                    break;
                default:
                    newCard = GameObjectPoolManager.GOPM.Pool_RetinueSelectCardPool.AllocateGameObject(parent).GetComponent<CardRetinue>();
                    newCard.gameObjectPool = GameObjectPoolManager.GOPM.Pool_RetinueSelectCardPool;
                    break;
            }

            newCard.transform.localScale = Vector3.one * 120;
            newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
            newCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#A1F7FF"));
        }

        newCard.IsCardSelect = isCardSelect;
        newCard.Initiate(cardInfo, clientPlayer, isCardSelect);
        newCard.ChangeColor(ClientUtils.HTMLColorToColor(cardInfo.BaseInfo.CardColor));
        return newCard;
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.GOPM.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, IsCardSelect);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, IsCardSelect);
        }
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block, char firstSign)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.GOPM.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, IsCardSelect);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, IsCardSelect);
        }
    }

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        IsCardSelect = isCardSelect;
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        initiateNumbers(ref GoNumberSet_Cost, ref CardNumberSet_Cost, NumberSize.Big, CardNumberSet.TextAlign.Center, Block_Cost);
        if (Block_Count)
        {
            initiateNumbers(ref GoNumberSet_Count, ref CardNumberSet_Count, NumberSize.Big, CardNumberSet.TextAlign.Center, Block_Count, 'x');
            CardNumberSet_Count.Clear();
        }

        M_Cost = CardInfo.BaseInfo.Cost;
        ChangePicture(CardInfo.CardID);
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

    private int m_CardInstanceId;

    public int M_CardInstanceId
    {
        get { return m_CardInstanceId; }
        set { m_CardInstanceId = value; }
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

    protected int stars;

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
    public Renderer PictureBoxRenderer;
    [SerializeField] private Image Image_RetinueDescPanel;

    public GameObject Block_Cost;
    protected GameObject GoNumberSet_Cost;
    protected CardNumberSet CardNumberSet_Cost;

    public GameObject Block_Count;
    protected GameObject GoNumberSet_Count;
    protected CardNumberSet CardNumberSet_Count;

    public void BeDimColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.BaseInfo.CardColor);
        ChangeColor(new Color(color.r / 2, color.g / 2, color.b / 2, color.a));
        ChangePictureColor(new Color(0.5f, 0.5f, 0.5f));
    }

    public void BeBrightColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.BaseInfo.CardColor);
        ChangeColor(color);
        ChangePictureColor(Color.white);
    }

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

        if (Image_RetinueDescPanel)
        {
            Image_RetinueDescPanel.color = new Color(newColor.r/2, newColor.g / 2, newColor.b / 2, 0.5f);
        }
    }

    public void ChangeCardBloomColor(Color color)
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

    public void ChangePicture(int pictureID)
    {
        Texture tx = (Texture) Resources.Load(string.Format("{0:000}", pictureID));
        if (tx == null) Debug.LogError("所选卡片没有图片资源：" + pictureID);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        PictureBoxRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", tx);
        mpb.SetTexture("_EmissionMap", tx);
        PictureBoxRenderer.SetPropertyBlock(mpb);
    }

    public void ChangePictureColor(Color newColor)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        PictureBoxRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", newColor);
        mpb.SetColor("_EmissionColor", newColor);
        PictureBoxRenderer.SetPropertyBlock(mpb);
    }

    public void SetBlockCountValue(int value)
    {
        if (value == 0)
        {
            CardNumberSet_Count.Clear();
        }
        else if (value > 0)
        {
            CardNumberSet_Count.Number = value;
        }
    }

    # endregion

    #region 卡牌交互

    internal ColliderReplace myColliderReplace;
    internal BoxCollider myCollider;

    internal bool CanBecomeBigger = true;

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

    public virtual void DragComponent_OnMouseDown()
    {
        ClientPlayer.MyHandManager.BeginDrag();
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
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

    public virtual void DragComponnet_DragOutEffects()
    {
        transform.position = GameObjectPool.GameObjectPoolPosition;
    }

    public virtual void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
        if (CanBecomeBigger) ClientPlayer.MyHandManager.CardOnMouseEnter(this);
    }

    public virtual void MouseHoverComponent_OnMouseEnter(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnMouseOver()
    {
    }

    public virtual void MouseHoverComponent_OnMouseLeave()
    {
    }

    public virtual void MouseHoverComponent_OnMouseLeaveImmediately()
    {
    }

    public virtual void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
    }

    #endregion

    #region  Utils

    public static void ReplaceDisplayCardOutOfView(CardBase targetCard) //检查卡牌是否在视野外，如果是则复位
    {
    }

    #endregion
}
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    protected bool IsCardSelect;
    [SerializeField] private DragComponent M_DragComponent;
    [SerializeField] private BoxCollider M_BoxCollider;

    internal bool IsFlying;

    public override void PoolRecycle()
    {
        iTween.Stop(gameObject);
        if (!IsCardSelect)
        {
            ResetColliderAndReplace();

            Usable = false;
            base.PoolRecycle();
            transform.localScale = Vector3.one * 2;
            transform.rotation = Quaternion.Euler(0, -180, 0);
            DragComponent.enabled = true;
            IsFlying = false;
        }
        else
        {
            base.PoolRecycle();
        }

        gameObject.SetActive(true);
        CardBloom.SetActive(false);
    }

    public CardInfo_Base CardInfo; //卡牌原始数值信息
    internal DragComponent DragComponent;

    protected virtual void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        DragComponent = GetComponent<DragComponent>();

        MetalIcon.color = GameManager.Instance.MetalIconColor;
        EnergyIcon.color = GameManager.Instance.EnergyIconColor;
        if (LifeIcon) LifeIcon.color = GameManager.Instance.LifeIconColor;
        Text_CardType.fontStyle = GameManager.Instance.isEnglish ? FontStyle.Bold : FontStyle.Normal;
        Text_CardTypeBG.fontStyle = GameManager.Instance.isEnglish ? FontStyle.Bold : FontStyle.Normal;
        Text_Desc.color = ClientUtils.HTMLColorToColor(AllColors.ColorDict[AllColors.ColorType.CardDecsTextColor]);
    }

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, ClientPlayer clientPlayer, bool isCardSelect)
    {
        CardBase newCard;
        if (!isCardSelect)
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.Instance.Pool_RetinueCardPool.AllocateGameObject<CardRetinue>(parent);
                    break;
                case CardTypes.Equip:
                    newCard = GameObjectPoolManager.Instance.Pool_EquipCardPool.AllocateGameObject<CardEquip>(parent);
                    ((CardEquip) newCard).M_EquipType = cardInfo.EquipInfo.SlotType;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.Instance.Pool_SpellCardPool.AllocateGameObject<CardSpell>(parent);
                    break;
                case CardTypes.Energy:
                    newCard = GameObjectPoolManager.Instance.Pool_SpellCardPool.AllocateGameObject<CardSpell>(parent);
                    break;
                default:
                    newCard = GameObjectPoolManager.Instance.Pool_RetinueCardPool.AllocateGameObject<CardRetinue>(parent);
                    break;
            }

            newCard.ChangeCardBloomColor(GameManager.Instance.CardBloomColor);

            newCard.CoinText.text = "";
            newCard.CoinImage.enabled = false;
            newCard.CoinImageBG.enabled = false;
            newCard.CoinImageBG.gameObject.SetActive(false);

            newCard.DragComponent.enabled = true;
            newCard.M_BoxCollider.enabled = true;

            if (clientPlayer.WhichPlayer == Players.Self)
            {
                if (newCard.CardBackRenderer) newCard.CardBackRenderer.gameObject.SetActive(clientPlayer.WhichPlayer != Players.Self);
            }
        }
        else
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.Instance.Pool_RetinueCardSelectPool.AllocateGameObject<CardRetinue>(parent);
                    break;
                case CardTypes.Equip:
                    newCard = GameObjectPoolManager.Instance.Pool_EquipCardSelectPool.AllocateGameObject<CardEquip>(parent);
                    ((CardEquip) newCard).M_EquipType = cardInfo.EquipInfo.SlotType;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.Instance.Pool_SpellCardSelectPool.AllocateGameObject<CardSpell>(parent);
                    break;
                case CardTypes.Energy:
                    newCard = GameObjectPoolManager.Instance.Pool_SpellCardSelectPool.AllocateGameObject<CardSpell>(parent);
                    break;
                default:
                    newCard = GameObjectPoolManager.Instance.Pool_RetinueCardSelectPool.AllocateGameObject<CardRetinue>(parent);
                    break;
            }

            newCard.transform.localScale = Vector3.one * 120;
            newCard.transform.rotation = Quaternion.Euler(90, 0, 0);
            newCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#A1F7FF"));

            newCard.CoinText.text = cardInfo.BaseInfo.Coin.ToString();
            newCard.CoinImage.enabled = true;
            newCard.CoinImageBG.enabled = false;
            newCard.CoinImageBG.gameObject.SetActive(true);
            if (clientPlayer == null || clientPlayer.WhichPlayer == Players.Self)
            {
                if (newCard.CardBackRenderer) newCard.CardBackRenderer.gameObject.SetActive(false);
            }
            else
            {
                if (newCard.CardBackRenderer) newCard.CardBackRenderer.gameObject.SetActive(true);
            }

            newCard.M_BoxCollider.enabled = true;
        }

        newCard.Initiate(cardInfo, clientPlayer, isCardSelect);
        newCard.Usable = false;

        return newCard;
    }

    private float MainboardEmissionIntensity = 0f;

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        IsCardSelect = isCardSelect;
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo.Clone();
        if (Block_Count)
        {
            ClientUtils.InitiateNumbers(ref CardNumberSet_Count, NumberSize.Big, CardNumberSet.TextAlign.Center, Block_Count, 'x');
            CardNumberSet_Count.Clear();
        }

        M_Metal = CardInfo.BaseInfo.Metal;
        M_Energy = CardInfo.BaseInfo.Energy;
        M_Name = (GameManager.Instance.isEnglish ? CardInfo.BaseInfo.CardName_en : CardInfo.BaseInfo.CardName) + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow(GameManager.Instance.isEnglish);
        Text_CardType.text = CardInfo.GetCardTypeDesc(GameManager.Instance.isEnglish);
        Text_CardTypeBG.text = CardInfo.GetCardTypeDesc(GameManager.Instance.isEnglish);

        Color cardColor = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        MainboardEmissionIntensity = CardInfo.GetCardColorIntensity();
        Text_CardType.color = ClientUtils.ChangeColorToWhite(cardColor, 0.3f);
        ClientUtils.ChangePicture(Picture, CardInfo.BaseInfo.PictureID);
        Stars = CardInfo.UpgradeInfo.CardLevel;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);

        if (IsCardSelect) CoinText.text = CardInfo.BaseInfo.Coin.ToString();

        SetCardBackColor();
        ChangeColor(cardColor);
    }

    #region 属性

    private int m_Metal;

    public int M_Metal
    {
        get { return m_Metal; }
        set
        {
            if (value != 0)
            {
                if (!MetalIcon.gameObject.activeSelf) MetalIcon.gameObject.SetActive(true);
            }
            else
            {
                if (MetalIcon.gameObject.activeSelf) MetalIcon.gameObject.SetActive(false);
            }

            if (m_Metal != value)
            {
                m_Metal = value;
                CardInfo.BaseInfo.Metal = value;
                Text_Metal.text = value.ToString();
            }
        }
    }

    private int m_Energy;

    public int M_Energy
    {
        get { return m_Energy; }
        set
        {
            if (value != 0)
            {
                if (!EnergyIcon.gameObject.activeSelf) EnergyIcon.gameObject.SetActive(true);
            }
            else
            {
                if (EnergyIcon.gameObject.activeSelf) EnergyIcon.gameObject.SetActive(false);
            }

            if (m_Energy != value)
            {
                m_Energy = value;
                CardInfo.BaseInfo.Energy = value;
                Text_Energy.text = value.ToString();
            }
        }
    }

    private int m_EffectFactor;

    public int M_EffectFactor
    {
        get { return m_EffectFactor; }
        set
        {
            m_EffectFactor = value;
            if (CardInfo.BaseInfo.CardType == CardTypes.Spell || CardInfo.BaseInfo.CardType == CardTypes.Energy)
            {
                foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
                {
                    if (see.SideEffectBase is IEffectFactor)
                    {
                        ((IEffectFactor) see.SideEffectBase).SetEffetFactor(value);
                    }
                }
            }
        }
    }

    private string m_Name;

    public string M_Name
    {
        get { return m_Name; }
        set
        {
            m_Name = value;
            Text_Name.text = value;
        }
    }

    private string m_Desc;

    public string M_Desc
    {
        get { return m_Desc; }
        set
        {
            m_Desc = value;
            Text_Desc.text = GameManager.Instance.isEnglish ? value : ReplaceWrapSpace(value);
        }
    }

    public static string ReplaceWrapSpace(string src)
    {
        if (src.Contains(" "))
        {
            src = src.Replace(" ", "\u00A0");
        }

        return src;
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

    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_Desc;
    [SerializeField] private Text Text_CardType;
    [SerializeField] private Text Text_CardTypeBG;

    public Renderer MainBoardRenderer;
    public GameObject CardBloom;
    [SerializeField] private Renderer CardBackRenderer;
    public GameObject CardBackBloom;
    [SerializeField] private Renderer CardBloomRenderer;
    [SerializeField] private Renderer Picture;
    [SerializeField] private Image Image_DescPanel;

    [SerializeField] private Image LifeIcon;
    [SerializeField] private Image MetalIcon;
    [SerializeField] private Image EnergyIcon;
    [SerializeField] private Text Text_Metal;
    [SerializeField] private Text Text_Energy;

    [SerializeField] private Transform Block_Count;
    protected CardNumberSet CardNumberSet_Count;

    [SerializeField] private Text CoinText;
    [SerializeField] private Image CoinImage;
    public Image CoinImageBG;


    public void BeDimColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        ChangeColor(new Color(color.r / 2, color.g / 2, color.b / 2, color.a));
        ChangePictureColor(new Color(0.5f, 0.5f, 0.5f));
    }

    public void BeBrightColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        ChangeColor(color);
        ChangePictureColor(Color.white);
    }

    public void ChangeColor(Color color)
    {
        ClientUtils.ChangeEmissionColor(MainBoardRenderer, color, MainboardEmissionIntensity);

        if (Image_DescPanel)
        {
            Image_DescPanel.color = new Color(color.r / 3, color.g / 3, color.b / 3, 0.5f);
        }
    }

    public void ChangeCardBloomColor(Color color)
    {
        ClientUtils.ChangeColor(CardBloomRenderer, color);
    }

    public void ChangePictureColor(Color color)
    {
        ClientUtils.ChangeColor(Picture, color);
    }

    public void SetCardBackColor()
    {
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            ClientUtils.ChangeColor(CardBackRenderer, GameManager.Instance.SelfCardDeckCardColor);
        }
        else
        {
            ClientUtils.ChangeColor(CardBackRenderer, GameManager.Instance.EnemyCardDeckCardColor);
        }
    }

    public void SetBlockCountValue(int value)
    {
        if (value == 0)
        {
            CardNumberSet_Count.Clear();
        }
        else if (value > 0)
        {
            CardNumberSet_Count.hasSign = true;
            CardNumberSet_Count.IsSelect = IsCardSelect;
            CardNumberSet_Count.Number = value;
        }
    }

    [SerializeField] private GameObject Star1;
    [SerializeField] private GameObject Star2;
    [SerializeField] private GameObject Star3;

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
            if (ClientPlayer == RoundManager.Instance.CurrentClientPlayer)
            {
                if (!value)
                {
                    BeDimColor();
                    CardBloom.SetActive(false);
                }
                else
                {
                    BeBrightColor();
                    CardBloom.SetActive(true);
                }
            }
            else
            {
                BeBrightColor();
                CardBloom.SetActive(false);
            }

            usable = value;
        }
    }

    public virtual void OnBeginRound()
    {
    }

    public virtual void OnEndRound()
    {
        BeBrightColor();
    }

    public virtual void OnPlayOut()
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
            myColliderReplace.PoolRecycle();
            myColliderReplace = null;
        }
    }

    public virtual void DragComponent_OnMouseDown()
    {
        ClientPlayer.MyHandManager.IsBeginDrag = true;
        iTween.Stop(gameObject);
    }

    public virtual void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
    }

    public virtual void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        ClientPlayer.MyHandManager.IsBeginDrag = false;
    }

    public virtual void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = Usable && ClientPlayer.MyHandManager.CurrentFocusCard == this && ClientPlayer == RoundManager.Instance.SelfClientPlayer;
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


    public virtual void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        ClientPlayer.MyHandManager.CardOnMouseEnter(this);
    }

    public virtual void MouseHoverComponent_OnHover1End()
    {
    }

    public void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover2End()
    {
    }

    public virtual void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnFocusEnd()
    {
    }

    public virtual void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public virtual void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        ClientPlayer.MyHandManager.CardOnMouseLeave(this);
        ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
    }

    #endregion

    #region  Utils

    public static void ReplaceDisplayCardOutOfView(CardBase targetCard) //检查卡牌是否在视野外，如果是则复位
    {
    }

    #endregion
}
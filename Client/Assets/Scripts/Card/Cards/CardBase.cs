using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public abstract class CardBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    protected bool IsCardSelect;
    [SerializeField] private DragComponent M_DragComponent;
    public BoxCollider M_BoxCollider;

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

        if (CardBackRenderer) CardBackRenderer.gameObject.SetActive(true);
        SetAccount(Client.Instance.Proxy.IsSuperAccount);
        gameObject.SetActive(true);
        CardBloom.SetActive(false);

        SetBanner(BannerType.None);
        SetArrow(ArrowType.None);
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
        Text_CardType.fontStyle = LanguageManager.Instance.IsEnglish ? FontStyle.Bold : FontStyle.Normal;
        Text_CardTypeBG.fontStyle = LanguageManager.Instance.IsEnglish ? FontStyle.Bold : FontStyle.Normal;
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

            if (newCard.CardBackRenderer) newCard.CardBackRenderer.gameObject.SetActive(true);
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

        newCard.SetBanner(BannerType.None);
        newCard.SetArrow(ArrowType.None);

        return newCard;
    }

    public void SetOrderInLayer(int value)
    {
        CardCanvas.overrideSorting = true;
        CardCanvas.sortingOrder = value * 3 + 1;
        CanvasSortingGroup.sortingOrder = value * 3 + 1;
        MainBoardSortingGroup.sortingOrder = value * 3;
        CardShadowSortingGroup.sortingOrder = value * 3;
        CardBloomSortingGroup.sortingOrder = value * 3;
        CardBackSortingGroup.sortingOrder = value * 3;
        CardBackBloomSortingGroup.sortingOrder = value * 3;
    }

    private float MainboardEmissionIntensity = 0f;

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect, int limitNum = -1)
    {
        IsCardSelect = isCardSelect;
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo.Clone();
        if (Block_Count)
        {
            CardNumberSet.InitiateNumbers(ref CardNumberSet_Count, NumberSize.Medium, CardNumberSet.TextAlign.Left, Block_Count, 'x');
            CardNumberSet_Count.Clear();
        }

        if (Block_CountMax)
        {
            CardNumberSet.InitiateNumbers(ref CardNumberSet_CountMax, NumberSize.Small, CardNumberSet.TextAlign.Right, Block_CountMax, '/');
            CardNumberSet_CountMax.Clear();
            if (limitNum == -1)
            {
                SetBlockCountMaxValue(cardInfo.BaseInfo.LimitNum);
            }
            else
            {
                cardInfo.BaseInfo.LimitNum = limitNum;
                SetBlockCountMaxValue(cardInfo.BaseInfo.LimitNum);
            }
        }

        M_Metal = CardInfo.BaseInfo.Metal;
        M_Energy = CardInfo.BaseInfo.Energy;
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();
        Text_CardType.text = CardInfo.GetCardTypeDesc();
        Text_CardTypeBG.text = CardInfo.GetCardTypeDesc();

        Color cardColor = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        MainboardEmissionIntensity = CardInfo.GetCardColorIntensity();
        Text_CardType.color = ClientUtils.ChangeColorToWhite(cardColor, 0.3f);
        ClientUtils.ChangeCardPicture(Picture, CardInfo.BaseInfo.PictureID);
        Stars = CardInfo.UpgradeInfo.CardLevel;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);

        if (IsCardSelect) CoinText.text = CardInfo.BaseInfo.Coin.ToString();

        if (!IsCardSelect && CardCanvas) CardCanvas.enabled = clientPlayer.WhichPlayer == Players.Self;
        SetCardBackColor();
        ChangeColor(cardColor);

        if (CardLimitCountUpButton) CardLimitCountUpButton.gameObject.SetActive(false);
        if (CardLimitCountDownButton) CardLimitCountDownButton.gameObject.SetActive(false);
        if (CriticalCardToggle) CriticalCardToggle.gameObject.SetActive(false);
    }

    public void RefreshCardText()
    {
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();
        Text_CardType.text = CardInfo.GetCardTypeDesc();
        Text_CardTypeBG.text = CardInfo.GetCardTypeDesc();
    }

    public void SetAccount(bool isSuperAccount)
    {
        if (CardLimitCountUpButton) CardLimitCountUpButton.gameObject.SetActive(isSuperAccount);
        if (CardLimitCountDownButton) CardLimitCountDownButton.gameObject.SetActive(isSuperAccount);
        if (CriticalCardToggle) CriticalCardToggle.gameObject.SetActive(isSuperAccount);
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
                        ((IEffectFactor) see.SideEffectBase).SetFactor(value);
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
            Text_Desc.text = LanguageManager.Instance.IsEnglish ? value : ReplaceWrapSpace(value);
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

    [SerializeField] private SortingGroup MainBoardSortingGroup;
    [SerializeField] private SortingGroup CardShadowSortingGroup;
    [SerializeField] private SortingGroup CardBloomSortingGroup;
    [SerializeField] private SortingGroup CardBackSortingGroup;
    [SerializeField] private SortingGroup CardBackBloomSortingGroup;
    [SerializeField] private SortingGroup CanvasSortingGroup;

    [SerializeField] private Renderer MainBoardRenderer;
    public GameObject CardBloom;
    [SerializeField] private Renderer CardBloomRenderer;
    [SerializeField] private Image Picture;

    public GameObject CardBackBloom;
    [SerializeField] private Renderer CardBackRenderer;
    [SerializeField] private Renderer CardBackBloomRenderer;

    public Canvas CardCanvas;

    [SerializeField] private Image Image_DescPanel;

    [SerializeField] private Image LifeIcon;
    [SerializeField] private Image MetalIcon;
    [SerializeField] private Image EnergyIcon;
    [SerializeField] private Text Text_Metal;
    [SerializeField] private Text Text_Energy;

    [SerializeField] private Transform Block_Count;
    [SerializeField] private Transform Block_CountMax;
    protected CardNumberSet CardNumberSet_Count;
    protected CardNumberSet CardNumberSet_CountMax;

    [SerializeField] private Text CoinText;
    [SerializeField] private Image CoinImage;
    public Image CoinImageBG;

    // 管理员权限
    [SerializeField] private Button CardLimitCountUpButton;
    [SerializeField] private Button CardLimitCountDownButton;
    [SerializeField] private Toggle CriticalCardToggle;

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
        ClientUtils.ChangeColor(CardBloomRenderer, color, 1.3f);
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

    public void SetBlockCountValue(int value, bool forceShow = false)
    {
        if (value == 0 && !forceShow)
        {
            CardNumberSet_Count.Clear();
        }
        else
        {
            CardNumberSet_Count.hasSign = true;
            CardNumberSet_Count.IsSelect = IsCardSelect;
            CardNumberSet_Count.Number = value;
        }
    }

    public void SetBlockCountMaxValue(int value, bool forceShow = false)
    {
        if (value == 0 && !forceShow)
        {
            CardNumberSet_CountMax.Clear();
        }
        else
        {
            CardNumberSet_CountMax.hasSign = true;
            CardNumberSet_CountMax.IsSelect = IsCardSelect;
            CardNumberSet_CountMax.Number = value;
        }
    }

    protected void SetBlockCountPosition(Vector3 pos)
    {
        Block_Count.localPosition = pos;
    }

    protected void SetBlockCountMaxPosition(Vector3 pos)
    {
        Block_CountMax.localPosition = pos;
    }

    [SerializeField] private RawImage Star1;
    [SerializeField] private RawImage Star2;
    [SerializeField] private RawImage Star3;

    protected int stars;

    public int Stars
    {
        get { return stars; }

        set
        {
            stars = value;
            Star1.gameObject.SetActive(true);
            Star2.gameObject.SetActive(true);
            Star3.gameObject.SetActive(true);
            switch (value)
            {
                case 0:
                    Star1.color = new Color(1, 1, 1, 0);
                    Star2.color = new Color(1, 1, 1, 0);
                    Star3.color = new Color(1, 1, 1, 0);
                    break;
                case 1:
                    Star1.color = Color.white;
                    Star2.color = Color.black;
                    Star3.color = Color.black;
                    break;
                case 2:
                    Star1.color = Color.white;
                    Star2.color = Color.white;
                    Star3.color = Color.black;
                    break;
                case 3:
                    Star1.color = Color.white;
                    Star2.color = Color.white;
                    Star3.color = Color.white;
                    break;
                default: break;
            }
        }
    }

    [SerializeField] private Image CardBanner;
    [SerializeField] private Text BannerText;

    public enum BannerType
    {
        None,
        NewCard,
    }

    public void SetBanner(BannerType bannerType)
    {
        switch (bannerType)
        {
            case BannerType.NewCard:
            {
                ShowBanner(new Color(1, 0, 0, 0.7f), LanguageManager.Instance.GetText("CardBase_NewCardBanner"));
                break;
            }
            case BannerType.None:
            {
                if (CardBanner) CardBanner.enabled = false;
                if (BannerText) BannerText.enabled = false;
                break;
            }
        }
    }

    private void ShowBanner(Color color, string text)
    {
        if (CardBanner) CardBanner.enabled = true;
        if (BannerText) BannerText.enabled = true;
        if (CardBanner) CardBanner.color = color;
        if (BannerText) BannerText.text = text;
    }

    [SerializeField] private Image CardUpgradeArrow;
    [SerializeField] private Text UpgradeText;

    public enum ArrowType
    {
        None,
        Upgrade,
    }

    public void SetArrow(ArrowType arrowType)
    {
        switch (arrowType)
        {
            case ArrowType.Upgrade:
            {
                ShowUpgradeArrow(new Color(0, 1, 0, 1f), LanguageManager.Instance.GetText("CardBase_Upgradable"));
                break;
            }
            case ArrowType.None:
            {
                if (CardUpgradeArrow) CardUpgradeArrow.enabled = false;
                if (UpgradeText) UpgradeText.enabled = false;
                break;
            }
        }
    }

    private void ShowUpgradeArrow(Color color, string text)
    {
        if (CardUpgradeArrow) CardUpgradeArrow.enabled = true;
        if (UpgradeText) UpgradeText.enabled = true;
        if (CardUpgradeArrow) CardUpgradeArrow.color = color;
        if (UpgradeText) UpgradeText.text = text;
    }

    public void ChangeCardLimit(int value, bool forceShow = false)
    {
        SetBlockCountMaxValue(value, forceShow);
        CardInfo.BaseInfo.LimitNum = value;
    }

    public void OnCardLimitCountUpButtonClick()
    {
        ChangeCardLimit(CardInfo.BaseInfo.LimitNum + 1);
        SelectBuildManager.Instance.CurrentEditBuildButton.BuildInfo.CardCountDict[CardInfo.CardID] = CardInfo.BaseInfo.LimitNum;
    }

    public void OnCardLimitCountDownButtonClick()
    {
        if (CardInfo.BaseInfo.LimitNum > 0)
        {
            ChangeCardLimit(CardInfo.BaseInfo.LimitNum - 1);
            SelectBuildManager.Instance.CurrentEditBuildButton.BuildInfo.CardCountDict[CardInfo.CardID] = CardInfo.BaseInfo.LimitNum;
        }
    }

    public void OnCriticalCardToggleChange(bool isOn)
    {
        List<int> ccids = SelectBuildManager.Instance.CurrentEditBuildButton.BuildInfo.CriticalCardIDs.ToList();
        if (CriticalCardToggle.isOn)
        {
            if (!ccids.Contains(CardInfo.CardID))
            {
                ccids.Add(CardInfo.CardID);
            }
        }
        else
        {
            if (ccids.Contains(CardInfo.CardID))
            {
                ccids.Remove(CardInfo.CardID);
            }
        }
    }

    public void SetCriticalCardToggle(bool on)
    {
        CriticalCardToggle.isOn = on;
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
            if (!RoundManager.Instance.InRound)
            {
                value = false;
            }

            if (ClientPlayer == RoundManager.Instance.CurrentClientPlayer && RoundManager.Instance.InRound)
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

    /// <summary>
    /// 只用于作为Bonus的时候
    /// </summary>
    /// <param name="isOpen"></param>
    public void SetBonusCardBloom(bool isOpen)
    {
        ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#A1F7FF"));
        CardBloom.SetActive(isOpen);
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
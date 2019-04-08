using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public abstract class CardBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    protected bool IsCardSelect;
    public CardInfo_Base CardInfo; //卡牌原始数值信息

    public Dictionary<CardComponentTypes, CardComponentBase> CardComponents = new Dictionary<CardComponentTypes, CardComponentBase>();
    [SerializeField] private DragComponent DragComponent;
    public BoxCollider M_BoxCollider;

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
        }
        else
        {
            base.PoolRecycle();
        }

        SetAccount(Client.Instance.Proxy.IsSuperAccount);
        gameObject.SetActive(true);
        CardBasicComponent.SetBloomShow(false);

        SetBannerType(CardNoticeComponent.BannerTypes.None);
        SetArrowType(CardNoticeComponent.ArrowTypes.None);
    }

    void Awake()
    {
        CardDescComponent.SetCardDescTextColor(ClientUtils.HTMLColorToColor(AllColors.ColorDict[AllColors.ColorType.CardDecsTextColor]));
    }

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, ClientPlayer clientPlayer, bool isCardSelect)
    {
        CardBase newCard;
        if (!isCardSelect)
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueCard].AllocateGameObject<CardRetinue>(parent);
                    break;
                case CardTypes.Equip:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.EquipCard].AllocateGameObject<CardEquip>(parent);
                    ((CardEquip) newCard).M_EquipType = cardInfo.EquipInfo.SlotType;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SpellCard].AllocateGameObject<CardSpell>(parent);
                    break;
                case CardTypes.Energy:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SpellCard].AllocateGameObject<CardSpell>(parent);
                    break;
                default:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueCard].AllocateGameObject<CardRetinue>(parent);
                    break;
            }

            newCard.ChangeCardBloomColor(GameManager.Instance.CardBloomColor);

            newCard.DragComponent.enabled = true;
            newCard.M_BoxCollider.enabled = true;
        }
        else
        {
            switch (cardInfo.BaseInfo.CardType)
            {
                case CardTypes.Retinue:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueCard].AllocateGameObject<CardRetinue>(parent);
                    break;
                case CardTypes.Equip:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.EquipCard].AllocateGameObject<CardEquip>(parent);
                    ((CardEquip) newCard).M_EquipType = cardInfo.EquipInfo.SlotType;
                    break;
                case CardTypes.Spell:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SpellCard].AllocateGameObject<CardSpell>(parent);
                    break;
                case CardTypes.Energy:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SpellCard].AllocateGameObject<CardSpell>(parent);
                    break;
                default:
                    newCard = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueCard].AllocateGameObject<CardRetinue>(parent);
                    break;
            }

            newCard.transform.localScale = Vector3.one * 120;
            newCard.transform.rotation = Quaternion.Euler(90, 0, 0);
            newCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#A1F7FF"));
            newCard.CardCoinComponent.SetCoin(cardInfo.BaseInfo.Coin);
            newCard.M_BoxCollider.enabled = true;
        }

        newCard.Initiate(cardInfo, clientPlayer, isCardSelect);
        newCard.Usable = false;

        newCard.SetBannerType(CardNoticeComponent.BannerTypes.None);
        newCard.SetArrowType(CardNoticeComponent.ArrowTypes.None);

        return newCard;
    }

    public void SetOrderInLayer(int value)
    {
        foreach (KeyValuePair<CardComponentTypes, CardComponentBase> kv in CardComponents)
        {
            kv.Value.CardOrder = value;
        }
    }

    private float MainboardEmissionIntensity = 0f;

    public virtual void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect, int limitNum = -1)
    {
        IsCardSelect = isCardSelect;
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo.Clone();
        //TODO limitnum set

        M_Metal = CardInfo.BaseInfo.Metal;
        M_Energy = CardInfo.BaseInfo.Energy;
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();
        CardDescComponent.SetCardTypeText(CardInfo.GetCardTypeDesc());

        Color cardColor = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        MainboardEmissionIntensity = CardInfo.GetCardColorIntensity();
        CardDescComponent.SetCardTypeTextColor(ClientUtils.ChangeColorToWhite(cardColor, 0.3f));
        CardBasicComponent.ChangePicture(CardInfo.BaseInfo.PictureID);
        SetStarNumber(CardInfo.UpgradeInfo.CardLevel);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);

        if (IsCardSelect) CardCoinComponent.SetCoin(CardInfo.BaseInfo.Coin);

        SetCardBackColor();
        ChangeColor(cardColor);

        if (CardLimitCountUpButton) CardLimitCountUpButton.gameObject.SetActive(false);
        if (CardLimitCountDownButton) CardLimitCountDownButton.gameObject.SetActive(false);
        if (CriticalCardToggle) CriticalCardToggle.gameObject.SetActive(false);
    }

    public void RefreshCardTextLanguage()
    {
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();
        CardDescComponent.SetCardTypeText(CardInfo.GetCardTypeDesc());
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
            if (m_Metal != value)
            {
                m_Metal = value;
                CardInfo.BaseInfo.Metal = value;
                CardCostIconComponent.SetMetal(m_Metal);
            }
        }
    }

    private int m_Energy;

    public int M_Energy
    {
        get { return m_Energy; }
        set
        {
            if (m_Energy != value)
            {
                m_Energy = value;
                CardInfo.BaseInfo.Energy = value;
                CardCostIconComponent.SetEnergy(m_Energy);
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
            CardDescComponent.SetCardName(value);
        }
    }

    private string m_Desc;

    public string M_Desc
    {
        get { return m_Desc; }
        set
        {
            m_Desc = value;
            CardDescComponent.SetDescText(LanguageManager.Instance.IsEnglish ? value : ReplaceWrapSpace(value));
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

    [SerializeField] private CardBasicComponent CardBasicComponent;
    [SerializeField] private CardDescComponent CardDescComponent;
    [SerializeField] private CardBackComponent CardBackComponent;
    [SerializeField] private CardCostIconComponent CardCostIconComponent;
    [SerializeField] private CardSelectCountComponent CardSelectCountComponent;
    [SerializeField] private CardStarsComponent CardStarsComponent;
    [SerializeField] private CardNoticeComponent CardNoticeComponent;
    [SerializeField] private CardCoinComponent CardCoinComponent;

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
        CardBasicComponent.SetMainBoardColor(color, MainboardEmissionIntensity);
        CardDescComponent.SetCardDescBGColor(new Color(color.r / 3, color.g / 3, color.b / 3, 0.5f));
    }

    public void ChangeCardBloomColor(Color color)
    {
        CardBasicComponent.SetCardBloomColor(color, 1.3f);
    }

    public void ChangePictureColor(Color color)
    {
        CardBasicComponent.SetPictureColor(color, 1.0f);
    }

    public void SetCardBackColor()
    {
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            CardBackComponent.SetCardBackColor(GameManager.Instance.SelfCardDeckCardColor, 1.0f);
        }
        else
        {
            CardBackComponent.SetCardBackColor(GameManager.Instance.EnemyCardDeckCardColor, 1.0f);
        }
    }

    public void SetBlockCountValue(int value, bool forceShow = false)
    {
        CardSelectCountComponent.SelectCount = value;
        CardSelectCountComponent.IsForceShow = forceShow;
    }

    public void SetBlockCountMaxValue(int value, bool forceShow = false)
    {
        CardSelectCountComponent.SelectLimitCount = value;
        CardSelectCountComponent.IsForceShow = forceShow;
    }

    public void SetSelectCountBlockPosition(CardSelectCountComponent.Position pos)
    {
        CardSelectCountComponent.SetPosition(pos);
    }

    private void SetStarNumber(int starNumber)
    {
        if (CardStarsComponent) CardStarsComponent.SetStarNumber(starNumber);
    }

    public void SetBannerType(CardNoticeComponent.BannerTypes bannerType)
    {
        if (CardNoticeComponent) CardNoticeComponent.SetBannerType(bannerType);
    }

    public void SetArrowType(CardNoticeComponent.ArrowTypes arrowType)
    {
        if (CardNoticeComponent) CardNoticeComponent.SetArrowType(arrowType);
    }

    public void ChangeCardLimit(int value, bool forceShow = false)
    {
        SetBlockCountMaxValue(value, forceShow);
        CardInfo.BaseInfo.LimitNum = value;
    }

    public void SetCoinBlockPos(CardCoinComponent.Position pos)
    {
        CardCoinComponent.SetPosition(pos);
    }

    public void OnCardLimitCountUpButtonClick()
    {
        ChangeCardLimit(CardInfo.BaseInfo.LimitNum + 1);
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[CardInfo.CardID].CardSelectUpperLimit = CardInfo.BaseInfo.LimitNum;
    }

    public void OnCardLimitCountDownButtonClick()
    {
        if (CardInfo.BaseInfo.LimitNum > 0)
        {
            ChangeCardLimit(CardInfo.BaseInfo.LimitNum - 1);
            UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[CardInfo.CardID].CardSelectUpperLimit = CardInfo.BaseInfo.LimitNum;
        }
    }

    public void SetCriticalCardToggle(bool on)
    {
        CriticalCardToggle.isOn = on;
    }

    # endregion

    #region 卡牌交互

    internal ColliderReplace myColliderReplace;

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
                    CardBasicComponent.SetBloomShow(false);
                }
                else
                {
                    BeBrightColor();
                    CardBasicComponent.SetBloomShow(true);
                }
            }
            else
            {
                BeBrightColor();
                CardBasicComponent.SetBloomShow(false);
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
        CardBasicComponent.SetBloomShow(isOpen);
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
        if (M_BoxCollider)
        {
            M_BoxCollider.enabled = true;
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
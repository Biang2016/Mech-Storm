using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class CardBase : PoolObject, IDragComponent, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    public CardInfo_Base CardInfo; //卡牌原始数值信息

    public Dictionary<CardComponentTypes, CardComponentBase> CardComponents = new Dictionary<CardComponentTypes, CardComponentBase>();
    public DragComponent DragComponent;
    public BoxCollider M_BoxCollider;

    public override void PoolRecycle()
    {
        iTween.Stop(gameObject);
        ResetColliderAndReplace();
        CardOrder = 0;
        Usable = false;
        base.PoolRecycle();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);
        DragComponent.enabled = true;
    }

    void Awake()
    {
        CardComponents.Add(CardComponentTypes.Basic, CardBasicComponent);
        CardComponents.Add(CardComponentTypes.Back, CardBackComponent);
        CardComponents.Add(CardComponentTypes.Desc, CardDescComponent);
        CardComponents.Add(CardComponentTypes.CostBlock, CardCostIconComponent);
        CardComponents.Add(CardComponentTypes.SelectCount, CardSelectCountComponent);
        CardComponents.Add(CardComponentTypes.Stars, CardStarsComponent);
        CardComponents.Add(CardComponentTypes.Notice, CardNoticeComponent);
        CardComponents.Add(CardComponentTypes.CoinBlock, CardCoinComponent);
        CardComponents.Add(CardComponentTypes.Slots, CardSlotsComponent);

        CardShowModeComponentList.Add(CardShowMode.CardBack, new List<CardComponentTypes> {CardComponentTypes.Back});
        CardShowModeComponentList.Add(CardShowMode.HandCard, new List<CardComponentTypes> {CardComponentTypes.Back, CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars});
        CardShowModeComponentList.Add(CardShowMode.ShowCard, new List<CardComponentTypes> {CardComponentTypes.Back, CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars});
        CardShowModeComponentList.Add(CardShowMode.CardPreviewBattle, new List<CardComponentTypes> {CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars});
        CardShowModeComponentList.Add(CardShowMode.CardSelect,
            new List<CardComponentTypes> {CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars, CardComponentTypes.CoinBlock, CardComponentTypes.Notice, CardComponentTypes.SelectCount});
        CardShowModeComponentList.Add(CardShowMode.CardUpgradePreview,
            new List<CardComponentTypes> {CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars, CardComponentTypes.CoinBlock, CardComponentTypes.Notice, CardComponentTypes.SelectCount});
        CardShowModeComponentList.Add(CardShowMode.SelectedCardPreview, new List<CardComponentTypes> {CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars, CardComponentTypes.CoinBlock});
        CardShowModeComponentList.Add(CardShowMode.CardReward, new List<CardComponentTypes> {CardComponentTypes.Back, CardComponentTypes.CostBlock, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars});
        CardShowModeComponentList.Add(CardShowMode.CardUpgradeAnim, new List<CardComponentTypes> {CardComponentTypes.Basic, CardComponentTypes.CostBlock, CardComponentTypes.Desc, CardComponentTypes.Slots, CardComponentTypes.Stars});
    }

    public enum CardShowMode
    {
        CardBack, //卡背
        HandCard, //游戏时手牌
        ShowCard, //游戏时出牌展示
        CardPreviewBattle, //游戏时卡牌预览
        CardSelect, //选牌界面卡牌库中的牌
        CardUpgradePreview, //选牌界面升级预览
        SelectedCardPreview, //选牌界面右侧滑动预览
        CardReward, //奖励界面卡牌
        CardUpgradeAnim, //卡牌升级画面
    }

    private Dictionary<CardShowMode, List<CardComponentTypes>> CardShowModeComponentList = new Dictionary<CardShowMode, List<CardComponentTypes>>();

    public CardShowMode M_CardShowMode;

    public void SetCardShowMode(CardShowMode value)
    {
        foreach (KeyValuePair<CardComponentTypes, CardComponentBase> kv in CardComponents)
        {
            kv.Value?.gameObject.SetActive(CardShowModeComponentList[value].Contains(kv.Key));
        }

        //TODO 特例
        ShowAllSlotBlooms(value != CardShowMode.CardSelect && value != CardShowMode.SelectedCardPreview);
        if (value == CardShowMode.CardSelect || value == CardShowMode.SelectedCardPreview) ResetCoinPosition();
        if (value == CardShowMode.CardUpgradePreview) RefreshCoinPosition();
        ShowCardBloom(value != CardShowMode.CardSelect);

        M_CardShowMode = value;
    }

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, Transform parent, CardShowMode cardShowMode, ClientPlayer clientPlayer = null)
    {
        CardBase newCard;

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

        newCard.Initiate(cardInfo, cardShowMode, clientPlayer);
        return newCard;
    }

    private int cardOrder;

    public int CardOrder
    {
        get { return cardOrder; }
        set
        {
            if (cardOrder != value)
            {
                SetOrderInLayer(value);
                cardOrder = value;
            }
        }
    }

    public void SetOrderInLayer(int value)
    {
        foreach (KeyValuePair<CardComponentTypes, CardComponentBase> kv in CardComponents)
        {
            if (kv.Value) kv.Value.CardOrder = value;
        }

        cardOrder = value;
    }

    public virtual void Initiate(CardInfo_Base cardInfo, CardShowMode cardShowMode, ClientPlayer clientPlayer = null)
    {
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo.Clone();
        SetCardShowMode(cardShowMode);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(Vector3.up, 180);
        Usable = false;
        DragComponent.enabled = true;
        M_BoxCollider.enabled = true;
        M_Metal = CardInfo.BaseInfo.Metal;
        M_Energy = CardInfo.BaseInfo.Energy;
        M_Coin = CardInfo.BaseInfo.Coin;
        string cur_Language = LanguageManager.Instance ? LanguageManager.Instance.GetCurrentLanguage() : "zh";
        M_Name = CardInfo.BaseInfo.CardNames[cur_Language] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();

        ChangeCardPicture(CardInfo.BaseInfo.PictureID);
        CardDescComponent?.SetCardTypeText(CardInfo.GetCardTypeDesc());
        SetBlockCountValue(0, true);
        SetBlockCountMaxValue(CardInfo.BaseInfo.LimitNum, true);
        SetBannerType(CardNoticeComponent.BannerTypes.None);
        SetArrowType(CardNoticeComponent.ArrowTypes.None);
        SetStarNumber(CardInfo.UpgradeInfo.CardLevel, CardInfo.UpgradeInfo.CardLevelMax);
        RefreshCardAllColors();
    }

    #region 属性

    private int m_Metal = -1;

    public int M_Metal
    {
        get { return m_Metal; }
        set
        {
            if (m_Metal != value)
            {
                m_Metal = value;
                CardInfo.BaseInfo.Metal = value;
                CardCostIconComponent?.SetMetal(m_Metal);
            }
        }
    }

    private int m_Energy = -1;

    public int M_Energy
    {
        get { return m_Energy; }
        set
        {
            if (m_Energy != value)
            {
                m_Energy = value;
                CardInfo.BaseInfo.Energy = value;
                CardCostIconComponent?.SetEnergy(m_Energy);
            }
        }
    }

    private int m_Coin = -1;

    public int M_Coin
    {
        get { return m_Coin; }
        set
        {
            if (m_Coin != value)
            {
                m_Coin = value;
                CardInfo.BaseInfo.Coin = value;
                CardCoinComponent?.SetCoin(m_Coin);
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
                    foreach (SideEffectBase se in see.SideEffectBases)
                    {
                        se.M_SideEffectParam.Factor = value;
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
            CardDescComponent?.SetCardName(value);
        }
    }

    private string m_Desc;

    public string M_Desc
    {
        get { return m_Desc; }
        set
        {
            m_Desc = value;
            CardDescComponent?.SetDescText(LanguageManager.Instance.IsEnglish ? value : ClientUtils.ReplaceWrapSpace(value));
        }
    }

    public int M_CardInstanceId;

    #endregion

    #region 卡牌上各模块及可能具有的功能

    [SerializeField] private CardBasicComponent CardBasicComponent;
    [SerializeField] private CardBackComponent CardBackComponent;
    [SerializeField] private CardDescComponent CardDescComponent;
    [SerializeField] private CardCostIconComponent CardCostIconComponent;
    [SerializeField] private CardSelectCountComponent CardSelectCountComponent;
    [SerializeField] private CardStarsComponent CardStarsComponent;
    [SerializeField] private CardNoticeComponent CardNoticeComponent;
    [SerializeField] private CardCoinComponent CardCoinComponent;
    [SerializeField] private CardSlotsComponent CardSlotsComponent;
    [SerializeField] private CardLifeComponent CardLifeComponent;

    public void ShowCardBloom(bool isShow)
    {
        CardBasicComponent?.SetBloomShow(isShow);
    }

    public void BeDimColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        ChangeMainBoardColor(new Color(color.r / 2, color.g / 2, color.b / 2, color.a));
        ChangePictureColor(new Color(0.5f, 0.5f, 0.5f));
    }

    public void BeBrightColor()
    {
        Color color = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        ChangeMainBoardColor(color);
        ChangePictureColor(Color.white);
    }

    public void ChangeMainBoardColor(Color color)
    {
        CardBasicComponent?.SetMainBoardColor(color, CardInfo.GetCardColorIntensity());
        CardDescComponent?.SetCardDescBGColor(new Color(color.r / 3, color.g / 3, color.b / 3, 0.5f));
    }

    public void RefreshCardAllColors()
    {
        Color cardColor = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        CardDescComponent?.SetCardDescTextColor(ClientUtils.HTMLColorToColor(AllColors.ColorDict[AllColors.ColorType.CardDescTextColor]));
        ChangeMainBoardColor(cardColor);
        CardDescComponent?.SetCardTypeTextColor(ClientUtils.ChangeColorToWhite(cardColor, 0.3f));
        SetCardBackColor();
        ChangeCardBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.CardBloomColor));
    }

    public void ChangeCardBloomColor(Color color)
    {
        CardBasicComponent?.SetCardBloomColor(color, 1.3f);
    }

    public void ChangePictureColor(Color color)
    {
        CardBasicComponent?.SetPictureColor(color, 1.0f);
    }

    public void ChangeCardPicture(int picID)
    {
        CardBasicComponent?.ChangePicture(picID);
    }

    public void SetCardBackColor()
    {
        CardBackComponent?.SetCardBackColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.SelfCardDeckCardColor), 1.0f);
        if (RoundManager.Instance)
        {
            if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
            {
                CardBackComponent?.SetCardBackColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.SelfCardDeckCardColor), 1.0f);
            }
            else
            {
                CardBackComponent?.SetCardBackColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.EnemyCardDeckCardColor), 1.0f);
            }
        }
    }

    public void RefreshCardTextLanguage()
    {
        string cur_Language = LanguageManager.Instance.GetCurrentLanguage();
        M_Name = CardInfo.BaseInfo.CardNames[cur_Language] + (CardInfo.BaseInfo.IsTemp ? "*" : "");
        M_Desc = CardInfo.GetCardDescShow();
        CardDescComponent?.SetCardTypeText(CardInfo.GetCardTypeDesc());
    }

    public void SetBlockCountValue(int value, bool forceShow = false)
    {
        CardSelectCountComponent?.SetSelectCount(value);
        CardSelectCountComponent?.SetForceShow(forceShow);
    }

    public void SetBlockCountMaxValue(int value, bool forceShow = false)
    {
        CardSelectCountComponent?.SetSelectLimitCount(value);
        CardSelectCountComponent?.SetForceShow(forceShow);
    }

    public void RefreshSelectCountBlockPosition()
    {
        if (CardInfo.RetinueInfo.HasSlotType(SlotTypes.MA))
        {
            CardSelectCountComponent?.SetPosition(CardSelectCountComponent.Position.Lower);
        }
        else
        {
            CardSelectCountComponent?.SetPosition(CardSelectCountComponent.Position.Higher);
        }
    }

    public void ResetSelectCountBlockPosition()
    {
        CardSelectCountComponent?.SetPosition(CardSelectCountComponent.Position.Higher);
    }

    private void SetStarNumber(int starNumber, int starMaxNumber)
    {
        CardStarsComponent?.SetStarNumber(starNumber, starMaxNumber);
    }

    public void SetBannerType(CardNoticeComponent.BannerTypes bannerType)
    {
        CardNoticeComponent?.SetBannerType(bannerType);
    }

    public void SetArrowType(CardNoticeComponent.ArrowTypes arrowType)
    {
        CardNoticeComponent?.SetArrowType(arrowType);
    }

    public void ChangeCardSelectLimit(int value, bool forceShow = false)
    {
        SetBlockCountMaxValue(value, forceShow);
        CardInfo.BaseInfo.LimitNum = value;
    }

    public void RefreshCoinPosition()
    {
        if (CardInfo.BaseInfo.CardType == CardTypes.Retinue && CardInfo.RetinueInfo.HasSlotType(SlotTypes.MA))
        {
            CardCoinComponent?.SetPosition(CardCoinComponent.Position.Lower);
        }
        else
        {
            CardCoinComponent?.SetPosition(CardCoinComponent.Position.Higher);
        }
    }

    public void ResetCoinPosition()
    {
        CardCoinComponent?.SetPosition(CardCoinComponent.Position.Higher);
    }

    public void InitSlots()
    {
        CardSlotsComponent.SetSlot(ClientPlayer, CardInfo.RetinueInfo);
    }

    public void ShowAllSlotLights(bool isShow)
    {
        CardSlotsComponent?.ShowAllSlotLights(isShow);
    }

    public void ShowAllSlotBlooms(bool isShow)
    {
        CardSlotsComponent?.ShowAllSlotBlooms(isShow);
    }

    protected void SetLifeText(int value)
    {
        CardLifeComponent.SetLife(value);
    }

    #region 编辑区

    public void OnCardLimitCountUpButtonClick()
    {
        ChangeCardSelectLimit(CardInfo.BaseInfo.LimitNum + 1);
        SelectBuildManager.Instance.CurrentEditBuildInfo.M_BuildCards.CardSelectInfos[CardInfo.CardID].CardSelectUpperLimit = CardInfo.BaseInfo.LimitNum;
    }

    public void OnCardLimitCountDownButtonClick()
    {
        if (CardInfo.BaseInfo.LimitNum > 0)
        {
            ChangeCardSelectLimit(CardInfo.BaseInfo.LimitNum - 1);
            SelectBuildManager.Instance.CurrentEditBuildInfo.M_BuildCards.CardSelectInfos[CardInfo.CardID].CardSelectUpperLimit = CardInfo.BaseInfo.LimitNum;
        }
    }

    #endregion

    #endregion

    #region 卡牌交互

    internal ColliderReplace MyColliderReplace;

    private bool usable;

    internal bool Usable
    {
        get { return usable; }

        set
        {
            if (RoundManager.Instance && ClientPlayer != null)
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
                        CardBasicComponent?.SetBloomShow(false);
                    }
                    else
                    {
                        BeBrightColor();
                        CardBasicComponent?.SetBloomShow(true);
                    }
                }
                else
                {
                    BeBrightColor();
                    CardBasicComponent?.SetBloomShow(false);
                }
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
        if (M_BoxCollider)
        {
            M_BoxCollider.enabled = true;
        }

        if (MyColliderReplace)
        {
            MyColliderReplace.PoolRecycle();
            MyColliderReplace = null;
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

    public virtual float DragComponent_DragDistance()
    {
        return 1f;
    }

    public virtual void DragComponent_DragOutEffects()
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
}
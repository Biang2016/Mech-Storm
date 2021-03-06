﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SelectBuildPanel
{
    [SerializeField] private Transform AllCardsContainer;
    [SerializeField] private GridLayoutGroup CardGridLayoutGroup;
    [SerializeField] private Text CardLibraryText;

    [SerializeField] private Button SelectAllButton;
    [SerializeField] private Button UnSelectAllButton;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button CloseButton;

    [SerializeField] private Text SelectAllCardText;
    [SerializeField] private Text UnSelectAllCardText;
    [SerializeField] private Text ConfirmSelectText;
    [SerializeField] private Text CloseText;

    [SerializeField] private Text LeftMouseButtonClickTipText;
    [SerializeField] private Text RightMouseButtonClickTipText;
    [SerializeField] private Text ESCTipText;

    void Awake_Cards()
    {
        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (CardLibraryText, "SelectBuildManagerSelect_AllCardDeckText"),

            (SelectAllCardText, "SelectBuildManagerSelect_SelectAllCardText"),
            (UnSelectAllCardText, "SelectBuildManagerSelect_UnSelectAllCardText"),
            (ConfirmSelectText, "SelectBuildManagerSelect_ConfirmSelectText"),
            (CloseText, "SelectBuildManagerSelect_CloseText"),

            (LeftMouseButtonClickTipText, "SelectBuildManagerSelect_LeftMouseButtonClickTipText"),
            (RightMouseButtonClickTipText, "SelectBuildManagerSelect_RightMouseButtonClickTipText"),
            (ESCTipText, "SelectBuildManagerSelect_ESCTipText"),
        });

        SelectAllButton.onClick.AddListener(SelectAllCard);
        UnSelectAllButton.onClick.AddListener(delegate { UnSelectAllCard(SelectCardMethods.ButtonClick); });
        ConfirmButton.onClick.AddListener(OnConfirmSubmitCardDeckButtonClick);
        CloseButton.onClick.AddListener(OnCloseButtonClick);
    }

    void Start_Cards()
    {
        InitAddAllCards();
        InitializeOnlineCardLimitDict();
    }

    void Init_Cards()
    {
        if (CurrentBuildButtons.Count == 0)
        {
            if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
            {
                SetCardLimit(StoryManager.Instance.GetStory().Base_CardLimitDict);
            }
            else if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
            {
                ShowAllOnlineCards();
            }
        }
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    void Update_Cards()
    {
        if (!IsInit) return;
        CardPreviewPanel previewPanel = UIManager.Instance.GetBaseUIForm<CardPreviewPanel>();
        ConfirmPanel confirmPanel = UIManager.Instance.GetBaseUIForm<ConfirmPanel>();
        BuildRenamePanel buildRenamePanel = UIManager.Instance.GetBaseUIForm<BuildRenamePanel>();
        if (previewPanel != null && previewPanel.gameObject.activeInHierarchy) return;
        if (confirmPanel != null && confirmPanel.gameObject.activeInHierarchy) return;
        if (buildRenamePanel != null && buildRenamePanel.gameObject.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseLeftDownCard = card;
                }
            }
            else
            {
                mouseRightDownCard = null;
                mouseLeftDownCard = null;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseRightDownCard = card;
                }
            }
            else
            {
                mouseRightDownCard = null;
                mouseLeftDownCard = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                    {
                        if (mouseLeftDownCard == card)
                        {
                            SelectCard(card, SelectCardMethods.CardClick);
                        }
                    }
                }
            }

            mouseLeftDownCard = null;
            mouseRightDownCard = null;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (Input.GetMouseButtonUp(1))
                {
                    if (card && mouseRightDownCard == card)
                    {
                        if (CurrentEditBuildButton == null)
                        {
                            OnCreateNewBuildButtonClick();
                            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_DeckCreatedPleaseSelectCards"), 0f, 1f);
                        }

                        UIManager.Instance.ShowUIForms<CardPreviewPanel>().ShowPreviewCardPanel(card, IsReadOnly);
                    }
                }
            }

            mouseLeftDownCard = null;
            mouseRightDownCard = null;
        }
    }

    void SetReadOnly_Cards(bool isReadOnly)
    {
        ConfirmButton.gameObject.SetActive(!isReadOnly && CurrentEditBuildButton);
        SelectAllButton.gameObject.SetActive(!isReadOnly && CurrentEditBuildButton);
        UnSelectAllButton.gameObject.SetActive(!isReadOnly && CurrentEditBuildButton);
    }

    #region 卡片初始化

    public void InitAddAllCards()
    {
        foreach (CardInfo_Base cardInfo in global::AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.EmptyCard) continue;
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.NoCard) continue;
            //if (cardInfo.BaseInfo.IsHide) continue;
            if (cardInfo.BaseInfo.IsTemp) continue;
            AddCardIntoCardSelectWindow(cardInfo.Clone());
        }
    }

    public void AddCardIntoCardSelectWindow(CardInfo_Base cardInfo)
    {
        CardSelectWindowCardContainer newCardContainer = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardSelectWindowCardContainer].AllocateGameObject<CardSelectWindowCardContainer>(AllCardsContainer);
        newCardContainer.Initialize(cardInfo);
        RefreshCardInSelectWindow(newCardContainer, false);
        AllCards.Add(cardInfo.CardID, newCardContainer.M_ChildCard);
        AllCardContainers.Add(cardInfo.CardID, newCardContainer);
    }

    #region 卡片的隐藏与显示

    private void HideAllCards()
    {
        foreach (CardSelectWindowCardContainer ccc in AllCardContainers.Values)
        {
            ccc.gameObject.SetActive(false);
        }

        AllShownCards.Clear();
    }

    public void ShowHiddenCard(int cardID)
    {
        CardBase cb = AllCards[cardID];
        if (!AllShownCards.ContainsKey(cardID))
        {
            AllShownCards.Add(cardID, cb);
            AllCardContainers[cardID].gameObject.SetActive(true);
        }
    }

    public void ShowAllOnlineCards()
    {
        SetCardLimit(buildInfo: null);
    }

    #endregion

    #region 卡牌数量限制

    private SortedDictionary<int, int> OnlineCardLimitDict;

    private void InitializeOnlineCardLimitDict()
    {
        OnlineCardLimitDict = new SortedDictionary<int, int>();
        foreach (CardInfo_Base cardInfo in global::AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.NoCard || cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.EmptyCard) continue;
            if (cardInfo.BaseInfo.IsHide) continue;
            OnlineCardLimitDict.Add(cardInfo.CardID, cardInfo.BaseInfo.LimitNum);
        }
    }

    public void SetCardLimit(BuildInfo buildInfo)
    {
        SortedDictionary<int, int> cardLimits;
        if (buildInfo == null)
        {
            cardLimits = OnlineCardLimitDict;
        }
        else
        {
            cardLimits = buildInfo.M_BuildCards.GetCardLimitDict();
        }

        SetCardLimit(cardLimits);
    }

    public void SetCardLimit(SortedDictionary<int, int> cardLimits)
    {
        foreach (KeyValuePair<int, int> kv in cardLimits)
        {
            int CardID = kv.Key;
            int CardLimitCount = kv.Value;

            if (AllCards.ContainsKey(CardID))
            {
                CardBase cb = AllCards[CardID];
                cb.ChangeCardSelectLimit(CardLimitCount);

                if (!AllShownCards.ContainsKey(CardID))
                {
                    if (CardLimitCount > 0)
                    {
                        AllShownCards.Add(CardID, cb);
                        AllCardContainers[CardID].gameObject.SetActive(true);
                    }
                    else
                    {
                        AllCardContainers[CardID].gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (CardLimitCount == 0)
                    {
                        AllCardContainers[CardID].gameObject.SetActive(false);
                        AllShownCards.Remove(CardID);
                    }
                }
            }
        }

        HideHigherLevelNumCardsForStoryMode();
    }

    /// <summary>
    /// 隐藏一些没有数量限额的卡牌
    /// </summary>
    private void HideNoLimitCards()
    {
        List<int> removeCards = new List<int>();
        foreach (KeyValuePair<int, CardBase> kv in AllShownCards)
        {
            if (kv.Value.CardInfo.BaseInfo.LimitNum == 0)
            {
                removeCards.Add(kv.Key);
            }
        }

        foreach (int cardID in removeCards)
        {
            AllCardContainers[cardID].gameObject.SetActive(false);
            AllShownCards.Remove(cardID);
        }
    }

    private void HideHigherLevelNumCardsForStoryMode()
    {
        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
        {
            List<int> removeCards = new List<int>();
            foreach (KeyValuePair<int, CardBase> kv in AllShownCards)
            {
                //if (kv.Value.CardInfo.BaseInfo.CardRareLevel > StoryManager.Instance.Conquered_LevelNum + 1)
                //{
                //    removeCards.Add(kv.Key);
                //}
            }

            foreach (int cardID in removeCards)
            {
                AllCardContainers[cardID].gameObject.SetActive(false);
                AllShownCards.Remove(cardID);
            }
        }
    }

    #endregion

    public void RefreshCardTextLanguage()
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            kv.Value.RefreshCardTextLanguage();
        }
    }

    private static void RefreshCardInSelectWindow(CardSelectWindowCardContainer container, bool isSelected)
    {
        if (isSelected)
        {
            container.M_ChildCard.BeBrightColor();
        }
        else
        {
            container.M_ChildCard.BeDimColor();
        }

        if (container.M_ChildCard.CardInfo.BaseInfo.LimitNum == 0)
        {
            container.gameObject.SetActive(false);
        }
    }

    #endregion

    public void UpdateCardUpgradeDegrade(bool isUpgrade, CardBase previewCard)
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerPreviewUpgrade_PleaseCreateDeck"), 0f, 1f);
            return;
        }

        int currentCardID = previewCard.CardInfo.CardID;
        int changeCardID = isUpgrade ? previewCard.CardInfo.UpgradeInfo.UpgradeCardID : previewCard.CardInfo.UpgradeInfo.DegradeCardID;
        CardInfo_Base changeCardInfo = global::AllCards.GetCard(changeCardID);

        if (previewCard.CardInfo.BaseInfo.LimitNum == 0)
        {
            return;
        }

        if (CurrentEditBuildButton.BuildInfo.M_BuildCards.GetCardLimitDict()[currentCardID] > 0)
        {
            if ((CurrentGamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) + (previewCard.CardInfo.BaseInfo.Coin - changeCardInfo.BaseInfo.Coin) < 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerPreviewUpgrade_NotEnoughBudget"), 0f, 1f);
                return;
            }
        }

        CardBase changeCard = AllCards[changeCardID];
        if (changeCard.CardInfo.BaseInfo.LimitNum == 0)
        {
            AllCardContainers[changeCardID].gameObject.SetActive(true);
            AllShownCards.Add(changeCardID, changeCard);
        }

        previewCard.ChangeCardSelectLimit(previewCard.CardInfo.BaseInfo.LimitNum - 1);
        changeCard.ChangeCardSelectLimit(changeCard.CardInfo.BaseInfo.LimitNum + 1);
        CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[previewCard.CardInfo.CardID].CardSelectUpperLimit--;
        CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[changeCardID].CardSelectUpperLimit++;

        if (GetSelectedCardCount(previewCard.CardInfo.CardID) > 0)
        {
            UnSelectCard(previewCard, SelectCardMethods.UpgradeDegrade);
            SelectCard(changeCard, SelectCardMethods.UpgradeDegrade);
            RefreshCoinLifeEnergy();
        }

        AudioManager.Instance.SoundPlay("sfx/OnMoneyChange");
        AudioManager.Instance.SoundPlay("sfx/ShowCardDetail");
        HideNoLimitCards();
    }

    public void ShowNewCardNotice()
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            kv.Value.SetBannerType(CardNoticeComponent.BannerTypes.None);
            kv.Value.SetArrowType(CardNoticeComponent.ArrowTypes.None);
        }

        foreach (int cardID in StoryManager.Instance.JustGetNewCards)
        {
            if (AllShownCards.ContainsKey(cardID))
            {
                AllCards[cardID].SetBannerType(CardNoticeComponent.BannerTypes.None);
                AllCards[cardID].SetArrowType(CardNoticeComponent.ArrowTypes.StorageIncrease);
            }
            else
            {
                AllCards[cardID].SetBannerType(CardNoticeComponent.BannerTypes.NewCard);
                AllCards[cardID].SetArrowType(CardNoticeComponent.ArrowTypes.None);
            }
        }
    }
}
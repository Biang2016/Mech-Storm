using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SelectBuildPanel
{
    [SerializeField] private Transform AllCardsContainer;
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
        UnSelectAllButton.onClick.AddListener(UnSelectAllCard);
        ConfirmButton.onClick.AddListener(OnConfirmSubmitCardDeckButtonClick);
        CloseButton.onClick.AddListener(OnCloseButtonClick);

        InitAddAllCards();
        InitializeOnlineCardLimitDict();
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    void Update_Cards()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, cardSelectLayer);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
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
            mouseDownPosition = Input.mousePosition;
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, cardSelectLayer);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    mouseRightDownCard = card;
                }
                else
                {
                    mouseRightDownCard = null;
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
            Physics.Raycast(ray, out raycast, 500f, cardSelectLayer);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                    {
                        if (mouseLeftDownCard == card)
                        {
                            SelectCard(card, false);
                        }
                    }
                }
            }
            else
            {
                UIManager.Instance.CloseUIForms<CardPreviewPanel>();
            }

            mouseLeftDownCard = null;
            mouseRightDownCard = null;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, cardSelectLayer);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (Input.GetMouseButtonUp(1))
                {
                    if (card && mouseRightDownCard == card)
                    {
                        UIManager.Instance.ShowUIForms<CardPreviewPanel>().ShowPreviewCardPanel(card, IsReadOnly);
                    }
                    else
                    {
                        UIManager.Instance.CloseUIForms<CardPreviewPanel>();
                    }
                }
            }
            else
            {
                UIManager.Instance.CloseUIForms<CardPreviewPanel>();
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
        foreach (CardInfo_Base cardInfo in AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == 999 || cardInfo.CardID == 99) continue;
            if (cardInfo.BaseInfo.Hide) continue;
            if (cardInfo.BaseInfo.IsTemp) continue;
            AddCardIntoCardSelectWindow(cardInfo.Clone());
        }
    }

    public void AddCardIntoCardSelectWindow(CardInfo_Base cardInfo)
    {
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, AllCardsContainer, null, CardBase.CardShowMode.CardSelect);
        RefreshCardInSelectWindow(newCard, false);
        allCards.Add(newCard.CardInfo.CardID, newCard);
    }

    #region 卡片的隐藏与显示

    private void HideAllCards()
    {
        foreach (CardBase cardBase in allCards.Values)
        {
            cardBase.gameObject.SetActive(false);
        }

        allShownCards.Clear();
    }

    public void ShowHiddenCard(int cardID)
    {
        CardBase cb = allCards[cardID];
        if (!allShownCards.ContainsKey(cardID))
        {
            allShownCards.Add(cardID, cb);
            cb.gameObject.SetActive(true);
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
        foreach (CardInfo_Base cardInfo in AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == 999 || cardInfo.CardID == 99) continue;
            if (cardInfo.BaseInfo.Hide) continue;
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

            if (allCards.ContainsKey(CardID))
            {
                CardBase cb = allCards[CardID];
                cb.ChangeCardSelectLimit(CardLimitCount);

                if (Client.Instance.Proxy.IsSuperAccount)
                {
                    if (!allShownCards.ContainsKey(CardID))
                    {
                        allShownCards.Add(CardID, cb);
                    }

                    cb.gameObject.SetActive(true);
                }
                else
                {
                    if (!allShownCards.ContainsKey(CardID))
                    {
                        if (CardLimitCount > 0)
                        {
                            allShownCards.Add(CardID, cb);
                            cb.gameObject.SetActive(true);
                        }
                        else
                        {
                            cb.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (CardLimitCount == 0)
                        {
                            cb.gameObject.SetActive(false);
                            allShownCards.Remove(CardID);
                        }
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
        if (!Client.Instance.Proxy.IsSuperAccount)
        {
            List<int> removeCards = new List<int>();
            foreach (KeyValuePair<int, CardBase> kv in allShownCards)
            {
                if (kv.Value.CardInfo.BaseInfo.LimitNum == 0)
                {
                    removeCards.Add(kv.Key);
                }
            }

            foreach (int cardID in removeCards)
            {
                allShownCards[cardID].gameObject.SetActive(false);
                allShownCards.Remove(cardID);
            }
        }
    }

    private void HideHigherLevelNumCardsForStoryMode()
    {
        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
        {
            List<int> removeCards = new List<int>();
            foreach (KeyValuePair<int, CardBase> kv in allShownCards)
            {
                //if (kv.Value.CardInfo.BaseInfo.CardRareLevel > StoryManager.Instance.Conquered_LevelNum + 1)
                //{
                //    removeCards.Add(kv.Key);
                //}
            }

            foreach (int cardID in removeCards)
            {
                allShownCards[cardID].gameObject.SetActive(false);
                allShownCards.Remove(cardID);
            }
        }
    }

    #endregion

    public void RefreshCardTextLanguage()
    {
        foreach (KeyValuePair<int, CardBase> kv in allCards)
        {
            kv.Value.RefreshCardTextLanguage();
        }
    }

    private static void RefreshCardInSelectWindow(CardBase newCard, bool isSelected)
    {
        newCard.transform.localScale = Vector3.one * 120;
        newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        if (isSelected)
        {
            newCard.BeBrightColor();
        }
        else
        {
            newCard.BeDimColor();
        }

        if (newCard.CardInfo.BaseInfo.LimitNum == 0)
        {
            newCard.gameObject.SetActive(false);
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
        CardInfo_Base changeCardInfo = AllCards.GetCard(changeCardID);

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

        CardBase changeCard = allCards[changeCardID];
        previewCard.ChangeCardSelectLimit(previewCard.CardInfo.BaseInfo.LimitNum - 1);
        changeCard.ChangeCardSelectLimit(changeCard.CardInfo.BaseInfo.LimitNum + 1);
        CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[previewCard.CardInfo.CardID].CardSelectUpperLimit--;
        CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[changeCardID].CardSelectUpperLimit++;
        if (GetSelectedCardCount(previewCard.CardInfo.CardID) > 0)
        {
            UnSelectCard(previewCard, false);
            SelectCard(changeCard, false);
            RefreshCoinLifeEnergy();
        }

        AudioManager.Instance.SoundPlay("sfx/OnMoneyChange");
        AudioManager.Instance.SoundPlay("sfx/ShowCardDetail");
        HideNoLimitCards();
    }

    public void ShowNewCardBanner()
    {
        foreach (KeyValuePair<int, CardBase> kv in allCards)
        {
            kv.Value.SetBannerType(CardNoticeComponent.BannerTypes.None);
        }

        foreach (int cardID in StoryManager.Instance.JustGetNewCards)
        {
            allCards[cardID].SetBannerType(CardNoticeComponent.BannerTypes.NewCard);
        }
    }

    public void ShowUpgradeCardBanner()
    {
        foreach (KeyValuePair<int, CardBase> kv in allCards)
        {
            kv.Value.SetArrowType(CardNoticeComponent.ArrowTypes.None);
        }

        foreach (int cardID in StoryManager.Instance.JustUpgradeCards)
        {
            allCards[cardID].SetArrowType(CardNoticeComponent.ArrowTypes.Upgrade);
        }
    }
}
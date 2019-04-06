using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager
{
    void Awake_Select()
    {
        Proxy.OnClientStateChange += NetworkStateChange_Select;
        SelectCardCount = 0;
        HeroCardCount = 0;

        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (CardDeckText, "SelectBuildManagerSelect_CardDeckText"),
            (DeleteCardDeckText, "SelectBuildManagerSelect_DeleteCardDeckText"),
            (LifeText, "SelectBuildManagerSelect_LifeText"),
            (EnergyText, "SelectBuildManagerSelect_EnergyText"),
            (BuggetText, "SelectBuildManagerSelect_BudgetText"),

            (DrawCardNumText, "SelectBuildManagerSelect_DrawCardNumText"),
            (AllCardDeckText, "SelectBuildManagerSelect_AllCardDeckText"),
            (CardDeckRenameText, "SelectBuildManagerSelect_CardDeckRenameText"),

            (SelectAllCardText, "SelectBuildManagerSelect_SelectAllCardText"),
            (UnSelectAllCardText, "SelectBuildManagerSelect_UnSelectAllCardText"),
            (ConfirmSelectText, "SelectBuildManagerSelect_ConfirmSelectText"),
            (CloseText, "SelectBuildManagerSelect_CloseText"),

            (HerosCardText, "SelectBuildManagerSelect_HeroesCardText"),
            (OtherCardText, "SelectBuildManagerSelect_OtherCardText"),
            (HerosCardCountText, "SelectBuildManagerSelect_HeroesCardCountText"),
            (OtherCardCountText, "SelectBuildManagerSelect_OtherCardCountText"),

            (LeftMouseButtonClickTipText, "SelectBuildManagerSelect_LeftMouseButtonClickTipText"),
            (RightMouseButtonClickTipText, "SelectBuildManagerSelect_RightMouseButtonClickTipText"),
            (ESCTipText, "SelectBuildManagerSelect_ESCTipText"),
        });

        BuggetIcon.SetActive(!LanguageManager.Instance.IsEnglish);
    }

    void Start_Select()
    {
        ConfirmButton.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(true);
        RetinueCountMaxNumberText.text = GamePlaySettings.MaxHeroNumber.ToString();
    }

    [SerializeField] private Transform RetinueContent;
    [SerializeField] private Transform SelectionContent;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button SelectAllButton;
    [SerializeField] private Button UnSelectAllButton;
    [SerializeField] private Text CountNumberText;
    [SerializeField] private Text RetinueCountNumberText;
    [SerializeField] private Text RetinueCountMaxNumberText;
    [SerializeField] private Transform SelectCardPrefab;

    [SerializeField] private Text CardDeckText;
    [SerializeField] private Text DeleteCardDeckText;
    [SerializeField] private Text LifeText;
    [SerializeField] private Text EnergyText;
    [SerializeField] private Text BuggetText;
    [SerializeField] private GameObject BuggetIcon;
    [SerializeField] private Text DrawCardNumText;
    [SerializeField] private Text AllCardDeckText;
    [SerializeField] private Text CardDeckRenameText;

    [SerializeField] private Text SelectAllCardText;
    [SerializeField] private Text UnSelectAllCardText;
    [SerializeField] private Text ConfirmSelectText;
    [SerializeField] private Text CloseText;

    [SerializeField] private Text HerosCardText;
    [SerializeField] private Text OtherCardText;
    [SerializeField] private Text HerosCardCountText;
    [SerializeField] private Text OtherCardCountText;

    CardBase currentPreviewCard;
    [SerializeField] private Transform currentPreviewCardContainer;
    [SerializeField] private Transform CurrentPreviewCardMaxPivot;
    [SerializeField] private Transform CurrentPreviewCardMinPivot;

    [SerializeField] private Text LeftMouseButtonClickTipText;
    [SerializeField] private Text RightMouseButtonClickTipText;
    [SerializeField] private Text ESCTipText;

    public void NetworkStateChange_Select(ProxyBase.ClientStates clientState)
    {
        bool isConnected = clientState == ProxyBase.ClientStates.Login || clientState == ProxyBase.ClientStates.Login;
        ConfirmButton.gameObject.SetActive(isConnected);
        CloseButton.gameObject.SetActive(!isConnected);
    }

    #region 选择卡片

    public Dictionary<int, CardBase> allCards = new Dictionary<int, CardBase>(); // 所有卡片都放入窗口，按需隐藏
    public Dictionary<int, CardBase> allUnlockedCards = new Dictionary<int, CardBase>(); // 所有显示的卡片
    private Dictionary<int, SelectCard> SelectedCards = new Dictionary<int, SelectCard>();
    private Dictionary<int, SelectCard> SelectedHeros = new Dictionary<int, SelectCard>();

    private int selectCardCount;

    public int SelectCardCount
    {
        get { return selectCardCount; }
        set
        {
            selectCardCount = value;
            CountNumberText.text = selectCardCount.ToString();
        }
    }

    private bool isSelectedHeroFull = false;
    private bool isSelectedHeroEmpty = true;
    private int _heroCardCount;

    public int HeroCardCount
    {
        get { return _heroCardCount; }
        set
        {
            _heroCardCount = value;
            RetinueCountNumberText.text = _heroCardCount.ToString();
            isSelectedHeroFull = _heroCardCount >= GamePlaySettings.MaxHeroNumber;
            isSelectedHeroEmpty = _heroCardCount == 0;
        }
    }

    private void SelectCard(CardBase card, bool isSelectAll)
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            if (Client.Instance.IsPlaying()) NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_Notice_LoginMenu_ClientNeedUpdate"), 0, 0.1f);
            else if (Client.Instance.IsMatching()) NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_CannotEditWhenPlaying"), 0, 0.1f);
            return;
        }

        if (CurrentEditBuildButton == null)
        {
            OnCreateNewBuildButtonClick();
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_DeckCreatedPleaseSelectCards"), 0f, 1f);
            return;
        }

        if (!isSwitchingBuildInfo && GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin < card.CardInfo.BaseInfo.Coin)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_BudgetLimited"), 0f, 1f);
            return;
        }

        bool isHero = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier;
        if (isHero)
        {
            if (isSelectedHeroFull)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_HeroesNumberUpperLimit"), 0, 1f);
                return;
            }

            if (SelectedHeros.ContainsKey(card.CardInfo.CardID))
            {
                if (!Client.Instance.Proxy.IsSuperAccount && SelectedHeros[card.CardInfo.CardID].Count >= card.CardInfo.BaseInfo.LimitNum)
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_OnlyTakeSeveralCards"), card.CardInfo.BaseInfo.LimitNum), 0, 0.7f);
                    return;
                }

                int count = ++SelectedHeros[card.CardInfo.CardID].Count;
                card.SetBlockCountValue(count);
            }
            else
            {
                SelectCard newSC = GenerateNewSelectCard(card);
                SelectedHeros.Add(card.CardInfo.CardID, newSC);
                if (!isSelectAll) //如果是全选，只进行一次排序
                {
                    SortSelectCards();
                }

                card.SetBlockCountValue(1);
                card.BeBrightColor();
                card.CardBloom.SetActive(true);
            }

            HeroCardCount++;
        }
        else
        {
            if (SelectedCards.ContainsKey(card.CardInfo.CardID))
            {
                if (!Client.Instance.Proxy.IsSuperAccount && SelectedCards[card.CardInfo.CardID].Count >= card.CardInfo.BaseInfo.LimitNum)
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_OnlyTakeSeveralCards"), card.CardInfo.BaseInfo.LimitNum), 0, 0.7f);
                    return;
                }

                int count = ++SelectedCards[card.CardInfo.CardID].Count;
                card.SetBlockCountValue(count);
            }
            else
            {
                SelectCard newSC = GenerateNewSelectCard(card);
                SelectedCards.Add(card.CardInfo.CardID, newSC);
                if (!isSelectAll) //如果是全选，只进行一次排序
                {
                    SortSelectCards();
                }

                card.SetBlockCountValue(1);
                card.BeBrightColor();
                card.CardBloom.SetActive(true);
            }

            SelectCardCount++;
        }

        if (!isSwitchingBuildInfo)
        {
            CurrentEditBuildButton.AddCard(card.CardInfo.CardID);
            RefreshCoinLifeEnergy();
            if (!isSelectAll) AudioManager.Instance.SoundPlay("sfx/SelectCard");
        }
    }

    private int GetSelectedCardCount(int cardID)
    {
        if (SelectedCards.ContainsKey(cardID))
        {
            return SelectedCards[cardID].Count;
        }
        else if (SelectedHeros.ContainsKey(cardID))
        {
            return SelectedHeros[cardID].Count;
        }

        return 0;
    }

    private void SortSCs(List<SelectCard> SCs)
    {
        SCs.Sort((a, b) =>
        {
            if (a.Metal == 0 && a.Energy == 0 && (b.Metal != 0 || b.Energy != 0)) return -1;
            if (a.Energy.CompareTo(b.Energy) == 0) return a.Metal.CompareTo(b.Metal);
            return a.Energy.CompareTo(b.Energy);
        });
    }

    private void SortCBs(List<CardBase> SCs)
    {
        SCs.Sort((a, b) =>
        {
            if (a.CardInfo.BaseInfo.Metal == 0 && a.CardInfo.BaseInfo.Energy == 0 && (b.CardInfo.BaseInfo.Metal != 0 || b.CardInfo.BaseInfo.Energy != 0)) return -1;
            if (a.CardInfo.BaseInfo.Energy.CompareTo(b.CardInfo.BaseInfo.Energy) == 0)
            {
                if (a.CardInfo.BaseInfo.Metal.CompareTo(b.CardInfo.BaseInfo.Metal) == 0)
                {
                    return a.CardInfo.CardID.CompareTo(b.CardInfo.CardID);
                }
                else
                {
                    return a.CardInfo.BaseInfo.Metal.CompareTo(b.CardInfo.BaseInfo.Metal);
                }
            }
            else
            {
                return a.CardInfo.BaseInfo.Energy.CompareTo(b.CardInfo.BaseInfo.Energy);
            }
        });
    }

    private void SortSelectCards()
    {
        List<SelectCard> SCs = RetinueContent.GetComponentsInChildren<SelectCard>(true).ToList();
        SortSCs(SCs);

        for (int i = 0; i < SCs.Count; i++)
        {
            SCs[i].transform.SetSiblingIndex(i);
        }

        //RetinueContent.DetachChildren();
        //foreach (SelectCard selectCard in SCs)
        //{
        //    selectCard.transform.SetParent(RetinueContent);
        //}

        SCs = SelectionContent.GetComponentsInChildren<SelectCard>(true).ToList();
        SortSCs(SCs);

        for (int i = 0; i < SCs.Count; i++)
        {
            SCs[i].transform.SetSiblingIndex(i);
        }

        //SelectionContent.DetachChildren();
        //foreach (SelectCard selectCard in SCs)
        //{
        //    selectCard.transform.SetParent(SelectionContent);
        //}
    }

    private SelectCard GenerateNewSelectCard(CardBase card)
    {
        Transform parenTransform;
        if (card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier)
        {
            parenTransform = RetinueContent;
        }
        else
        {
            parenTransform = SelectionContent;
        }

        SelectCard newSC = GameObjectPoolManager.Instance.PoolDict["SelectCard"].AllocateGameObject<SelectCard>(parenTransform);
        Color cardColor = ClientUtils.HTMLColorToColor(card.CardInfo.GetCardColor());

        newSC.Initiate(
            count: 1,
            cardInfo: card.CardInfo,
            enterHandler: SelectCardOnMouseEnter,
            leaveHandler: SelectCardOnMouseLeave,
            color: new Color(cardColor.r, cardColor.g, cardColor.b, 1f)
        );
        newSC.CardButton.onClick.AddListener(delegate { UnSelectCard(card, true); });

        return newSC;
    }

    public void RefreshSelectCardsLanguage()
    {
        foreach (KeyValuePair<int, SelectCard> kv in SelectedCards)
        {
            kv.Value.RefreshLanguage();
        }
        foreach (KeyValuePair<int, SelectCard> kv in SelectedHeros)
        {
            kv.Value.RefreshLanguage();
        }
    }

    private void SelectCardOnMouseEnter(SelectCard selectCard)
    {
        if (PreviewCardOriginCardSelect != null) return;
        currentPreviewCardContainer.position = selectCard.transform.position;
        if (currentPreviewCardContainer.position.y > CurrentPreviewCardMaxPivot.position.y)
        {
            currentPreviewCardContainer.position = CurrentPreviewCardMaxPivot.position;
        }
        else if (currentPreviewCardContainer.position.y < CurrentPreviewCardMinPivot.position.y)
        {
            currentPreviewCardContainer.position = CurrentPreviewCardMinPivot.position;
        }

        currentPreviewCard = CardBase.InstantiateCardByCardInfo(selectCard.CardInfo.Clone(), currentPreviewCardContainer, null, true);
        currentPreviewCard.transform.localPosition = new Vector3(-180f, 0, -290);
        currentPreviewCard.transform.localScale = Vector3.one * 220;
        currentPreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        currentPreviewCard.BeBrightColor();
        currentPreviewCard.CardBloom.SetActive(true);
        currentPreviewCard.CoinImageBG.enabled = true;
        currentPreviewCard.CoinImageBG.gameObject.SetActive(true);
        currentPreviewCard.ChangeCardLimit(0);

        UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {selectCard.CardInfo}, null);
    }

    private void SelectCardOnMouseLeave(SelectCard selectCard)
    {
        //if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
        if (PreviewCardOriginCardSelect != null) return;
        if (currentPreviewCard) currentPreviewCard.PoolRecycle();
        UIManager.Instance.CloseUIForms<AffixPanel>();
    }

    private void UnSelectCard(CardBase card, bool playSound)
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            if (Client.Instance.IsPlaying())
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_CannotEditWhenPlaying"), 0, 0.1f);
            }
            else if (Client.Instance.IsMatching())
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_Notice_LoginMenu_ClientNeedUpdate"), 0, 0.1f);
            }

            return;
        }

        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerPreviewUpgrade_PleaseCreateDeck"), 0f, 1f);
            return;
        }

        bool isRetinue = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier;

        if (isRetinue)
        {
            int count = --SelectedHeros[card.CardInfo.CardID].Count;
            card.SetBlockCountValue(count);
            if (SelectedHeros[card.CardInfo.CardID].Count == 0)
            {
                SelectedHeros[card.CardInfo.CardID].PoolRecycle();
                SelectedHeros.Remove(card.CardInfo.CardID);
                card.BeDimColor();
                card.CardBloom.SetActive(false);
            }

            HeroCardCount--;
        }
        else
        {
            int count = --SelectedCards[card.CardInfo.CardID].Count;
            card.SetBlockCountValue(count);
            if (SelectedCards[card.CardInfo.CardID].Count == 0)
            {
                SelectedCards[card.CardInfo.CardID].PoolRecycle();
                SelectedCards.Remove(card.CardInfo.CardID);
                card.BeDimColor();
                card.CardBloom.SetActive(false);
            }

            SelectCardCount--;
        }

        if (!isSwitchingBuildInfo)
        {
            CurrentEditBuildButton.RemoveCard(card.CardInfo.CardID);
            RefreshCoinLifeEnergy();
            if (playSound) AudioManager.Instance.SoundPlay("sfx/UnSelectCard");
        }
    }

    public void SelectAllCard()
    {
        if (CurrentEditBuildButton == null)
        {
            OnCreateNewBuildButtonClick();
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerPreviewUpgrade_CreateDeckAuto"), 0f, 1f);
        }
        else
        {
            foreach (CardBase cardBase in allUnlockedCards.Values)
            {
                if (SelectedCards.ContainsKey(cardBase.CardInfo.CardID)) continue;
                if (SelectedHeros.ContainsKey(cardBase.CardInfo.CardID)) continue;
                SelectCard(cardBase, true);
            }

            SortSelectCards();
            AudioManager.Instance.SoundPlay("sfx/SelectCard");
        }
    }

    private bool isSwitchingBuildInfo;

    private void SelectCardsByBuildInfo(BuildInfo buildInfo)
    {
        isSwitchingBuildInfo = true;
        UnSelectAllCard();
        UnlockedCards(buildInfo);
        List<CardBase> selectCB = new List<CardBase>();
        foreach (int cardID in buildInfo.M_BuildCards.GetCardIDs())
        {
            CardBase cb = allCards[cardID];
            selectCB.Add(cb);
        }

        CurrentBuildButtons[buildInfo.BuildID].Initialize(buildInfo);

        SortCBs(selectCB);

        foreach (CardBase cb in selectCB)
        {
            if (cb.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !cb.CardInfo.RetinueInfo.IsSoldier)
            {
                SelectCard(cb, true);
            }
            else
            {
                SelectCard(cb, true);
            }
        }

        RefreshCoinLifeEnergy();
        RefreshCardNum();

        isSwitchingBuildInfo = false;
    }

    public void UnSelectAllCard()
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            return;
        }

        foreach (KeyValuePair<int, CardBase> kv in allCards)
        {
            kv.Value.BeDimColor();
            kv.Value.CardBloom.SetActive(false);
            kv.Value.SetBlockCountValue(0);
            if (SelectedCards.ContainsKey(kv.Key))
            {
                SelectedCards[kv.Key].PoolRecycle();
                SelectedCards.Remove(kv.Key);
            }

            if (SelectedHeros.ContainsKey(kv.Key))
            {
                SelectedHeros[kv.Key].PoolRecycle();
                SelectedHeros.Remove(kv.Key);
            }
        }

        if (!isSwitchingBuildInfo)
        {
            if (CurrentEditBuildButton != null)
            {
                CurrentEditBuildButton.BuildInfo.M_BuildCards.ClearAllCardCounts();
                CurrentEditBuildButton.RefreshCardCountText();
                CurrentEditBuildButton.BuildInfo.Life = GamePlaySettings.DefaultLife;
                CurrentEditBuildButton.BuildInfo.Energy = GamePlaySettings.DefaultEnergy;
                RefreshCoinLifeEnergy();
                AudioManager.Instance.SoundPlay("sfx/UnSelectCard");
            }
        }

        SelectCardCount = 0;
        HeroCardCount = 0;
    }

    public void OnConfirmSubmitCardDeckButtonClick()
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            return;
        }

        if (CurrentEditBuildButton == null)
        {
            OnCreateNewBuildButtonClick();
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_DeckCreatedPleaseSelectCards"), 0f, 1f);
        }
        else
        {
            Client.Instance.Proxy.OnSendBuildInfo(CurrentEditBuildButton.BuildInfo);
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_UpdateDeckSuccess"), 0, 1f);
            if (CurrentSelectedBuildButton) CurrentSelectedBuildButton.IsSelected = false;
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            M_StateMachine.SetState(StateMachine.States.Hide);
        }
    }

    public void OnCloseButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    #endregion
}
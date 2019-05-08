using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class SelectBuildPanel
{
    [SerializeField] private Transform HeroCardsContainer;
    [SerializeField] private Text HeroCardText;
    [SerializeField] private Text HeroCardCountText;
    [SerializeField] private Text HeroSelectCountNumberText;
    [SerializeField] private Text HeroCountMaxNumberText;

    [SerializeField] private Transform OtherCardsContainer;
    [SerializeField] private Text OtherCardText;
    [SerializeField] private Text OtherCardCountText;
    [SerializeField] private Text OtherSelectCountNumberText;

    [SerializeField] private Transform currentPreviewCardContainer;
    [SerializeField] private Transform CurrentPreviewCardMaxPivot;
    [SerializeField] private Transform CurrentPreviewCardMinPivot;

    CardBase currentPreviewCard;

    void Awake_SelectCards()
    {
        HeroCountMaxNumberText.text = GamePlaySettings.MaxHeroNumber.ToString();
        SelectCardCount = 0;
        HeroCardCount = 0;

        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (HeroCardText, "SelectBuildManagerSelect_HeroesCardText"),
            (OtherCardText, "SelectBuildManagerSelect_OtherCardText"),
            (HeroCardCountText, "SelectBuildManagerSelect_HeroesCardCountText"),
            (OtherCardCountText, "SelectBuildManagerSelect_OtherCardCountText"),
        });

        Dictionary<string, FontStyle> temp = new Dictionary<string, FontStyle> {{"zh", FontStyle.Normal}, {"en", FontStyle.BoldAndItalic}};
        LanguageManager.Instance.RegisterTextFontBinding(HeroCardText, temp);
        LanguageManager.Instance.RegisterTextFontBinding(OtherCardText, temp);
        LanguageManager.Instance.RegisterTextFontBinding(HeroCardCountText, temp);
        LanguageManager.Instance.RegisterTextFontBinding(OtherCardCountText, temp);
    }

    void Start_SelectCards()
    {
        BudgetIcon.SetActive(!LanguageManager.Instance.IsEnglish);
    }

    void Init_SelectCards()
    {
        BudgetIcon.SetActive(!LanguageManager.Instance.IsEnglish);
    }

    void SetReadOnly_SelectCards(bool isReadOnly)
    {
    }

    #region 选择卡片

    private Dictionary<int, SelectCard> SelectedCards = new Dictionary<int, SelectCard>();
    private Dictionary<int, SelectCard> SelectedHeroes = new Dictionary<int, SelectCard>();

    private int selectCardCount = 0;

    private int SelectCardCount
    {
        get { return selectCardCount; }
        set
        {
            selectCardCount = value;
            OtherSelectCountNumberText.text = selectCardCount.ToString();
        }
    }

    private bool isSelectedHeroFull => heroCardCount >= GamePlaySettings.MaxHeroNumber;
    private bool isSelectedHeroEmpty => heroCardCount == 0;

    private int heroCardCount = 0;

    private int HeroCardCount
    {
        get { return heroCardCount; }
        set
        {
            heroCardCount = value;
            HeroSelectCountNumberText.text = heroCardCount.ToString();
        }
    }

    private void SelectCard(CardBase card, bool isSelectAll)
    {
        if (IsReadOnly)
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

        if (!isSwitchingBuildInfo && CurrentGamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin < card.CardInfo.BaseInfo.Coin)
        {
            if (!isSelectAll)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_BudgetLimited"), 0f, 1f);
            }

            return;
        }

        bool isHero = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier;

        Dictionary<int, SelectCard> selectCards = isHero ? SelectedHeroes : SelectedCards;
        if (isHero && isSelectedHeroFull)
        {
            if (!isSelectAll)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_HeroesNumberUpperLimit"), 0, 1f);
            }

            return;
        }

        if (selectCards.ContainsKey(card.CardInfo.CardID))
        {
            SelectCard sc = selectCards[card.CardInfo.CardID];
            if (!Client.Instance.Proxy.IsSuperAccount && sc.Count >= card.CardInfo.BaseInfo.LimitNum)
            {
                if (!isSelectAll)
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_SelectBuildManagerSelect_OnlyTakeSeveralCards"), card.CardInfo.BaseInfo.LimitNum), 0, 0.7f);
                }

                return;
            }

            int count = ++sc.Count;
            card.SetBlockCountValue(count);
        }
        else
        {
            SelectCard newSC = GenerateNewSelectCard(card);
            selectCards.Add(card.CardInfo.CardID, newSC);
            if (!isSelectAll) SortSelectCards();
            card.SetBlockCountValue(1);
            card.BeBrightColor();
            card.ShowCardBloom(true);
        }

        if (isHero)
        {
            HeroCardCount++;
        }
        else
        {
            SelectCardCount++;
        }

        if (!isSwitchingBuildInfo)
        {
            CurrentEditBuildButton.AddCard(card.CardInfo.CardID);
            if (!isSelectAll)
            {
                RefreshCoinLifeEnergy();
                AudioManager.Instance.SoundPlay("sfx/SelectCard");
            }
        }
    }

    public int GetSelectedCardCount(int cardID)
    {
        if (SelectedCards.ContainsKey(cardID))
        {
            return SelectedCards[cardID].Count;
        }
        else if (SelectedHeroes.ContainsKey(cardID))
        {
            return SelectedHeroes[cardID].Count;
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
        List<SelectCard> SCs = HeroCardsContainer.GetComponentsInChildren<SelectCard>(true).ToList();
        SortSCs(SCs);

        for (int i = 0; i < SCs.Count; i++)
        {
            SCs[i].transform.SetSiblingIndex(i);
        }

        SCs = OtherCardsContainer.GetComponentsInChildren<SelectCard>(true).ToList();
        SortSCs(SCs);

        for (int i = 0; i < SCs.Count; i++)
        {
            SCs[i].transform.SetSiblingIndex(i);
        }
    }

    private SelectCard GenerateNewSelectCard(CardBase card)
    {
        Transform parenTransform;
        if (card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier)
        {
            parenTransform = HeroCardsContainer;
        }
        else
        {
            parenTransform = OtherCardsContainer;
        }

        SelectCard newSC = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SelectCard].AllocateGameObject<SelectCard>(parenTransform);
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

    public void RefreshSelectCardTextLanguage()
    {
        foreach (KeyValuePair<int, SelectCard> kv in SelectedCards)
        {
            kv.Value.RefreshTextLanguage();
        }

        foreach (KeyValuePair<int, SelectCard> kv in SelectedHeroes)
        {
            kv.Value.RefreshTextLanguage();
        }
    }

    [SerializeField] private RawImage SelectCardPreviewRawImage;

    private void SelectCardOnMouseEnter(SelectCard selectCard)
    {
        SelectCardPreviewRawImage.enabled = true;
        if (UIManager.Instance.IsPeekUIForm<CardPreviewPanel>()) return;
        if (selectCard.transform.position.y > CurrentPreviewCardMaxPivot.position.y)
        {
            currentPreviewCardContainer.position = new Vector3(selectCard.transform.position.x, CurrentPreviewCardMaxPivot.position.y, selectCard.transform.position.z);
        }
        else if (selectCard.transform.position.y < CurrentPreviewCardMinPivot.position.y)
        {
            currentPreviewCardContainer.position = new Vector3(selectCard.transform.position.x, CurrentPreviewCardMinPivot.position.y, selectCard.transform.position.z);
        }
        else
        {
            currentPreviewCardContainer.position = new Vector3(selectCard.transform.position.x, selectCard.transform.position.y, selectCard.transform.position.z);
        }

        currentPreviewCard = CardBase.InstantiateCardByCardInfo(selectCard.CardInfo.Clone(), currentPreviewCardContainer, CardBase.CardShowMode.SelectedCardPreview);
        currentPreviewCard.transform.localPosition = new Vector3(-180f, 0, 0);
        currentPreviewCard.transform.Translate(0, 0, -1, Space.World);
        currentPreviewCard.transform.localScale = Vector3.one * 20;
        currentPreviewCard.transform.rotation = Quaternion.Euler(0, 180, 0);
        currentPreviewCard.BeBrightColor();
        currentPreviewCard.ShowCardBloom(true);
        currentPreviewCard.ChangeCardSelectLimit(0);

        UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {selectCard.CardInfo}, null);
    }

    private void SelectCardOnMouseLeave(SelectCard selectCard)
    {
        SelectCardPreviewRawImage.enabled = false;
        if (currentPreviewCard) currentPreviewCard.PoolRecycle();
        UIManager.Instance.CloseUIForm<AffixPanel>();
    }

    private void UnSelectCard(CardBase card, bool playSound)
    {
        if (IsReadOnly)
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
            int count = --SelectedHeroes[card.CardInfo.CardID].Count;
            card.SetBlockCountValue(count);
            if (SelectedHeroes[card.CardInfo.CardID].Count == 0)
            {
                SelectedHeroes[card.CardInfo.CardID].PoolRecycle();
                SelectedHeroes.Remove(card.CardInfo.CardID);
                card.BeDimColor();
                card.ShowCardBloom(false);
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
                card.ShowCardBloom(false);
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
            foreach (CardBase cardBase in allShownCards.Values)
            {
                if (SelectedCards.ContainsKey(cardBase.CardInfo.CardID)) continue;
                if (SelectedHeroes.ContainsKey(cardBase.CardInfo.CardID)) continue;
                SelectCard(cardBase, true);
            }

            SortSelectCards();
            RefreshCoinLifeEnergy();
            AudioManager.Instance.SoundPlay("sfx/SelectCard");
        }
    }

    private bool isSwitchingBuildInfo;

    private void SelectCardsByBuildInfo(BuildInfo buildInfo)
    {
        isSwitchingBuildInfo = true;
        UnSelectAllCard();
        SetCardLimit(buildInfo);
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
        RefreshDrawCardNum();

        isSwitchingBuildInfo = false;
    }

    public void UnSelectAllCard()
    {
        if (IsReadOnly)
        {
            return;
        }

        foreach (KeyValuePair<int, CardBase> kv in allCards)
        {
            kv.Value.BeDimColor();
            kv.Value.ShowCardBloom(false);
            kv.Value.SetBlockCountValue(0);
            if (SelectedCards.ContainsKey(kv.Key))
            {
                SelectedCards[kv.Key].PoolRecycle();
                SelectedCards.Remove(kv.Key);
            }

            if (SelectedHeroes.ContainsKey(kv.Key))
            {
                SelectedHeroes[kv.Key].PoolRecycle();
                SelectedHeroes.Remove(kv.Key);
            }
        }

        if (!isSwitchingBuildInfo)
        {
            if (CurrentEditBuildButton != null)
            {
                CurrentEditBuildButton.BuildInfo.M_BuildCards.ClearAllCardCounts();
                CurrentEditBuildButton.RefreshCardCountText();
                CurrentEditBuildButton.BuildInfo.Life = CurrentGamePlaySettings.DefaultLife;
                CurrentEditBuildButton.BuildInfo.Energy = CurrentGamePlaySettings.DefaultEnergy;
                RefreshCoinLifeEnergy();
                AudioManager.Instance.SoundPlay("sfx/UnSelectCard");
            }
        }

        SelectCardCount = 0;
        HeroCardCount = 0;
    }

    public void OnConfirmSubmitCardDeckButtonClick()
    {
        if (IsReadOnly)
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
            CloseUIForm();
        }
    }

    public void OnCloseButtonClick()
    {
        CloseUIForm();
    }

    #endregion
}
using System;
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

        CardDeckText.text = GameManager.Instance.isEnglish ? "Decks" : "卡 组";
        DeleteCardDeckText.text = GameManager.Instance.isEnglish ? "Delete" : "删除卡组";
        LifeText.text = GameManager.Instance.isEnglish ? "Life" : "生命值";
        EnergyText.text = GameManager.Instance.isEnglish ? "Energy" : "能量";
        BuggetText.text = GameManager.Instance.isEnglish ? "Bugget" : "预算";
        BuggetIcon.SetActive(!GameManager.Instance.isEnglish);
        DrawCardNumText.text = GameManager.Instance.isEnglish ? "Number of Draws Per Round" : "每回合抽牌数";
        AllCardDeckText.text = GameManager.Instance.isEnglish ? "Card Library" : "牌 库";
        CardDeckRenameText.text = GameManager.Instance.isEnglish ? "Rename Deck" : "卡组命名";

        SelectAllCardText.text = GameManager.Instance.isEnglish ? "Select All" : "全选";
        UnSelectAllCardText.text = GameManager.Instance.isEnglish ? "Unselect All" : "清除选择";
        ConfirmSelectText.text = GameManager.Instance.isEnglish ? "Confirm" : "确认出战";
        CloseText.text = GameManager.Instance.isEnglish ? "Close (Tab)" : "关闭(Tab)";

        HerosCardText.text = GameManager.Instance.isEnglish ? "HeroMech" : "英 雄 随 从";
        OtherCardText.text = GameManager.Instance.isEnglish ? "Other Cards" : "其 他 卡 片";
        HerosCardCountText.text = GameManager.Instance.isEnglish ? "Total:" : "总计";
        OtherCardCountText.text = GameManager.Instance.isEnglish ? "Total:" : "数量";
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


    public void NetworkStateChange_Select(ProxyBase.ClientStates clientState)
    {
        bool isConnected = clientState == ProxyBase.ClientStates.Login || clientState == ProxyBase.ClientStates.Login;
        ConfirmButton.gameObject.SetActive(isConnected);
        CloseButton.gameObject.SetActive(!isConnected);
    }

    #region 选择卡片

    public Dictionary<int, CardBase> allCards = new Dictionary<int, CardBase>();
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

    private void SelectCard(CardBase card, bool playSound)
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            return;
        }

        if (CurrentEditBuildButton == null)
        {
            OnCreateNewBuildButtonClick();
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Card group created. You can choose cards now." : "已创建卡组，请挑选卡片", 0f, 1f);
            return;
        }

        if (!isSwitchingBuildInfo && GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin() < card.CardInfo.BaseInfo.Coin)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Bugget is Limited" : "预算不足", 0f, 1f);
            return;
        }

        bool isHero = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.RetinueInfo.IsSoldier;
        if (isHero)
        {
            if (isSelectedHeroFull)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "The number of HeroMeches reaches the upper limit." : "可携带英雄卡牌数量已达上限", 0, 1f);
                return;
            }

            if (SelectedHeros.ContainsKey(card.CardInfo.CardID))
            {
                int count = ++SelectedHeros[card.CardInfo.CardID].Count;
                card.SetBlockCountValue(count);
            }
            else
            {
                SelectCard retinueSelect = GenerateNewSelectCard(card, RetinueContent);
                SelectedHeros.Add(card.CardInfo.CardID, retinueSelect);
                List<SelectCard> SCs = RetinueContent.GetComponentsInChildren<SelectCard>(true).ToList();
                SortSelectCards(SCs);
                RetinueContent.DetachChildren();
                foreach (SelectCard selectCard in SCs)
                {
                    selectCard.transform.SetParent(RetinueContent);
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
                int count = ++SelectedCards[card.CardInfo.CardID].Count;
                card.SetBlockCountValue(count);
            }
            else
            {
                SelectCard newSC = GenerateNewSelectCard(card, SelectionContent);
                SelectedCards.Add(card.CardInfo.CardID, newSC);
                List<SelectCard> SCs = SelectionContent.GetComponentsInChildren<SelectCard>(true).ToList();
                SortSelectCards(SCs);
                SelectionContent.DetachChildren();
                foreach (SelectCard selectCard in SCs)
                {
                    selectCard.transform.SetParent(SelectionContent);
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
            CurrentEditBuildButton.BuildInfo.CardConsumeCoin += card.CardInfo.BaseInfo.Coin;
            RefreshCoinLifeEnergy();
            if (playSound) AudioManager.Instance.SoundPlay("sfx/SelectCard");
        }
    }

    private static void SortSelectCards(List<SelectCard> SCs)
    {
        SCs.Sort((a, b) =>
        {
            if (a.Metal == 0 && a.Energy == 0 && (b.Metal != 0 || b.Energy != 0)) return -1;
            if (a.Energy.CompareTo(b.Energy) == 0) return a.Metal.CompareTo(b.Metal);
            return a.Energy.CompareTo(b.Energy);
        });
    }

    private SelectCard GenerateNewSelectCard(CardBase card, Transform parenTransform)
    {
        SelectCard newSC = GameObjectPoolManager.Instance.Pool_SelectCardPool.AllocateGameObject<SelectCard>(parenTransform);
        Color cardColor = ClientUtils.HTMLColorToColor(card.CardInfo.GetCardColor());

        newSC.Initiate(
            count: 1,
            metal: card.CardInfo.BaseInfo.Metal,
            energy: card.CardInfo.BaseInfo.Energy,
            cardInfo: card.CardInfo,
            text: GameManager.Instance.isEnglish ? card.CardInfo.BaseInfo.CardName_en : card.CardInfo.BaseInfo.CardName,
            enterHandler: SelectCardOnMouseEnter,
            leaveHandler: SelectCardOnMouseLeave,
            color: new Color(cardColor.r, cardColor.g, cardColor.b, 1f)
        );
        newSC.CardButton.onClick.AddListener(delegate { UnSelectCard(card, true); });


        return newSC;
    }

    private void SelectCardOnMouseEnter(SelectCard selectCard)
    {
        currentPreviewCardContainer.position = selectCard.transform.position;
        if (currentPreviewCardContainer.position.y > CurrentPreviewCardMaxPivot.position.y)
        {
            currentPreviewCardContainer.position = CurrentPreviewCardMaxPivot.position;
        }
        else if (currentPreviewCardContainer.position.y < CurrentPreviewCardMinPivot.position.y)
        {
            currentPreviewCardContainer.position = CurrentPreviewCardMinPivot.position;
        }

        currentPreviewCard = CardBase.InstantiateCardByCardInfo(selectCard.CardInfo, currentPreviewCardContainer, null, true);
        currentPreviewCard.transform.localPosition = new Vector3(-180f, 0, -290);
        currentPreviewCard.transform.localScale = Vector3.one * 220;
        currentPreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        currentPreviewCard.BeBrightColor();
        currentPreviewCard.CardBloom.SetActive(true);
        currentPreviewCard.CoinImageBG.enabled = true;
        currentPreviewCard.CoinImageBG.gameObject.SetActive(true);

        AffixManager.Instance.ShowAffixTips(new List<CardInfo_Base> {selectCard.CardInfo});
    }

    private void SelectCardOnMouseLeave(SelectCard selectCard)
    {
        currentPreviewCard.PoolRecycle();
        AffixManager.Instance.HideAffixPanel();
    }

    private void UnSelectCard(CardBase card, bool playSound)
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly)
        {
            return;
        }

        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Please create your deck." : "请创建卡组!", 0f, 1f);
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
            CurrentEditBuildButton.BuildInfo.CardConsumeCoin -= card.CardInfo.BaseInfo.Coin;
            RefreshCoinLifeEnergy();

            if (playSound) AudioManager.Instance.SoundPlay("sfx/UnSelectCard");
        }
    }

    public void SelectAllCard()
    {
        if (CurrentEditBuildButton == null)
        {
            OnCreateNewBuildButtonClick();
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "First deck is created automatically." : "已自动为您创建第一个卡组", 0f, 1f);
        }
        else
        {
            foreach (CardBase cardBase in allCards.Values)
            {
                if (SelectedCards.ContainsKey(cardBase.CardInfo.CardID)) continue;
                if (SelectedHeros.ContainsKey(cardBase.CardInfo.CardID)) continue;
                SelectCard(cardBase, false);
            }

            AudioManager.Instance.SoundPlay("sfx/SelectCard");
        }
    }

    private bool isSwitchingBuildInfo;

    private void SelectCardsByBuildInfo(BuildInfo buildInfo)
    {
        isSwitchingBuildInfo = true;
        UnSelectAllCard();

        foreach (int cardID in buildInfo.CardIDs)
        {
            CardBase cb = null;
            if (!allCards.ContainsKey(cardID))
            {
                List<int> cardSeriesId = AllCards.GetCardSeries(cardID);
                foreach (int id in cardSeriesId)
                {
                    if (allCards.ContainsKey(id))
                    {
                        cb = allCards[id];
                        allCards.Remove(id);
                        allCards.Add(cardID, cb);
                        cb.Initiate(AllCards.GetCard(cardID), cb.ClientPlayer, true);
                        RefreshCardInSelectWindow(cb, true);
                        break;
                    }
                }
            }
            else
            {
                cb = allCards[cardID];
            }

            if (cb.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !cb.CardInfo.RetinueInfo.IsSoldier)
            {
                SelectCard(cb, false);
            }
            else
            {
                SelectCard(cb, false);
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
                CurrentEditBuildButton.BuildInfo.CardIDs.Clear();
                CurrentEditBuildButton.BuildInfo.CardConsumeCoin = 0;
                CurrentEditBuildButton.BuildInfo.Life = GamePlaySettings.PlayerDefaultLife;
                CurrentEditBuildButton.BuildInfo.Energy = GamePlaySettings.PlayerDefaultEnergy;
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
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Deck created. You can choose cards now." : "已创建卡组,请挑选卡片", 0f, 1f);
        }
        else
        {
            Client.Instance.Proxy.OnSendBuildInfo(CurrentEditBuildButton.BuildInfo);
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Update your deck successfully" : "更新卡组成功", 0, 1f);
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
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

    public void NetworkStateChange_Select(ProxyBase.ClientStates clientState)
    {
        bool isConnected = clientState == ProxyBase.ClientStates.Login || clientState == ProxyBase.ClientStates.Login;
        ConfirmButton.gameObject.SetActive(isConnected);
        CloseButton.gameObject.SetActive(!isConnected);
    }

    #region 选择卡片

    private Dictionary<int, CardBase> allCards = new Dictionary<int, CardBase>();
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

    private void SelectCard(CardBase card)
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
            return;
        }

        if (!isSwitchingBuildInfo && GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney() < card.CardInfo.BaseInfo.Money)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("预算不足", 0f, 1f);
            return;
        }

        bool isHero = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.BattleInfo.IsSoldier;
        if (isHero)
        {
            if (isSelectedHeroFull)
            {
                NoticeManager.Instance.ShowInfoPanelCenter("可携带英雄卡牌数量已达上限", 0, 1f);
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
                SCs.Sort((a, b) => a.Cost.CompareTo(b.Cost));
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
                SCs.Sort((a, b) => a.Cost.CompareTo(b.Cost));
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
            CurrentEditBuildButton.BuildInfo.CardConsumeMoney += card.CardInfo.BaseInfo.Money;
            RefreshMoneyLifeMagic();
        }
    }

    private SelectCard GenerateNewSelectCard(CardBase card, Transform parenTransform)
    {
        SelectCard newSC = GameObjectPoolManager.Instance.Pool_SelectCardPool.AllocateGameObject(parenTransform).GetComponent<SelectCard>();

        newSC.Count = 1;
        newSC.Cost = card.CardInfo.BaseInfo.Cost;
        newSC.Text_CardName.text = card.CardInfo.BaseInfo.CardName;
        newSC.CardButton.onClick.RemoveAllListeners();
        newSC.CardButton.onClick.AddListener(delegate { UnSelectCard(card); });
        Color cardColor = ClientUtils.HTMLColorToColor(card.CardInfo.BaseInfo.CardColor);
        newSC.CardButton.image.color = new Color(cardColor.r, cardColor.g, cardColor.b, 1f);
        return newSC;
    }

    private void UnSelectCard(CardBase card)
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
            return;
        }

        bool isRetinue = card.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !card.CardInfo.BattleInfo.IsSoldier;

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
            CurrentEditBuildButton.BuildInfo.CardConsumeMoney -= card.CardInfo.BaseInfo.Money;
            RefreshMoneyLifeMagic();
        }
    }

    public void SelectAllCard()
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
        }
        else
        {
            foreach (CardBase cardBase in allCards.Values)
            {
                if (SelectedCards.ContainsKey(cardBase.CardInfo.CardID)) continue;
                if (SelectedHeros.ContainsKey(cardBase.CardInfo.CardID)) continue;
                SelectCard(cardBase);
            }
        }
    }

    private bool isSwitchingBuildInfo;

    private void SelectCardsByBuildInfo(BuildInfo buildInfo)
    {
        isSwitchingBuildInfo = true;
        UnSelectAllCard();

        foreach (int cardID in buildInfo.CardIDs)
        {
            CardBase cb = allCards[cardID];
            if (cb.CardInfo.BaseInfo.CardType == CardTypes.Retinue && !cb.CardInfo.BattleInfo.IsSoldier)
            {
                SelectCard(cb);
            }
            else
            {
                SelectCard(cb);
            }
        }

        RefreshMoneyLifeMagic();

        isSwitchingBuildInfo = false;
    }

    public void UnSelectAllCard()
    {
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
                CurrentEditBuildButton.BuildInfo.CardConsumeMoney = 0;
                CurrentEditBuildButton.BuildInfo.Life = GamePlaySettings.PlayerDefaultLife;
                CurrentEditBuildButton.BuildInfo.Magic = GamePlaySettings.PlayerDefaultMagic;
                RefreshMoneyLifeMagic();
            }
        }

        SelectCardCount = 0;
        HeroCardCount = 0;
    }

    public void OnConfirmSubmitCardDeckButtonClick()
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
        }
        else
        {
            Client.Instance.Proxy.OnSendBuildInfo(CurrentEditBuildButton.BuildInfo);
            NoticeManager.Instance.ShowInfoPanelCenter("更新卡组成功", 0, 1f);
            if (CurrentSelectedBuildButton) CurrentSelectedBuildButton.IsSelected = false;
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            M_StateMachine.SetState(StateMachine.States.Hide);
        }
    }

    public void OnCloseButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        if (Client.Instance.IsLogin())
        {
            StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);
        }
    }

    #endregion
}
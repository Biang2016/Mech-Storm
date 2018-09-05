using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private Transform PreviewContent;

    private CardBase CurrentPreviewCard;
    private CardBase PreviewCard;
    private CardBase PreviewCardUpgrade;
    [SerializeField] private Image UpgradeArrow;
    private CardBase PreviewCardDegrade;
    [SerializeField] private Image DegradeArrow;

    #region 预览卡片、升级卡片

    private void Awake_PreviewAndUpgrade()
    {
        PreviewCardPanel.SetActive(false);
        PreviewCardPanelBG.SetActive(false);
    }

    private void ShowPreviewCardPanel(CardBase card)
    {
        HidePreviewCardPanel();
        CurrentPreviewCard = card;

        RefreshPreviewCard();

        RefreshUpgradePanel();

        PreviewCardPanel.SetActive(true);
        PreviewCardPanelBG.SetActive(true);
    }

    private void RefreshPreviewCard()
    {
        if (PreviewCard)
        {
            PreviewCard.CardBloom.SetActive(true);
            PreviewCard.PoolRecycle();
            PreviewCard = null;
        }

        if (PreviewCardUpgrade)
        {
            PreviewCardUpgrade.CardBloom.SetActive(true);
            PreviewCardUpgrade.PoolRecycle();
            PreviewCardUpgrade = null;
        }

        if (PreviewCardDegrade)
        {
            PreviewCardDegrade.CardBloom.SetActive(true);
            PreviewCardDegrade.PoolRecycle();
            PreviewCardDegrade = null;
        }


        PreviewCard = CardBase.InstantiateCardByCardInfo(CurrentPreviewCard.CardInfo, PreviewContent, null, true);
        PreviewCard.transform.localScale = Vector3.one * 300;
        PreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        PreviewCard.transform.localPosition = new Vector3(0, 50, -10);
        PreviewCard.CardBloom.SetActive(true);
        PreviewCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFDD8C"));

        int U_id = CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID;
        int D_id = CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID;


        if (U_id != -1)
        {
            PreviewCardUpgrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(U_id), PreviewContent, null, true);
            PreviewCardUpgrade.transform.localScale = Vector3.one * 270;
            PreviewCardUpgrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            PreviewCardUpgrade.transform.localPosition = new Vector3(500, 50, -10);
            PreviewCardUpgrade.CardBloom.SetActive(true);
            PreviewCardUpgrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FD5400"));
            UpgradeArrow.enabled = true;
        }
        else
        {
            UpgradeArrow.enabled = false;
        }


        if (D_id != -1)
        {
            PreviewCardDegrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(D_id), PreviewContent, null, true);
            PreviewCardDegrade.transform.localScale = Vector3.one * 270;
            PreviewCardDegrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            PreviewCardDegrade.transform.localPosition = new Vector3(-500, 50, -10);
            PreviewCardDegrade.CardBloom.SetActive(true);
            PreviewCardDegrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#0CE9FF"));
            DegradeArrow.enabled = true;
        }
        else
        {
            DegradeArrow.enabled = false;
        }
    }

    private void RefreshUpgradePanel()
    {
        UpgradeCardButton.onClick.RemoveAllListeners();
        DegradeCardButton.onClick.RemoveAllListeners();
        if (CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID != -1)
        {
            int moreCoin = AllCards.GetCard(CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID).BaseInfo.Coin - CurrentPreviewCard.CardInfo.BaseInfo.Coin;
            UpgradeCoinText.text = (-moreCoin).ToString();

            UpgradeCardButton.gameObject.SetActive(true);
            UpgradeCoin.enabled = true;
            UpgradeCardButton.onClick.AddListener(OnUpgradeButtonClick);
        }
        else
        {
            UpgradeCoinText.text = "";
            UpgradeCardButton.gameObject.SetActive(false);
            UpgradeCoin.enabled = false;
        }

        if (CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID != -1)
        {
            int lessCoin = AllCards.GetCard(CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID).BaseInfo.Coin - CurrentPreviewCard.CardInfo.BaseInfo.Coin;
            if (lessCoin == 0)
            {
                DegradeCoinText.text = 0.ToString();
            }
            else if (lessCoin < 0)
            {
                DegradeCoinText.text = "+" + (-lessCoin);
            }

            DegradeCardButton.gameObject.SetActive(true);
            DegradeCoin.enabled = true;
            DegradeCardButton.onClick.AddListener(OnDegradeButtonClick);
        }
        else
        {
            DegradeCoinText.text = "";
            DegradeCardButton.gameObject.SetActive(false);
            DegradeCoin.enabled = false;
        }
    }

    public void HidePreviewCardPanel()
    {
        if (PreviewCard)
        {
            PreviewCardPanel.SetActive(false);
            PreviewCardPanelBG.SetActive(false);

            PreviewCard.CardBloom.SetActive(true);
            PreviewCard.PoolRecycle();
            PreviewCard = null;
            if (PreviewCardUpgrade)
            {
                PreviewCardUpgrade.CardBloom.SetActive(true);
                PreviewCardUpgrade.PoolRecycle();
                PreviewCardUpgrade = null;
                UpgradeArrow.enabled = false;
            }

            if (PreviewCardDegrade)
            {
                PreviewCardDegrade.CardBloom.SetActive(true);
                PreviewCardDegrade.PoolRecycle();
                PreviewCardDegrade = null;
                DegradeArrow.enabled = false;
            }

            CurrentPreviewCard = null;

            UpgradeCardButton.onClick.RemoveAllListeners();
            DegradeCardButton.onClick.RemoveAllListeners();
            UpgradeCoinText.text = "";
            DegradeCoinText.text = "";
        }
    }

    public GameObject PreviewCardPanel;
    public GameObject PreviewCardPanelBG;
    public Button UpgradeCardButton;
    public Button DegradeCardButton;
    public Text UpgradeCoinText;
    public Image UpgradeCoin;
    public Text DegradeCoinText;
    public Image DegradeCoin;

    internal void OnUpgradeButtonClick()
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
            return;
        }

        int currentCardID = CurrentPreviewCard.CardInfo.CardID;
        int upgradeCardID = CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID;
        CardInfo_Base upgradeCardInfo = AllCards.GetCard(upgradeCardID);

        int cardCount = 0;
        if (SelectedCards.ContainsKey(currentCardID))
        {
            cardCount = SelectedCards[currentCardID].Count;

            if ((GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) + (CurrentPreviewCard.CardInfo.BaseInfo.Coin - upgradeCardInfo.BaseInfo.Coin) * cardCount < 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter("预算不足", 0f, 1f);
                return;
            }

            SelectCard currentSelectCard = SelectedCards[currentCardID];
            SelectedCards.Remove(currentCardID);
            SelectedCards.Add(upgradeCardID, currentSelectCard);
            currentSelectCard.Text_CardName.text = upgradeCardInfo.BaseInfo.CardName;
        }

        CurrentEditBuildButton.BuildInfo.CardConsumeCoin -= (CurrentPreviewCard.CardInfo.BaseInfo.Coin - upgradeCardInfo.BaseInfo.Coin) * cardCount;
        RefreshCoinLifeEnergy();

        CurrentPreviewCard.Initiate(upgradeCardInfo, CurrentPreviewCard.ClientPlayer, true);

        RefreshCardInSelectWindow(CurrentPreviewCard, cardCount != 0);
        RefreshUpgradePanel();
        RefreshPreviewCard();
    }

    internal void OnDegradeButtonClick()
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter("请创建卡组!", 0f, 1f);
            return;
        }

        int currentCardID = CurrentPreviewCard.CardInfo.CardID;
        int degradeCardID = CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID;
        CardInfo_Base degradeCardInfo = AllCards.GetCard(degradeCardID);

        int cardCount = 0;
        if (SelectedCards.ContainsKey(currentCardID))
        {
            cardCount = SelectedCards[currentCardID].Count;
            SelectCard currentSelectCard = SelectedCards[currentCardID];
            SelectedCards.Remove(currentCardID);
            SelectedCards.Add(degradeCardID, currentSelectCard);
            currentSelectCard.Text_CardName.text = degradeCardInfo.BaseInfo.CardName;
        }

        CurrentEditBuildButton.BuildInfo.CardConsumeCoin -= (CurrentPreviewCard.CardInfo.BaseInfo.Coin - degradeCardInfo.BaseInfo.Coin) * cardCount;
        RefreshCoinLifeEnergy();

        CurrentPreviewCard.Initiate(degradeCardInfo, CurrentPreviewCard.ClientPlayer, true);

        RefreshCardInSelectWindow(CurrentPreviewCard, cardCount != 0);
        RefreshUpgradePanel();
        RefreshPreviewCard();
    }

    #endregion
}
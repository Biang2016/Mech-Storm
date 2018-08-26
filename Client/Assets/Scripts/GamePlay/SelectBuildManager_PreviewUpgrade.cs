using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private Transform PreviewContent;

    private CardBase CurrentPreviewCard;
    private CardBase PreviewCard;

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
        }

        PreviewCard = CardBase.InstantiateCardByCardInfo(CurrentPreviewCard.CardInfo, PreviewContent, null, true);
        PreviewCard.transform.localScale = Vector3.one * 300;
        PreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        PreviewCard.transform.localPosition = new Vector3(0, 50, -10);
        PreviewCard.CardBloom.SetActive(true);
        PreviewCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFDD8C"));
    }

    private void RefreshUpgradePanel()
    {
        UpgradeCardButton.onClick.RemoveAllListeners();
        DegradeCardButton.onClick.RemoveAllListeners();
        if (CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID != -1)
        {
            int moreMoney = AllCards.GetCard(CurrentPreviewCard.CardInfo.UpgradeInfo.UpgradeCardID).BaseInfo.Money - CurrentPreviewCard.CardInfo.BaseInfo.Money;
            UpgradeCostText.text = (-moreMoney).ToString();

            UpgradeCardButton.gameObject.SetActive(true);
            UpgradeCoin.enabled = true;
            UpgradeCardButton.onClick.AddListener(OnUpgradeButtonClick);
        }
        else
        {
            UpgradeCostText.text = "";
            UpgradeCardButton.gameObject.SetActive(false);
            UpgradeCoin.enabled = false;
        }

        if (CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID != -1)
        {
            int lessMoney = AllCards.GetCard(CurrentPreviewCard.CardInfo.UpgradeInfo.DegradeCardID).BaseInfo.Money - CurrentPreviewCard.CardInfo.BaseInfo.Money;
            if (lessMoney == 0)
            {
                DegradeCostText.text = 0.ToString();
            }
            else if (lessMoney < 0)
            {
                DegradeCostText.text = "+" + (-lessMoney);
            }

            DegradeCardButton.gameObject.SetActive(true);
            DegradeCoin.enabled = true;
            DegradeCardButton.onClick.AddListener(OnDegradeButtonClick);
        }
        else
        {
            DegradeCostText.text = "";
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
            CurrentPreviewCard = null;

            UpgradeCardButton.onClick.RemoveAllListeners();
            DegradeCardButton.onClick.RemoveAllListeners();
            UpgradeCostText.text = "";
            DegradeCostText.text = "";
        }
    }

    public GameObject PreviewCardPanel;
    public GameObject PreviewCardPanelBG;
    public Button UpgradeCardButton;
    public Button DegradeCardButton;
    public Text UpgradeCostText;
    public Image UpgradeCoin;
    public Text DegradeCostText;
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

            if ((GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney()) + (CurrentPreviewCard.CardInfo.BaseInfo.Money - upgradeCardInfo.BaseInfo.Money) * cardCount < 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter("预算不足", 0f, 1f);
                return;
            }

            SelectCard currentSelectCard = SelectedCards[currentCardID];
            SelectedCards.Remove(currentCardID);
            SelectedCards.Add(upgradeCardID, currentSelectCard);
            currentSelectCard.Text_CardName.text = upgradeCardInfo.BaseInfo.CardName;
        }

        CurrentEditBuildButton.BuildInfo.CardConsumeMoney -= (CurrentPreviewCard.CardInfo.BaseInfo.Money - upgradeCardInfo.BaseInfo.Money) * cardCount;
        RefreshMoneyLifeMagic();

        CurrentPreviewCard.Initiate(upgradeCardInfo, CurrentPreviewCard.ClientPlayer, true);
        PreviewCard.Initiate(upgradeCardInfo, PreviewCard.ClientPlayer, true);

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

        CurrentEditBuildButton.BuildInfo.CardConsumeMoney -= (CurrentPreviewCard.CardInfo.BaseInfo.Money - degradeCardInfo.BaseInfo.Money) * cardCount;
        RefreshMoneyLifeMagic();

        CurrentPreviewCard.Initiate(degradeCardInfo, CurrentPreviewCard.ClientPlayer, true);
        PreviewCard.Initiate(degradeCardInfo, PreviewCard.ClientPlayer, true);

        RefreshCardInSelectWindow(CurrentPreviewCard, cardCount != 0);
        RefreshUpgradePanel();
        RefreshPreviewCard();
    }

    #endregion
}
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private Canvas PreviewCanvas;
    [SerializeField] private Transform PreviewContent;

    private CardBase PreviewCardOriginCardSelect;

    private CardBase PreviewCard;

    private CardBase PreviewCardUpgrade;
    [SerializeField] private Image UpgradeArrow;
    [SerializeField] private Transform UpgradeArrowPivot_normal;
    [SerializeField] private Transform UpgradeArrowPivot_retinueCards;

    private CardBase PreviewCardDegrade;
    [SerializeField] private Image DegradeArrow;
    [SerializeField] private Transform DegradeArrowPivot_normal;
    [SerializeField] private Transform DegradeArrowPivot_retinueCards;

    [SerializeField] private Text UpgradeText;
    [SerializeField] private Text DegradeText;

    #region 预览卡片、升级卡片

    private void Awake_PreviewAndUpgrade()
    {
        PreviewCanvas.enabled = true;
        PreviewCardPanel.SetActive(false);
        PreviewCardPanelBG.SetActive(false);

        UpgradeText.text = GameManager.Instance.IsEnglish ? "Upgrade" : "升级";
        DegradeText.text = GameManager.Instance.IsEnglish ? "Degrade" : "降级";

        UpgradeText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DegradeText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
    }

    public void ShowPreviewCardPanel(CardBase card)
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Please create a new deck first." : "请先创建一个卡组", 0, 0.5f);
            return;
        }

        HidePreviewCardPanel();
        PreviewCardOriginCardSelect = card;

        RefreshPreviewCard();

        RefreshUpgradePanel();

        PreviewCardPanel.SetActive(true);
        PreviewCardPanelBG.SetActive(true);
        AudioManager.Instance.SoundPlay("sfx/ShowCardDetail");
    }

    float normalCardPreviewDistance = 500f;
    float retinueCardPreviewDistance = 550f;

    public void HideOtherThingsExceptShowCard()
    {
        if (PreviewCard)
        {
            PreviewCard.CoinImageBG.gameObject.SetActive(false);
            DegradeArrow.enabled = false;
            UpgradeArrow.enabled = false;
            UpgradeCardButton.gameObject.SetActive(false);
            DegradeCardButton.gameObject.SetActive(false);
            AffixManager.Instance.HideAffixPanel();
            if (PreviewCardDegrade)
            {
                PreviewCardDegrade.PoolRecycle();
                PreviewCardDegrade.CoinImageBG.gameObject.SetActive(false);
                DegradeCoin.enabled = false;
                DegradeCoinText.enabled = false;
            }

            if (PreviewCardUpgrade)
            {
                PreviewCardUpgrade.PoolRecycle();
                PreviewCardUpgrade.CoinImageBG.gameObject.SetActive(false);
                UpgradeCoin.enabled = false;
                UpgradeCoinText.enabled = false;
            }
        }
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

        PreviewCard = CardBase.InstantiateCardByCardInfo(PreviewCardOriginCardSelect.CardInfo, PreviewContent, null, true);
        PreviewCard.ChangeCardLimit(CurrentEditBuildButton.BuildInfo.CardCountDict[PreviewCard.CardInfo.CardID], true);
        PreviewCard.SetBlockCountValue(GetSelectedCardCount(PreviewCard.CardInfo.CardID), true);
        PreviewCard.transform.localScale = Vector3.one * 300;
        PreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        PreviewCard.transform.localPosition = new Vector3(0, 50, -10);
        PreviewCard.CardBloom.SetActive(true);
        PreviewCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFDD8C"));
        PreviewCard.BeBrightColor();
        PreviewCard.M_BoxCollider.enabled = false;

        if (PreviewCard is CardRetinue)
        {
            ((CardRetinue) PreviewCard).ShowAllSlotHover();
            ((CardRetinue) PreviewCard).MoveCoinBGLower();
        }

        bool hasUpgradeCard = false;
        bool hasDegradeCard = false;
        if (GameMode_State == GameMode.Single)
        {
            int u_id = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID;
            int d_id = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID;
            hasUpgradeCard = u_id != -1 && AllCards.GetCard(u_id).BaseInfo.CardRareLevel <= StoryManager.Instance.Conquered_LevelNum;
            hasDegradeCard = d_id != -1 && AllCards.GetCard(d_id).BaseInfo.CardRareLevel <= StoryManager.Instance.Conquered_LevelNum;
        }
        else if (GameMode_State == GameMode.Online)
        {
            hasUpgradeCard = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID != -1;
            hasDegradeCard = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID != -1;
        }

        if (hasUpgradeCard)
        {
            PreviewCardUpgrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID), PreviewContent, null, true);
            PreviewCardUpgrade.ChangeCardLimit(CurrentEditBuildButton.BuildInfo.CardCountDict[PreviewCardUpgrade.CardInfo.CardID], true);
            PreviewCardUpgrade.SetBlockCountValue(GetSelectedCardCount(PreviewCardUpgrade.CardInfo.CardID), true);
            PreviewCardUpgrade.transform.localScale = Vector3.one * 270;
            PreviewCardUpgrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            if (PreviewCardUpgrade.CardInfo.BaseInfo.CardType == CardTypes.Retinue)
            {
                PreviewCardUpgrade.transform.localPosition = new Vector3(retinueCardPreviewDistance, 50, -10);
                UpgradeArrow.transform.position = UpgradeArrowPivot_retinueCards.position;
            }
            else
            {
                PreviewCardUpgrade.transform.localPosition = new Vector3(normalCardPreviewDistance, 50, -10);
                UpgradeArrow.transform.position = UpgradeArrowPivot_normal.position;
            }

            UpgradeArrow.enabled = true;

            PreviewCardUpgrade.CardBloom.SetActive(true);
            PreviewCardUpgrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FD5400"));
            PreviewCardUpgrade.BeBrightColor();
            PreviewCardUpgrade.M_BoxCollider.enabled = false;
            if (PreviewCardUpgrade is CardRetinue)
            {
                ((CardRetinue) PreviewCardUpgrade).ShowAllSlotHover();
                ((CardRetinue) PreviewCardUpgrade).MoveCoinBGLower();
            }
        }
        else
        {
            UpgradeArrow.enabled = false;
        }


        if (hasDegradeCard)
        {
            PreviewCardDegrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID), PreviewContent, null, true);
            PreviewCardDegrade.ChangeCardLimit(CurrentEditBuildButton.BuildInfo.CardCountDict[PreviewCardDegrade.CardInfo.CardID], true);
            PreviewCardDegrade.SetBlockCountValue(GetSelectedCardCount(PreviewCardDegrade.CardInfo.CardID), true);
            PreviewCardDegrade.transform.localScale = Vector3.one * 270;
            PreviewCardDegrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            if (PreviewCardDegrade.CardInfo.BaseInfo.CardType == CardTypes.Retinue)
            {
                PreviewCardDegrade.transform.localPosition = new Vector3(-retinueCardPreviewDistance, 50, -10);
                DegradeArrow.transform.position = DegradeArrowPivot_retinueCards.position;
            }
            else
            {
                PreviewCardDegrade.transform.localPosition = new Vector3(-normalCardPreviewDistance, 50, -10);
                DegradeArrow.transform.position = DegradeArrowPivot_normal.position;
            }

            PreviewCardDegrade.CardBloom.SetActive(true);
            PreviewCardDegrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#0CE9FF"));
            PreviewCardDegrade.BeBrightColor();
            PreviewCardDegrade.M_BoxCollider.enabled = false;
            if (PreviewCardDegrade is CardRetinue)
            {
                ((CardRetinue) PreviewCardDegrade).ShowAllSlotHover();
                ((CardRetinue) PreviewCardDegrade).MoveCoinBGLower();
            }

            DegradeArrow.enabled = true;
        }
        else
        {
            DegradeArrow.enabled = false;
        }

        List<CardInfo_Base> cardInfos = new List<CardInfo_Base>();
        cardInfos.Add(PreviewCard.CardInfo);
        if (PreviewCardDegrade) cardInfos.Add(PreviewCardDegrade.CardInfo);
        if (PreviewCardUpgrade) cardInfos.Add(PreviewCardUpgrade.CardInfo);
        bool isShowAffixTips = AffixManager.Instance.ShowAffixTips(cardInfos, null);

        //如果显示Tips占据屏幕空间的话，右移预览卡牌窗口
        if (!isShowAffixTips || PreviewCardDegrade == null)
        {
            PreviewCardPanel.transform.position = PreviewCardPanelCenterPivot.position;
        }
        else
        {
            PreviewCardPanel.transform.position = PreviewCardPanelRightPivot.position;
        }
    }

    private void RefreshUpgradePanel()
    {
        UpgradeCardButton.onClick.RemoveAllListeners();
        DegradeCardButton.onClick.RemoveAllListeners();

        bool hasUpgradeCard = false;
        bool hasDegradeCard = false;
        if (GameMode_State == GameMode.Single)
        {
            int u_id = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID;
            int d_id = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID;
            hasUpgradeCard = u_id != -1 && AllCards.GetCard(u_id).BaseInfo.CardRareLevel <= StoryManager.Instance.Conquered_LevelNum;
            hasDegradeCard = d_id != -1 && AllCards.GetCard(d_id).BaseInfo.CardRareLevel <= StoryManager.Instance.Conquered_LevelNum;
        }
        else if (GameMode_State == GameMode.Online)
        {
            hasUpgradeCard = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID != -1;
            hasDegradeCard = PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID != -1;
        }

        if (hasUpgradeCard)
        {
            int moreCoin = AllCards.GetCard(PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID).BaseInfo.Coin - PreviewCardOriginCardSelect.CardInfo.BaseInfo.Coin;
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

        if (hasDegradeCard)
        {
            int lessCoin = AllCards.GetCard(PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID).BaseInfo.Coin - PreviewCardOriginCardSelect.CardInfo.BaseInfo.Coin;
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

            PreviewCardOriginCardSelect = null;

            UpgradeCardButton.onClick.RemoveAllListeners();
            DegradeCardButton.onClick.RemoveAllListeners();
            UpgradeCoinText.text = "";
            DegradeCoinText.text = "";
        }

        AffixManager.Instance.HideAffixPanel();
    }

    public GameObject PreviewCardPanel;
    public Transform PreviewCardPanelRightPivot;
    public Transform PreviewCardPanelCenterPivot;
    public GameObject PreviewCardPanelBG;
    public Button UpgradeCardButton;
    public Button DegradeCardButton;
    public Text UpgradeCoinText;
    public Image UpgradeCoin;
    public Text DegradeCoinText;
    public Image DegradeCoin;

    internal void OnUpgradeButtonClick()
    {
        OnUpgradeDegradeCore(true);
    }

    internal void OnDegradeButtonClick()
    {
        OnUpgradeDegradeCore(false);
    }

    internal void OnUpgradeDegradeCore(bool isUpgrade)
    {
        if (CurrentEditBuildButton == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Please create your deck." : "请创建卡组!", 0f, 1f);
            return;
        }

        int currentCardID = PreviewCardOriginCardSelect.CardInfo.CardID;
        int changeCardID = isUpgrade ? PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.UpgradeCardID : PreviewCardOriginCardSelect.CardInfo.UpgradeInfo.DegradeCardID;
        CardInfo_Base changeCardInfo = AllCards.GetCard(changeCardID);

        CardBase changeCard = allCards[changeCardID];
        if (PreviewCardOriginCardSelect.CardInfo.BaseInfo.LimitNum == 0)
        {
            return;
        }

        if (!allUnlockedCards.ContainsKey(changeCardID))
        {
            allUnlockedCards.Add(changeCardID, changeCard);
            changeCard.gameObject.SetActive(true);
        }

        PreviewCardOriginCardSelect.ChangeCardLimit(PreviewCardOriginCardSelect.CardInfo.BaseInfo.LimitNum - 1);
        changeCard.ChangeCardLimit(changeCard.CardInfo.BaseInfo.LimitNum + 1);
        CurrentEditBuildButton.BuildInfo.CardCountDict[currentCardID]--;
        CurrentEditBuildButton.BuildInfo.CardCountDict[changeCardID]++;

        if (GetSelectedCardCount(currentCardID) > 0)
        {
            if ((GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin) + (PreviewCardOriginCardSelect.CardInfo.BaseInfo.Coin - changeCardInfo.BaseInfo.Coin) < 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Not enough bugget." : "预算不足", 0f, 1f);
                return;
            }

            CurrentEditBuildButton.BuildInfo.CardConsumeCoin -= PreviewCardOriginCardSelect.CardInfo.BaseInfo.Coin - changeCardInfo.BaseInfo.Coin;
            UnSelectCard(PreviewCardOriginCardSelect, false);
            SelectCard(changeCard, false);
            RefreshCoinLifeEnergy();
        }

        AudioManager.Instance.SoundPlay("sfx/OnMoneyChange");
        AudioManager.Instance.SoundPlay("sfx/ShowCardDetail");
        RefreshUpgradePanel();
        RefreshPreviewCard();
        HideNoLimitCards();
    }

    #endregion
}
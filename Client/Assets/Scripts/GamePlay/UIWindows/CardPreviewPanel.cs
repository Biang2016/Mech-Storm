using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewPanel : BaseUIForm
{
    [SerializeField] private Transform PreviewContent;
    [SerializeField] private Transform PreviewCardPanelRightPivot;
    [SerializeField] private Transform PreviewCardPanelCenterPivot;

    [SerializeField] private GameObject Others;

    [SerializeField] private Image UpgradeArrow;
    [SerializeField] private Transform UpgradeArrowPivot_normal;
    [SerializeField] private Transform UpgradeArrowPivot_retinueCards;

    [SerializeField] private Image DegradeArrow;
    [SerializeField] private Transform DegradeArrowPivot_normal;
    [SerializeField] private Transform DegradeArrowPivot_retinueCards;

    [SerializeField] private GameObject OperationUIs;
    [SerializeField] private Button UpgradeCardButton;
    [SerializeField] private Text UpgradeText;
    [SerializeField] private Text UpgradeCoinText;
    [SerializeField] private Button DegradeCardButton;
    [SerializeField] private Text DegradeText;
    [SerializeField] private Text DegradeCoinText;

    private CardBase PreviewCard_Src;
    private CardBase PreviewCard;
    private CardBase PreviewCardUpgrade;
    private CardBase PreviewCardDegrade;

    #region 预览卡片、升级卡片

    private void Awake()
    {
        UIType.IsClearStack = false;
        UIType.IsClickElsewhereClose = true;
        UIType.IsESCClose = true;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        UIType.UIForms_ShowMode = UIFormShowModes.Return;
        UIType.UIForms_Type = UIFormTypes.PopUp;

        UpgradeCardButton.onClick.AddListener(OnUpgradeButtonClick);
        DegradeCardButton.onClick.AddListener(OnDegradeButtonClick);
        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (UpgradeText, "SelectBuildManagerPreviewUpgrade_UpgradeText"),
            (DegradeText, "SelectBuildManagerPreviewUpgrade_DegradeText"),
        });
    }

    public void ShowPreviewCardPanel(CardBase card, bool isReadOnly)
    {
        //NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_SelectBuildManagerPreviewUpgrade_CreateANewDeck"), 0, 0.5f);

        OperationUIs.SetActive(!isReadOnly);

        PreviewCard_Src = card;
        card.SetBannerType(CardNoticeComponent.BannerTypes.None);
        card.SetArrowType(CardNoticeComponent.ArrowTypes.None);

        RefreshPreviewCard();
        RefreshUpgradePanel();

        StoryManager.Instance.JustUpgradeCards.Remove(card.CardInfo.CardID);
        StoryManager.Instance.JustGetNewCards.Remove(card.CardInfo.CardID);

        bool hasNewCard = StoryManager.Instance.JustGetNewCards.Count != 0 || StoryManager.Instance.JustUpgradeCards.Count != 0;
        UIManager.Instance.GetBaseUIForm<StartMenuPanel>().SetButtonTipImageShow("SingleDeckButton", hasNewCard);
        AudioManager.Instance.SoundPlay("sfx/ShowCardDetail");
    }

    public void HideOtherThingsExceptShowCard()
    {
        UIManager.Instance.CloseUIForms<AffixPanel>();
        Others.SetActive(false);
    }

    float normalCardPreviewDistance = 500f;
    float retinueCardPreviewDistance = 550f;

    public void RefreshPreviewCard()
    {
        if (PreviewCard)
        {
            PreviewCard.ShowCardBloom(true);
            PreviewCard.PoolRecycle();
            PreviewCard = null;
        }

        if (PreviewCardUpgrade)
        {
            PreviewCardUpgrade.ShowCardBloom(true);
            PreviewCardUpgrade.PoolRecycle();
            PreviewCardUpgrade = null;
        }

        if (PreviewCardDegrade)
        {
            PreviewCardDegrade.ShowCardBloom(true);
            PreviewCardDegrade.PoolRecycle();
            PreviewCardDegrade = null;
        }

        PreviewCard = CardBase.InstantiateCardByCardInfo(PreviewCard_Src.CardInfo, PreviewContent, CardBase.CardShowMode.CardUpgradePreview);
        PreviewCard.CardOrder = 1;
        PreviewCard.ChangeCardSelectLimit(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[PreviewCard.CardInfo.CardID].CardSelectUpperLimit, true);
        PreviewCard.SetBlockCountValue(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().GetSelectedCardCount(PreviewCard.CardInfo.CardID), true);
        PreviewCard.transform.localScale = Vector3.one * 20;
        //PreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        PreviewCard.transform.localPosition = new Vector3(0, 50, 0);
        PreviewCard.ShowCardBloom(true);
        PreviewCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFDD8C"));
        PreviewCard.BeBrightColor();
        PreviewCard.M_BoxCollider.enabled = false;
        PreviewCard.ShowAllSlotBlooms(true);
        PreviewCard.RefreshCoinPosition();

        bool hasUpgradeCard = false;
        bool hasDegradeCard = false;
        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
        {
            int u_id = PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID;
            int d_id = PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID;
            hasUpgradeCard = u_id != -1 && AllCards.GetCard(u_id).BaseInfo.CardRareLevel <= StoryManager.Instance.UnlockedCardLevelNum;
            hasDegradeCard = d_id != -1 && AllCards.GetCard(d_id).BaseInfo.CardRareLevel <= StoryManager.Instance.UnlockedCardLevelNum;
        }
        else if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
        {
            hasUpgradeCard = PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID != -1;
            hasDegradeCard = PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID != -1;
        }

        if (hasUpgradeCard)
        {
            PreviewCardUpgrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID), PreviewContent, CardBase.CardShowMode.CardUpgradePreview);
            PreviewCardUpgrade.CardOrder = 1;
            PreviewCardUpgrade.ChangeCardSelectLimit(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[PreviewCard.CardInfo.CardID].CardSelectUpperLimit, true);
            PreviewCardUpgrade.SetBlockCountValue(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().GetSelectedCardCount(PreviewCardUpgrade.CardInfo.CardID), true);
            PreviewCardUpgrade.transform.localScale = Vector3.one * 15;
            //PreviewCardUpgrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            if (PreviewCardUpgrade.CardInfo.BaseInfo.CardType == CardTypes.Retinue)
            {
                PreviewCardUpgrade.transform.localPosition = new Vector3(retinueCardPreviewDistance, 50, 0);
                UpgradeArrow.transform.position = UpgradeArrowPivot_retinueCards.position;
            }
            else
            {
                PreviewCardUpgrade.transform.localPosition = new Vector3(normalCardPreviewDistance, 50, 0);
                UpgradeArrow.transform.position = UpgradeArrowPivot_normal.position;
            }

            UpgradeArrow.enabled = true;

            PreviewCardUpgrade.ShowCardBloom(true);
            PreviewCardUpgrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FD5400"));
            PreviewCardUpgrade.BeBrightColor();
            PreviewCardUpgrade.M_BoxCollider.enabled = false;
            PreviewCardUpgrade.ShowAllSlotBlooms(true);
            PreviewCardUpgrade.RefreshCardTextLanguage();
        }
        else
        {
            UpgradeArrow.enabled = false;
        }

        if (hasDegradeCard)
        {
            PreviewCardDegrade = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID), PreviewContent, CardBase.CardShowMode.CardUpgradePreview);
            PreviewCardDegrade.CardOrder = 1;
            PreviewCardDegrade.ChangeCardSelectLimit(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().CurrentEditBuildButton.BuildInfo.M_BuildCards.CardSelectInfos[PreviewCard.CardInfo.CardID].CardSelectUpperLimit, true);
            PreviewCardDegrade.SetBlockCountValue(UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().GetSelectedCardCount(PreviewCardDegrade.CardInfo.CardID), true);
            PreviewCardDegrade.transform.localScale = Vector3.one * 15;
            //PreviewCardDegrade.transform.rotation = Quaternion.Euler(90, 180, 0);
            if (PreviewCardDegrade.CardInfo.BaseInfo.CardType == CardTypes.Retinue)
            {
                PreviewCardDegrade.transform.localPosition = new Vector3(-retinueCardPreviewDistance, 50, 0);
                DegradeArrow.transform.position = DegradeArrowPivot_retinueCards.position;
            }
            else
            {
                PreviewCardDegrade.transform.localPosition = new Vector3(-normalCardPreviewDistance, 50, 0);
                DegradeArrow.transform.position = DegradeArrowPivot_normal.position;
            }

            PreviewCardDegrade.ShowCardBloom(true);
            PreviewCardDegrade.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#0CE9FF"));
            PreviewCardDegrade.BeBrightColor();
            PreviewCardDegrade.M_BoxCollider.enabled = false;
            PreviewCardDegrade.ShowAllSlotBlooms(true);
            PreviewCardDegrade.RefreshCoinPosition();

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
        bool isShowAffixTips = UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(cardInfos, null);

        //如果显示Tips占据屏幕空间的话，右移预览卡牌窗口
        if (!isShowAffixTips || PreviewCardDegrade == null)
        {
            transform.position = PreviewCardPanelCenterPivot.position;
        }
        else
        {
            transform.position = PreviewCardPanelRightPivot.position;
        }
    }

    public void RefreshUpgradePanel()
    {
        bool hasUpgradeCard = false;
        bool hasDegradeCard = false;
        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
        {
            int u_id = PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID;
            int d_id = PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID;
            hasUpgradeCard = u_id != -1 && AllCards.GetCard(u_id).BaseInfo.CardRareLevel <= StoryManager.Instance.UnlockedCardLevelNum;
            hasDegradeCard = d_id != -1 && AllCards.GetCard(d_id).BaseInfo.CardRareLevel <= StoryManager.Instance.UnlockedCardLevelNum;
        }
        else if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
        {
            hasUpgradeCard = PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID != -1;
            hasDegradeCard = PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID != -1;
        }

        UpgradeCardButton.gameObject.SetActive(hasUpgradeCard);
        if (hasUpgradeCard)
        {
            int moreCoin = AllCards.GetCard(PreviewCard_Src.CardInfo.UpgradeInfo.UpgradeCardID).BaseInfo.Coin - PreviewCard_Src.CardInfo.BaseInfo.Coin;
            UpgradeCoinText.text = (-moreCoin).ToString();
        }

        DegradeCardButton.gameObject.SetActive(hasDegradeCard);
        if (hasDegradeCard)
        {
            int lessCoin = AllCards.GetCard(PreviewCard_Src.CardInfo.UpgradeInfo.DegradeCardID).BaseInfo.Coin - PreviewCard_Src.CardInfo.BaseInfo.Coin;
            if (lessCoin == 0)
            {
                DegradeCoinText.text = "0";
            }
            else if (lessCoin < 0)
            {
                DegradeCoinText.text = "+" + (-lessCoin);
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
        PreviewCard?.PoolRecycle();
        PreviewCardDegrade?.PoolRecycle();
        PreviewCardUpgrade?.PoolRecycle();
        PreviewCard = null;
        PreviewCardDegrade = null;
        PreviewCardUpgrade = null;
        PreviewCard_Src = null;
        UIManager.Instance.CloseUIForms<AffixPanel>();
    }

    internal void OnUpgradeButtonClick()
    {
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.UpdateCardUpgradeDegrade(true, PreviewCard_Src);
        RefreshUpgradePanel();
    }

    internal void OnDegradeButtonClick()
    {
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.UpdateCardUpgradeDegrade(false, PreviewCard_Src);
        RefreshUpgradePanel();
    }

    #endregion
}
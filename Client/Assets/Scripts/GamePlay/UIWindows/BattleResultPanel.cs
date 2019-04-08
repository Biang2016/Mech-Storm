using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

internal class BattleResultPanel : BaseUIForm
{
    private BattleResultPanel()
    {
    }

    #region Common

    [SerializeField] private Animator PanelAnimator;

    [SerializeField] private GameObject OnlineResultContent;
    [SerializeField] private Text OnlineWinText;
    [SerializeField] private Text OnlineLostText;

    #endregion

    #region Win

    [SerializeField] private GameObject WinContent;
    [SerializeField] private Text WinText;
    [SerializeField] private Text RewardsTitleText;
    [SerializeField] private Text SelectTipText;
    [SerializeField] private Transform BonusButtonContainer;
    public List<BonusButton> M_CurrentBonusButtons = new List<BonusButton>();
    private List<SmallBonusItem> M_CurrentFixedBonusItems = new List<SmallBonusItem>();
    [SerializeField] private Text NoRewardText;
    [SerializeField] private Text FixedBonusText;
    [SerializeField] private Transform FixedBonusContainer;
    [SerializeField] private Transform CardPreviewContainer;
    [SerializeField] private Transform CardRotationSample;
    [SerializeField] private Animator CardPreviewContainerAnim;
    [SerializeField] private Text ConfirmButtonText;
    private List<BonusGroup> AlwaysBonusGroup;
    private List<BonusGroup> OptionalBonusGroup;

    #endregion

    #region Lost

    [SerializeField] private GameObject LostContent;
    [SerializeField] private Text LostText;
    [SerializeField] private Text LostTipTitle;
    [SerializeField] private Text LostTipText;
    [SerializeField] private Text GoAheadButtonText;

    #endregion

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (WinText, "WinLostPanelManager_WinText"),
            (LostText, "WinLostPanelManager_LostText"),
            (OnlineWinText, "WinLostPanelManager_OnlineWinText"),
            (OnlineLostText, "WinLostPanelManager_OnlineLostText"),
            (LostTipTitle, "WinLostPanelManager_LostTipTitle"),
            (RewardsTitleText, "WinLostPanelManager_RewardsTitleText"),
            (NoRewardText, "WinLostPanelManager_NoRewardText"),
            (FixedBonusText, "WinLostPanelManager_FixedBonusText"),
            (ConfirmButtonText, "WinLostPanelManager_ConfirmButtonText"),
            (GoAheadButtonText, "WinLostPanelManager_GoAheadButtonText"),
            (CardUpgradeUnlockText, "WinLostPanelManager_CardUpgradeUnlockText"),
            (CardUpgradeUnlockDescText, "WinLostPanelManager_CardUpgradeUnlockDescText"),
        });
    }

    #region  Common

    public bool IsShow = false;

    public void Reset()
    {
        if (RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single)
        {
            foreach (BonusButton bb in M_CurrentBonusButtons)
            {
                bb.PoolRecycle();
            }

            M_CurrentBonusButtons.Clear();

            foreach (SmallBonusItem sbi in M_CurrentFixedBonusItems)
            {
                sbi.PoolRecycle();
            }

            M_CurrentFixedBonusItems.Clear();

            if (CurrentPreivewCard)
            {
                CurrentPreivewCard.PoolRecycle();
                CurrentPreivewCard = null;
            }

            OnConfirmButtonClickHandler = null;
            WinContent.SetActive(true);

            ResetCardUpgradeShowPanel();
        }

        CardUpgradeUnlockContent.SetActive(false);
        OnlineResultContent.SetActive(false);
    }

    IEnumerator Co_OnGameStopByWin(bool isWin)
    {
        IsShow = true;
        UIManager.Instance.CloseUIForms<SelectBuildPanel>();
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
        AudioManager.Instance.BGMStop();
        if (isWin)
        {
            PanelAnimator.SetTrigger("Show");
            AudioManager.Instance.SoundPlay("sfx/Victory");
        }
        else
        {
            AudioManager.Instance.SoundPlay("sfx/Lose");
        }

        PanelAnimator.SetTrigger("Show");
        GameManager.Instance.StartBlurBackGround();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EndWinLostPanel()
    {
        PanelAnimator.SetTrigger("Hide");
        GameManager.Instance.StopBlurBackGround();
        RoundManager.Instance.OnGameStop();
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        IsShow = false;
        Reset();
    }

    #endregion

    #region Win

    public void WinGame()
    {
        LostContent.SetActive(false);
        Reset();

        if (RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single)
        {
            StoryManager.Instance.ResetStoryBonusInfo();

            //AlwaysBonusGroup = StoryManager.Instance.GetCurrentBonusGroup(false, -1); //Always要执行，因为如果Always里面解锁了某些卡片，则要去掉避免重复
            //OptionalBonusGroup = StoryManager.Instance.GetCurrentBonusGroup(true, Random.Range(3, 4));

            foreach (BonusGroup bg in OptionalBonusGroup)
            {
                BonusButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BonusButton].AllocateGameObject<BonusButton>(BonusButtonContainer);
                bb.Initialize(bg);
                M_CurrentBonusButtons.Add(bb);
            }

            foreach (BonusGroup bg in AlwaysBonusGroup)
            {
                foreach (Bonus bonus in bg.Bonuses)
                {
                    SmallBonusItem sbi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SmallBonusItem].AllocateGameObject<SmallBonusItem>(FixedBonusContainer);
                    sbi.Initialize(bonus);
                    M_CurrentFixedBonusItems.Add(sbi);
                }
            }

            if (OptionalBonusGroup.Count == 1)
            {
                M_CurrentBonusButtons[0].SetSelected(true);
            }

            NoRewardText.enabled = AlwaysBonusGroup.Count == 0 && OptionalBonusGroup.Count == 0;
        }
        else
        {
            WinContent.SetActive(false);
            OnlineResultContent.SetActive(true);
            OnlineWinText.enabled = true;
            OnlineLostText.enabled = false;
        }

        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnGameStopByWin(true), "Co_OnGameStopByWin");
    }

    #endregion

    #region Lost

    public void LostGame()
    {
        Reset();
        WinContent.SetActive(false);
        if (RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single)
        {
            foreach (BonusButton bb in M_CurrentBonusButtons)
            {
                bb.PoolRecycle();
            }

            StoryManager.Instance.ResetStoryBonusInfo();

            M_CurrentBonusButtons.Clear();
            LostContent.SetActive(true);
            OnlineResultContent.SetActive(false);
            List<string> Tips = Utils.GetRandomFromList(LostTips[LanguageManager.Instance.IsEnglish], 2);

            foreach (string tip in Tips)
            {
                LostTipText.text = "";
                LostTipText.text += tip + "\n";
            }
        }
        else
        {
            LostContent.SetActive(false);
            OnlineResultContent.SetActive(true);
            OnlineWinText.enabled = false;
            OnlineLostText.enabled = true;
        }

        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnGameStopByWin(false), "Co_OnGameStopByWin");
    }

    public void OnGoAheadButtonClick()
    {
        EndWinLostPanel();
    }

    private static Dictionary<bool, List<string>> LostTips = new Dictionary<bool, List<string>>
    {
        {
            true, new List<string>
            {
                "Don't take too many useless cards, which would dilute your deck and decrease the chance to draw powerful cards.",
                "Lost? Nerver mind! Go to buy some powerful cards. And spare more budget on adding your life.",
                "Getting cards slowly? Try to increase the number of draw cards per round in the deck window.",
            }
        },
        {
            false, new List<string>
            {
                "没有用的卡牌不要拿太多哦~ 那样会稀释你的牌库，减少抽到好牌的概率",
                "太容易被击败? 尝试着花费更多预算来提高你的生命值吧! 就在选牌窗口哦~",
                "抽牌太少? 去选牌窗口里调整每回合抽牌数吧!",
                "不同的卡牌有不同的选牌上限~ 通常只能携带少量的强力卡牌",
            }
        }
    };

    #endregion

    public void SetBonusButtonSelected(BonusButton bonusButton)
    {
        foreach (BonusButton bb in M_CurrentBonusButtons)
        {
            bb.SetSelected(bb == bonusButton);
        }

        OnConfirmButtonClickHandler = bonusButton.OnConfirmButtonClickDelegate;
    }

    public delegate void OnConfirmButtonClickDelegate();

    public OnConfirmButtonClickDelegate OnConfirmButtonClickHandler;

    public void OnConfirmButtonClick()
    {
        if (OnConfirmButtonClickHandler != null) OnConfirmButtonClickHandler();
        if (OptionalBonusGroup.Count != 0 && OnConfirmButtonClickHandler == null)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("WinLostPanelManager_YouNeedToChooseACard"), 0, 0.8f);
        }
        else
        {
            SendAlwaysBonusRequest();
            ShowCardsUpgrade();
        }

        EndBattleRequest request = new EndBattleRequest(Client.Instance.Proxy.ClientId);
        Client.Instance.Proxy.SendMessage(request);
    }

    #region Bonus

    private void SendAlwaysBonusRequest()
    {
        foreach (BonusGroup bg in AlwaysBonusGroup)
        {
            GetBonusBuildChangeInfo(bg);
            BonusGroupRequest request = new BonusGroupRequest(Client.Instance.Proxy.ClientId, bg);
            Client.Instance.Proxy.SendMessage(request);
            foreach (Bonus bgBonus in bg.Bonuses)
            {
                if (bgBonus.M_BonusType == Bonus.BonusType.UnlockCardByID)
                {
                    StoryManager.Instance.JustGetNewCards.Add(bgBonus.BonusFinalValue);
                }
            }
        }
    }

    public void GetBonusBuildChangeInfo(BonusGroup bg)
    {
        if (!StoryManager.Instance.JustGetSomeCard)
        {
            if (bg.Bonuses[0].M_BonusType == Bonus.BonusType.UnlockCardByID) StoryManager.Instance.JustGetSomeCard = true;
        }

        foreach (Bonus b in bg.Bonuses)
        {
            if (b.M_BonusType == Bonus.BonusType.LifeUpperLimit && b.BonusFinalValue > 0) StoryManager.Instance.JustLifeAdd = true;
            if (b.M_BonusType == Bonus.BonusType.LifeUpperLimit && b.BonusFinalValue < 0) StoryManager.Instance.JustLifeLost = true;
            if (b.M_BonusType == Bonus.BonusType.EnergyUpperLimit && b.BonusFinalValue > 0) StoryManager.Instance.JustLifeAdd = true;
            if (b.M_BonusType == Bonus.BonusType.EnergyUpperLimit && b.BonusFinalValue < 0) StoryManager.Instance.JustEnergyLost = true;
            if (b.M_BonusType == Bonus.BonusType.Budget && b.BonusFinalValue > 0) StoryManager.Instance.JustBudgetAdd = true;
            if (b.M_BonusType == Bonus.BonusType.Budget && b.BonusFinalValue < 0) StoryManager.Instance.JustBudgetLost = true;
        }
    }

    private CardBase CurrentPreivewCard;

    public void ShowCardPreview(CardInfo_Base cb)
    {
        if (CurrentPreivewCard)
        {
            CurrentPreivewCard.PoolRecycle();
            CurrentPreivewCard = null;
        }

        CurrentPreivewCard = CardBase.InstantiateCardByCardInfo(cb, CardPreviewContainer, RoundManager.Instance.SelfClientPlayer, CardBase.CardShowMode.CardReward);
        CurrentPreivewCard.transform.localScale = CardRotationSample.localScale;
        CurrentPreivewCard.transform.rotation = CardRotationSample.rotation;
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CurrentPreivewCard.transform.parent.position = new Vector3(cameraPosition.x, CardRotationSample.position.y, CardRotationSample.position.z);
        CurrentPreivewCard.SetOrderInLayer(2);
        CurrentPreivewCard.BeBrightColor();
        CardPreviewContainerAnim.SetTrigger("Hover");
    }

    public void HideCardPreview()
    {
        if (CurrentPreivewCard)
        {
            CurrentPreivewCard.PoolRecycle();
            CurrentPreivewCard = null;
            CardPreviewContainerAnim.SetTrigger("Exit");
        }
    }

    #endregion

    #region CardUpgradeUnlock

    [SerializeField] private GameObject CardUpgradeUnlockContent;

    [SerializeField] private Animator CardUpgradeUnlockAnim;

    [SerializeField] private Text CardUpgradeUnlockText;
    [SerializeField] private Text CardUpgradeUnlockDescText;

    [SerializeField] private Transform CardUpgradeUnlockCardContainter;
    [SerializeField] private Transform CardUpgradeUnlockCardSample;

    [SerializeField] private Transform CardUpgradeUnlockCardContainter_Upgrade;
    [SerializeField] private Transform CardUpgradeUnlockCardSample_Upgrade;

    private CardBase Cur_BaseCard;
    private CardBase Cur_UpgradeCard;
    private bool isBeginCardUpgradeShow = false; //是否正在展示中
    private bool isOneCardUpgradeShowOver = false; //一个展示是否结束
    private bool isMouseClickSkip = false;

    private void ResetCardUpgradeShowPanel()
    {
        if (Cur_BaseCard != null) Cur_BaseCard.PoolRecycle();
        if (Cur_UpgradeCard != null) Cur_UpgradeCard.PoolRecycle();
        isBeginCardUpgradeShow = false;
        isOneCardUpgradeShowOver = false;
        isMouseClickSkip = false;
        CardUpgradeUnlockContent.SetActive(false);
        Cur_BaseCard = null;
        Cur_UpgradeCard = null;
    }

    private void ShowCardsUpgrade()
    {
        StartCoroutine(Co_ShowCardsUpgrade());
    }

    IEnumerator Co_ShowCardsUpgrade()
    {
        WinContent.SetActive(false);
        CardUpgradeUnlockContent.SetActive(true);
        isBeginCardUpgradeShow = true;
        if (StoryManager.Instance.IsThisLevelNumberUp)
        {
            foreach (int cardID in StoryManager.Instance.GetStory().Base_CardLimitDict.Keys.ToList())
            {
                if (StoryManager.Instance.GetStory().Base_CardLimitDict[cardID] != 0)
                {
                    CardInfo_Base cb = AllCards.GetCard(cardID);
                    if (cb.UpgradeInfo.UpgradeCardID != -1)
                    {
                        CardInfo_Base cb_upgrade = AllCards.GetCard(cb.UpgradeInfo.UpgradeCardID);
                        if (cb_upgrade.BaseInfo.CardRareLevel == StoryManager.Instance.UnlockedCardLevelNum)
                        {
                            StoryManager.Instance.JustUpgradeCards.Add(cb.CardID);
                            yield return Co_UnlockCardShowOne(cb.CardID, cb.UpgradeInfo.UpgradeCardID);
                        }
                    }
                }
            }
        }

        isBeginCardUpgradeShow = false;
        CardUpgradeUnlockAnim.SetTrigger("Reset");
        EndWinLostPanel();

        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().ShowNewCardBanner();
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().ShowUpgradeCardBanner();
    }

    IEnumerator Co_UnlockCardShowOne(int baseCardID, int upgradeCardID)
    {
        if (Cur_BaseCard != null) Cur_BaseCard.PoolRecycle();
        if (Cur_UpgradeCard != null) Cur_UpgradeCard.PoolRecycle();
        isOneCardUpgradeShowOver = false;
        Cur_BaseCard = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(baseCardID), CardUpgradeUnlockCardContainter, RoundManager.Instance.SelfClientPlayer, CardBase.CardShowMode.CardUpgradeAnim);
        Cur_BaseCard.transform.position = CardUpgradeUnlockCardSample.position;
        Cur_BaseCard.transform.rotation = CardUpgradeUnlockCardSample.rotation;
        Cur_BaseCard.transform.localScale = CardUpgradeUnlockCardSample.localScale;
        Cur_BaseCard.SetOrderInLayer(20);
        Cur_BaseCard.BeBrightColor();
        Cur_UpgradeCard = CardBase.InstantiateCardByCardInfo(AllCards.GetCard(upgradeCardID), CardUpgradeUnlockCardContainter_Upgrade, RoundManager.Instance.SelfClientPlayer, CardBase.CardShowMode.CardUpgradeAnim);
        Cur_UpgradeCard.transform.position = CardUpgradeUnlockCardSample_Upgrade.position;
        Cur_UpgradeCard.transform.rotation = CardUpgradeUnlockCardSample_Upgrade.rotation;
        Cur_UpgradeCard.transform.localScale = CardUpgradeUnlockCardSample_Upgrade.localScale;
        Cur_UpgradeCard.SetOrderInLayer(19);
        Cur_UpgradeCard.BeBrightColor();

        CardUpgradeUnlockAnim.speed = 3;
        CardUpgradeUnlockAnim.SetTrigger("Jump");
        AudioManager.Instance.SoundPlay("sfx/OnCardUpgradeShow");
        yield return new WaitForSeconds(1f);
        isOneCardUpgradeShowOver = true;

        while (true)
        {
            if (isMouseClickSkip)
            {
                isMouseClickSkip = false;
                break;
            }

            yield return null;
        }

        isOneCardUpgradeShowOver = true;
    }

    void Update()
    {
        if (isBeginCardUpgradeShow && Input.GetMouseButtonUp(0))
        {
            if (isOneCardUpgradeShowOver) isMouseClickSkip = true;
        }
    }

    #endregion
}
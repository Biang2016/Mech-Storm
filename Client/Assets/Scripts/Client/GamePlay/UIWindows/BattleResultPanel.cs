using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class BattleResultPanel : BaseUIForm
{
    private BattleResultPanel()
    {
    }

    #region Common

    internal bool IsShow = false;

    [SerializeField] private Animator PanelAnimator;

    [SerializeField] private GameObject OnlineResultContent;
    [SerializeField] private Text OnlineWinText;
    [SerializeField] private Text OnlineLostText;
    [SerializeField] private Button OnlineGoAheadButton;
    [SerializeField] private Text OnlineGoAheadButtonText;

    #endregion

    #region Win

    [SerializeField] private GameObject WinContent;
    [SerializeField] private Text WinText;
    [SerializeField] private Text SelectTipText;

    [SerializeField] private Text NoRewardText_OptionalBonus;
    [SerializeField] private Text NoRewardText_FixedBonus;

    [SerializeField] private GameObject OptionalBonus;
    [SerializeField] private GameObject OptionalBonusSeperator;
    [SerializeField] private Text OptionalBonusText;
    [SerializeField] private Transform OptionalBonusContainer;

    [SerializeField] private GameObject FixedBonus;
    [SerializeField] private GameObject FixedBonusSeperator;
    [SerializeField] private Text FixedBonusText;
    [SerializeField] private Transform FixedBonusContainer;

    [SerializeField] private Transform CardPreviewContainer;
    [SerializeField] private Transform CardRotationSample;
    [SerializeField] private Animator CardPreviewContainerAnim;

    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Text ConfirmButtonText;

    private List<BonusButton> M_CurrentBonusButtons = new List<BonusButton>();
    private List<SmallBonusItem> M_CurrentFixedBonusItems = new List<SmallBonusItem>();

    private List<BonusGroup> AllBonusGroups;
    private List<BonusGroup> AlwaysBonusGroups;
    private BonusGroup AlwaysBonusGroupsResults;
    private List<BonusGroup> OptionalBonusGroups;

    #endregion

    #region Lost

    [SerializeField] private GameObject LostContent;
    [SerializeField] private Text LostText;
    [SerializeField] private Text LostTipTitle;
    [SerializeField] private Text LostTipText;
    [SerializeField] private Button LostGoAheadButton;
    [SerializeField] private Text LostGoAheadButtonText;

    #endregion

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Fixed,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Blur);

        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (SelectTipText, "WinLostPanelManager_SelectTipText"),
            (WinText, "WinLostPanelManager_WinText"),
            (LostText, "WinLostPanelManager_LostText"),
            (OnlineWinText, "WinLostPanelManager_OnlineWinText"),
            (OnlineLostText, "WinLostPanelManager_OnlineLostText"),
            (LostTipTitle, "WinLostPanelManager_LostTipTitle"),
            (NoRewardText_OptionalBonus, "WinLostPanelManager_NoRewardText"),
            (NoRewardText_FixedBonus, "WinLostPanelManager_NoRewardText"),
            (OptionalBonusText, "WinLostPanelManager_OptionalBonusText"),
            (FixedBonusText, "WinLostPanelManager_FixedBonusText"),
            (ConfirmButtonText, "WinLostPanelManager_ConfirmButtonText"),
            (OnlineGoAheadButtonText, "WinLostPanelManager_GoAheadButtonText"),
            (LostGoAheadButtonText, "WinLostPanelManager_GoAheadButtonText"),
            (LostGoAheadButtonText, "WinLostPanelManager_GoAheadButtonText"),

            (CrystalRewardText, "WinLostPanelManager_CrystalRewardText"),
            (CrystalLevelLabel, "WinLostPanelManager_CrystalLevelLabel"),
            (CrystalHealthLabel, "WinLostPanelManager_CrystalHealthLabel"),
            (CrystalKillLabel, "WinLostPanelManager_CrystalKillEquation"),
            (CrystalDamageLabel, "WinLostPanelManager_CrystalDamageLabel"),
        });

        OnlineGoAheadButton.onClick.AddListener(OnGoAheadButtonClick);
        LostGoAheadButton.onClick.AddListener(OnGoAheadButtonClick);
        ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    #region  Common

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

            Cur_SelectedBonusButton = null;
            WinContent.SetActive(true);
        }

        OnlineResultContent.SetActive(false);
    }

    IEnumerator Co_OnGameStopByWin(bool isWin)
    {
        UIManager.Instance.CloseUIForm<SelectBuildPanel>();
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.None);
        AudioManager.Instance.BGMStop();
        if (isWin)
        {
            AudioManager.Instance.SoundPlay("sfx/Victory");
        }
        else
        {
            AudioManager.Instance.SoundPlay("sfx/Lose");
        }

        PanelAnimator.SetTrigger("Show");
        IsShow = true;
        RootManager.Instance.StartBlurBackGround();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EndWinLostPanel()
    {
        PanelAnimator.SetTrigger("Hide");
        IsShow = false;
        RootManager.Instance.StopBlurBackGround();
        RoundManager.Instance.OnGameStop();
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        Reset();
    }

    internal BattleStatistics BattleStatistics;

    #region Crystal

    [SerializeField] private GameObject CrystalRewardGO;
    [SerializeField] private GameObject CrystalRewardSeperator;
    [SerializeField] private Text CrystalRewardText;
    [SerializeField] private Transform CrystalRewardContainer;

    [SerializeField] private Text CrystalLevelLabel;
    [SerializeField] private Text CrystalHealthLabel;
    [SerializeField] private Text CrystalKillLabel;
    [SerializeField] private Text CrystalDamageLabel;

    [SerializeField] private Text CrystalLevelEquation;
    [SerializeField] private Text CrystalHealthEquation;
    [SerializeField] private Text CrystalKillEquation;
    [SerializeField] private Text CrystalDamageEquation;

    [SerializeField] private Text CrystalTotalText;

    public void SetCrystalInfo()
    {
        CrystalLevelEquation.text = string.Format("Lv.{0} x{1} = {2}", BattleStatistics.Level, BattleStatistics.LEVEL_CRYSTAL, BattleStatistics.levelCrystal);
        CrystalHealthEquation.text = string.Format("{0}% -> {1}", BattleStatistics.FinalHealthRatio * 100, BattleStatistics.healthCrystal);
        CrystalKillEquation.text = string.Format("{0} x{1} = {2}", BattleStatistics.TotalKill, BattleStatistics.KILL_CRYSTAL, BattleStatistics.killCrystal) + (BattleStatistics.KILL_CRYSTAL == BattleStatistics.killCrystal ? "(max)" : "");
        CrystalDamageEquation.text = string.Format("{0} x{1} = {2}", BattleStatistics.TotalDamage, BattleStatistics.DAMAGE_CRYSTAL, BattleStatistics.damageCrystal) + (BattleStatistics.DAMAGE_CRYSTAL == BattleStatistics.damageCrystal ? "(max)" : "");

        CrystalTotalText.text = "x" + BattleStatistics.totalCrystal.ToString();
    }

    [SerializeField] private RectTransform CrystalDetails;

    public void HideDetailsOfCrystal()
    {
        CrystalDetails.sizeDelta = new Vector2(CrystalDetails.sizeDelta.x, 120);
    }

    public void ShowDetailsOfCrystal()
    {
        CrystalDetails.sizeDelta = new Vector2(CrystalDetails.sizeDelta.x, 260);
    }

    #endregion

    #endregion

    #region Win

    private const int WinBonusCountMin = 3;
    private const int WinBonusCountMax = 5;

    public void WinGame()
    {
        LostContent.SetActive(false);
        Reset();

        if (RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single)
        {
            StoryManager.Instance.ResetStoryBonusInfo();

            AllBonusGroups = StoryManager.Instance.GetCurrentBonusGroup(); //Always要执行，因为如果Always里面解锁了某些卡片，则要去掉避免重复
            AlwaysBonusGroups = new List<BonusGroup>();
            OptionalBonusGroups = new List<BonusGroup>();
            foreach (BonusGroup bg in AllBonusGroups)
            {
                if (bg.IsAlways)
                {
                    AlwaysBonusGroups.Add(bg);
                }
                else
                {
                    OptionalBonusGroups.Add(bg);
                }
            }

            List<BonusGroup> RandomOptionalBonusGroup = Utils.GetRandomWithProbabilityFromList(OptionalBonusGroups, 3);

            List<BonusGroup> firstCheckBGs = new List<BonusGroup>();
            List<BonusGroup> secondCheckBGs = new List<BonusGroup>();
            foreach (BonusGroup bg in RandomOptionalBonusGroup)
            {
                bool findUnlockCardByID = false;
                foreach (Bonus b in bg.Bonuses)
                {
                    if (b is Bonus_UnlockCardByID b_id)
                    {
                        findUnlockCardByID = true;
                        break;
                    }
                }

                if (findUnlockCardByID)
                {
                    firstCheckBGs.Add(bg);
                }
                else
                {
                    secondCheckBGs.Add(bg);
                }
            }

            List<Bonus_BudgetLifeEnergyMixed.BudgetLifeEnergyComb> exceptionBudgetLifeEnergyComb = new List<Bonus_BudgetLifeEnergyMixed.BudgetLifeEnergyComb>();
            HashSet<int> exceptionCardIDs = new HashSet<int>();
            foreach (BonusGroup bg in firstCheckBGs)
            {
                BonusButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BonusButton].AllocateGameObject<BonusButton>(OptionalBonusContainer);
                bb.Initialize(bg, onClickAction: delegate { SetBonusButtonSelected(bb); }, exceptionCardIDs, exceptionBudgetLifeEnergyComb);
                M_CurrentBonusButtons.Add(bb);
            }

            foreach (BonusGroup bg in secondCheckBGs)
            {
                BonusButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BonusButton].AllocateGameObject<BonusButton>(OptionalBonusContainer);
                bb.Initialize(bg, onClickAction: delegate { SetBonusButtonSelected(bb); }, exceptionCardIDs, exceptionBudgetLifeEnergyComb);
                M_CurrentBonusButtons.Add(bb);
            }

            foreach (BonusGroup bg in AlwaysBonusGroups)
            {
                AlwaysBonusGroupsResults = new BonusGroup(true, new List<Bonus>(), 0, false);
                foreach (Bonus bonus in bg.Bonuses)
                {
                    if (bonus is Bonus_BudgetLifeEnergyMixed b_Mixed)
                    {
                        foreach (Bonus b in b_Mixed.GetBonusListFromBonusComb(b_Mixed.GetBudgetLifeEnergyComb(new List<Bonus_BudgetLifeEnergyMixed.BudgetLifeEnergyComb>())))
                        {
                            AlwaysBonusGroupsResults.Bonuses.Add(b.Clone());
                            SmallBonusItem _sbi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SmallBonusItem].AllocateGameObject<SmallBonusItem>(FixedBonusContainer);
                            _sbi.Initialize(b);
                            M_CurrentFixedBonusItems.Add(_sbi);
                        }
                    }
                    else
                    {
                        SmallBonusItem sbi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.SmallBonusItem].AllocateGameObject<SmallBonusItem>(FixedBonusContainer);
                        sbi.Initialize(bonus);
                        M_CurrentFixedBonusItems.Add(sbi);
                        AlwaysBonusGroupsResults.Bonuses.Add(bonus.Clone());
                    }
                }
            }

            ConfirmButton.gameObject.SetActive(false);

            if (OptionalBonusGroups.Count == 1)
            {
                M_CurrentBonusButtons[0].SetSelected(true);
                ConfirmButton.gameObject.SetActive(true);
                Cur_SelectedBonusButton = M_CurrentBonusButtons[0];
            }
            else if (OptionalBonusGroups.Count == 0)
            {
                ConfirmButton.gameObject.SetActive(true);
                Cur_SelectedBonusButton = null;
            }

            OptionalBonus.SetActive(true);
            OptionalBonusSeperator.SetActive(true);

            FixedBonus.SetActive(true);
            FixedBonusSeperator.SetActive(true);

            CrystalRewardGO.SetActive(true);
            CrystalRewardSeperator.SetActive(true);

            NoRewardText_OptionalBonus.enabled = OptionalBonusGroups.Count == 0;
            NoRewardText_FixedBonus.enabled = AlwaysBonusGroups.Count == 0;
        }
        else
        {
            WinContent.SetActive(false);
            OnlineResultContent.SetActive(true);
            OnlineWinText.enabled = true;
            OnlineLostText.enabled = false;
        }

        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnGameStopByWin(true), "Co_OnGameStopByWin");
        GameObjectPoolManager.Instance.RefreshAllRecycledPoolObjectPosition();
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
        GameObjectPoolManager.Instance.RefreshAllRecycledPoolObjectPosition();
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
                "Prepare different decks for different enemies. You can create new deck in the deck window",
                "Don't take too many useless cards, which would dilute your deck and decrease the chance to draw powerful cards.",
                "Lost? Never mind! Go to buy some powerful cards. And spare more budget on adding your life.",
                "Drawing cards slowly? Try to increase the number of draw cards per round in the deck window.",
                "Enemy too strong? Try other enemies first to get more cards!",
                "Increase the number of draw cards per round, decrease the number of cards in deck, and then speed up the circulation of cards to have more combo!",
                "Cards stuck in hand? Get a broken energy cycle? Never mind! Take more Energy cards and less Spell cards.",
            }
        },
        {
            false, new List<string>
            {
                "准备多套卡组以应付不同类型的敌人; 在选牌窗口可以新建卡组",
                "没有用的卡牌不要拿太多哦~ 那样会稀释你的牌库，减少抽到好牌的概率",
                "太容易被击败? 尝试着花费更多预算来提高你的生命值吧! 就在选牌窗口哦~",
                "抽牌太少? 去选牌窗口里调整每回合抽牌数吧!",
                "不同的卡牌有不同的选牌上限~ 通常只能携带少量的强力卡牌",
                "关卡太难? 可以先尝试其他关卡，待收集到更强力的卡片之后再来挑战哦~",
                "增加每回合抽牌数，减少卡牌数量，能够有效加速卡组循环，更好地打出卡牌配合！",
                "卡手了? 能量循环跟不上? 多带一些能量牌，少带一些法术牌吧!",
                "敌方机甲挡住了你胜利的步伐? 尝试使用直伤法术直取敌舰吧!",
                "后期节奏跟不上时，说明卡组循环速度不足，可以提高抽牌数，或携带更多具有过牌效果的卡片。",
            }
        }
    };

    #endregion

    private BonusButton Cur_SelectedBonusButton = null;

    public void SetBonusButtonSelected(BonusButton bonusButton)
    {
        foreach (BonusButton bb in M_CurrentBonusButtons)
        {
            bb.SetSelected(bb == bonusButton);
        }

        Cur_SelectedBonusButton = bonusButton;
        ConfirmButton.gameObject.SetActive(Cur_SelectedBonusButton != null);
    }

    private void OnConfirmButtonClick()
    {
        SendOptionalBonusRequest();
        SendAlwaysBonusRequest();

        List<BonusGroup> getBonusGroups = new List<BonusGroup>();
        if (Cur_SelectedBonusButton)
        {
            getBonusGroups.Add(Cur_SelectedBonusButton.BonusGroup);
        }

        getBonusGroups.Add(AlwaysBonusGroupsResults);

        ApplyBonusChange(getBonusGroups);
        EndWinLostPanel();

        EndBattleRequest request = new EndBattleRequest(Client.Instance.Proxy.ClientID, StoryManager.Instance.GetStory().CurrentFightingChapterID, StoryManager.Instance.CurrentFightingEnemy.LevelID);
        Client.Instance.Proxy.SendMessage(request);
    }

    #region Bonus

    private void SendOptionalBonusRequest()
    {
        if (Cur_SelectedBonusButton != null)
        {
            BonusGroupRequest request1 = new BonusGroupRequest(Client.Instance.Proxy.ClientID, Cur_SelectedBonusButton.BonusGroup);
            Client.Instance.Proxy.SendMessage(request1);
        }
    }

    private void SendAlwaysBonusRequest()
    {
        BonusGroupRequest request = new BonusGroupRequest(Client.Instance.Proxy.ClientID, AlwaysBonusGroupsResults);
        Client.Instance.Proxy.SendMessage(request);
    }

    private void ApplyBonusChange(List<BonusGroup> getBonusGroups)
    {
        foreach (BonusGroup bg in getBonusGroups)
        {
            foreach (Bonus b in bg.Bonuses)
            {
                switch (b)
                {
                    case Bonus_LifeUpperLimit bl:
                    {
                        StoryManager.Instance.JustLifeChange += bl.LifeUpperLimit;
                        break;
                    }

                    case Bonus_EnergyUpperLimit be:
                    {
                        StoryManager.Instance.JustEnergyChange += be.EnergyUpperLimit;
                        break;
                    }
                    case Bonus_Budget bb:
                    {
                        StoryManager.Instance.JustBudgetChange += bb.Budget;
                        break;
                    }
                    case Bonus_UnlockCardByID bu:
                    {
                        StoryManager.Instance.JustGetNewCards.Add(bu.CardID);
                        break;
                    }
                }
            }
        }

        UIManager.Instance.GetBaseUIForm<StoryPanel>().UnSelectNode();
        UIManager.Instance.GetBaseUIForm<StoryPanel>().Cur_ChapterMap.UnSelectAllNode();
    }

    private CardBase CurrentPreivewCard;

    public void ShowCardPreview(CardInfo_Base cb)
    {
        if (CurrentPreivewCard)
        {
            CurrentPreivewCard.PoolRecycle();
            CurrentPreivewCard = null;
        }

        CurrentPreivewCard = CardBase.InstantiateCardByCardInfo(cb, CardPreviewContainer, CardBase.CardShowMode.CardReward, RoundManager.Instance.SelfClientPlayer);
        CurrentPreivewCard.transform.localScale = CardRotationSample.localScale;
        CurrentPreivewCard.transform.rotation = CardRotationSample.rotation;
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CurrentPreivewCard.transform.parent.position = new Vector3(cameraPosition.x, CardRotationSample.position.y, CardRotationSample.position.z);
        CurrentPreivewCard.CardOrder = 2;
        CurrentPreivewCard.BeBrightColor();
        CurrentPreivewCard.ShowCardBloom(true);
        CardPreviewContainerAnim.SetTrigger("Hover");
        UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {cb}, null);
    }

    public void HideCardPreview()
    {
        if (CurrentPreivewCard)
        {
            CurrentPreivewCard.PoolRecycle();
            CurrentPreivewCard = null;
            CardPreviewContainerAnim.SetTrigger("Exit");
            UIManager.Instance.CloseUIForm<AffixPanel>();
        }
    }

    #endregion
}
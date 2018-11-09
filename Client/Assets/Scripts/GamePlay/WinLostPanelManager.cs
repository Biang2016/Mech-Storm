using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

internal class WinLostPanelManager : MonoSingleton<WinLostPanelManager>
{
    private WinLostPanelManager()
    {
    }

    void Awake()
    {
        WinLostCanvas.enabled = false;
        WinText.text = GameManager.Instance.IsEnglish ? "You win!" : "你赢了！";
        LostText.text = GameManager.Instance.IsEnglish ? "You lost ..." : "你输了...";
        OnlineWinText.text = GameManager.Instance.IsEnglish ? "You lost ..." : "你输了...";
        OnlineLostText.text = GameManager.Instance.IsEnglish ? "You lost ..." : "你输了...";
        LostTipTitle.text = GameManager.Instance.IsEnglish ? "Tips: " : "提示: ";
        RewardsTitleText.text = GameManager.Instance.IsEnglish ? "Rewards" : "奖励";
        NoRewardText.text = GameManager.Instance.IsEnglish ? "The designer is so mean that here isn't any reward." : "设计师是个吝啬鬼，这里没有任何奖励";
        FixedBonusText.text = GameManager.Instance.IsEnglish ? "Fixed Bonus" : "固定\n奖励";
    }

    [SerializeField] private Canvas WinLostCanvas;
    [SerializeField] private Animator PanelAnimator;

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

    [SerializeField] private GameObject LostContent;
    [SerializeField] private Text LostText;
    [SerializeField] private Text LostTipTitle;
    [SerializeField] private Text LostTipText;

    [SerializeField] private GameObject OnlineResultContent;
    [SerializeField] private Text OnlineWinText;
    [SerializeField] private Text OnlineLostText;

    private List<BonusGroup> AlwaysBonusGroup;
    private List<BonusGroup> OptionalBonusGroup;
    public bool IsShow = false;


    public void Reset()
    {
        if (RoundManager.Instance.isSingleBattle)
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
            OnlineResultContent.SetActive(false);
        }
    }

    public void WinGame()
    {
        LostContent.SetActive(false);

        if (RoundManager.Instance.isSingleBattle)
        {
            Reset();
            SelectBuildManager.Instance.ResetStoryBonusInfo();

            AlwaysBonusGroup = StoryManager.Instance.GetCurrentBonusGroup(false); //Always要执行，因为如果Always里面解锁了某些卡片，则要去掉避免重复
            OptionalBonusGroup = Utils.GetRandomFromList(StoryManager.Instance.GetCurrentBonusGroup(true), Random.Range(2, 4));
            foreach (BonusGroup bg in OptionalBonusGroup)
            {
                BonusButton bb = GameObjectPoolManager.Instance.Pool_BonusButtonPool.AllocateGameObject<BonusButton>(BonusButtonContainer);
                bb.Initialize(bg);
                M_CurrentBonusButtons.Add(bb);
            }

            foreach (BonusGroup bg in AlwaysBonusGroup)
            {
                foreach (Bonus bonus in bg.Bonuses)
                {
                    SmallBonusItem sbi = GameObjectPoolManager.Instance.Pool_SmallBonusItemPool.AllocateGameObject<SmallBonusItem>(FixedBonusContainer);
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

    public void LostGame()
    {
        WinContent.SetActive(false);
        if (RoundManager.Instance.isSingleBattle)
        {
            foreach (BonusButton bb in M_CurrentBonusButtons)
            {
                bb.PoolRecycle();
            }

            M_CurrentBonusButtons.Clear();
            LostContent.SetActive(true);
            OnlineResultContent.SetActive(false);
            List<string> Tips = Utils.GetRandomFromList(LostTips[GameManager.Instance.IsEnglish], 2);

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

    IEnumerator Co_OnGameStopByWin(bool isWin)
    {
        IsShow = true;
        if (SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.Show_ReadOnly) SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
        WinLostCanvas.enabled = true;
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
        WinLostCanvas.enabled = false;
        IsShow = false;
        Reset();
    }

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
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Come on, you should select one reward!" : "你需要选择一个奖励,不要客气!", 0, 0.8f);
        }
        else
        {
            SendAlwaysBonusRequest();
            EndWinLostPanel();
        }
    }

    private void SendAlwaysBonusRequest()
    {
        foreach (BonusGroup bg in AlwaysBonusGroup)
        {
            GetBonusBuildChangeInfo(bg);
            BonusGroupRequest request = new BonusGroupRequest(Client.Instance.Proxy.ClientId, bg);
            Client.Instance.Proxy.SendMessage(request);
        }
    }

    public void GetBonusBuildChangeInfo(BonusGroup bg)
    {
        if (!SelectBuildManager.Instance.JustGetSomeCard)
        {
            if (bg.Bonuses[0].M_BonusType == Bonus.BonusType.UnlockCardByID) SelectBuildManager.Instance.JustGetSomeCard = true;
        }

        foreach (Bonus b in bg.Bonuses)
        {
            if (b.M_BonusType == Bonus.BonusType.LifeUpperLimit && b.Value > 0) SelectBuildManager.Instance.JustLifeAdd = true;
            if (b.M_BonusType == Bonus.BonusType.LifeUpperLimit && b.Value < 0) SelectBuildManager.Instance.JustLifeLost = true;
            if (b.M_BonusType == Bonus.BonusType.EnergyUpperLimit && b.Value > 0) SelectBuildManager.Instance.JustLifeAdd = true;
            if (b.M_BonusType == Bonus.BonusType.EnergyUpperLimit && b.Value < 0) SelectBuildManager.Instance.JustEnergyLost = true;
            if (b.M_BonusType == Bonus.BonusType.Budget && b.Value > 0) SelectBuildManager.Instance.JustBudgetAdd = true;
            if (b.M_BonusType == Bonus.BonusType.Budget && b.Value < 0) SelectBuildManager.Instance.JustBudgetLost = true;
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

        CurrentPreivewCard = CardBase.InstantiateCardByCardInfo(cb, CardPreviewContainer, RoundManager.Instance.SelfClientPlayer, false);
        CurrentPreivewCard.transform.localScale = CardRotationSample.localScale;
        CurrentPreivewCard.transform.rotation = CardRotationSample.rotation;
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CurrentPreivewCard.transform.parent.position = new Vector3(cameraPosition.x, CardRotationSample.position.y, CardRotationSample.position.z);
        CurrentPreivewCard.SetOrderInLayer(2);
        CurrentPreivewCard.BeBrightColor();
        CurrentPreivewCard.SetBonusCardBloom(true);
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

    private static Dictionary<bool, List<string>> LostTips = new Dictionary<bool, List<string>>
    {
        {
            true, new List<string>
            {
                "Don't take too many useless cards, which would dilute your deck and decrease the change to draw powerful cards.",
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
}
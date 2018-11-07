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
    }

    [SerializeField] private Canvas WinLostCanvas;
    [SerializeField] private Animator PanelAnimator;

    [SerializeField] private GameObject WinContent;
    [SerializeField] private Text WinText;
    [SerializeField] private Text RewardsTitleText;
    [SerializeField] private Text SelectTipText;
    [SerializeField] private Transform BonusButtonContainer;
    private List<BonusButton> M_CurrentBonusButtons = new List<BonusButton>();
    [SerializeField] private Text NoRewardText;

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

    public void WinGame()
    {
        LostContent.SetActive(false);

        if (RoundManager.Instance.isSingleBattle)
        {
            foreach (BonusButton bb in M_CurrentBonusButtons)
            {
                bb.PoolRecycle();
            }

            OnConfirmButtonClickHandler = null;
            M_CurrentBonusButtons.Clear();
            WinContent.SetActive(true);
            OnlineResultContent.SetActive(false);
            AlwaysBonusGroup = StoryManager.Instance.GetCurrentAlwaysBonusGroup();
            OptionalBonusGroup = Utils.GetRandomFromList(StoryManager.Instance.GetCurrentOptionalBonusGroup(), Random.Range(2, 4));
            foreach (BonusGroup bonusGroup in OptionalBonusGroup)
            {
                BonusButton bb = GameObjectPoolManager.Instance.Pool_BonusButtonPool.AllocateGameObject<BonusButton>(BonusButtonContainer);
                bb.Initialize(bonusGroup);
                M_CurrentBonusButtons.Add(bb);
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
            EndWinLostPanel();
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
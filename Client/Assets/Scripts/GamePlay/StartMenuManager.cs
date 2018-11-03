using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StartMenuManager : MonoSingleton<StartMenuManager>
{
    private StartMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        Awake_TextFont();
        Awake_DeckAbstract();
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    void Update()
    {
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                M_StateMachine.SetState(StateMachine.States.Hide);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Hide);
                break;
            case ProxyBase.ClientStates.Login:
                GameBoardManager.Instance.ChangeBoardBG();
                M_StateMachine.SetState(StateMachine.States.Show_Main);
                break;
            case ProxyBase.ClientStates.Matching:
                if (M_StateMachine.GetState() == StateMachine.States.Show_Online) M_StateMachine.SetState(StateMachine.States.Show_Online_Matching);
                if (M_StateMachine.GetState() == StateMachine.States.Show_Single) M_StateMachine.SetState(StateMachine.States.Show_Single_Preparing);
                break;
            case ProxyBase.ClientStates.Playing:
                M_StateMachine.SetState(StateMachine.States.Hide);
                break;
        }
    }

    public StateMachine M_StateMachine;

    public class StateMachine
    {
        public StateMachine()
        {
            state = States.Default;
            previousState = States.Default;
        }

        public enum States
        {
            Default,
            Hide,
            Show_Main,
            Show_Online,
            Show_Online_Matching,
            Show_Single,
            Show_Single_Preparing,
        }

        public bool IsShow()
        {
            return state == States.Show_Main || state == States.Show_Online || state == States.Show_Online_Matching || state == States.Show_Single || state == States.Show_Single_Preparing;
        }

        private States state;
        private States previousState;

        public void SetState(States newState)
        {
            if (state != newState)
            {
                switch (newState)
                {
                    case States.Hide:
                        HideMenu();
                        break;

                    case States.Show_Main:
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(true);
                        Instance.SingleMenuButton.gameObject.SetActive(true);
                        Instance.SettingButton.gameObject.SetActive(true);
                        Instance.AboutButton.gameObject.SetActive(true);
                        Instance.BackButton.gameObject.SetActive(false);
                        Instance.QuitGameButton.gameObject.SetActive(true);
                        Instance.OnlineStartButton.gameObject.SetActive(false);
                        Instance.CancelMatchButton.gameObject.SetActive(false);
                        Instance.OnlineDeckButton.gameObject.SetActive(false);
                        Instance.SingleStartButton.gameObject.SetActive(false);
                        Instance.SingleResumeButton.gameObject.SetActive(false);
                        Instance.SingleDeckButton.gameObject.SetActive(false);
                        break;
                    case States.Show_Online:
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SettingButton.gameObject.SetActive(true);
                        Instance.AboutButton.gameObject.SetActive(false);
                        Instance.BackButton.gameObject.SetActive(true);
                        Instance.QuitGameButton.gameObject.SetActive(false);
                        Instance.OnlineStartButton.gameObject.SetActive(true);
                        Instance.CancelMatchButton.gameObject.SetActive(false);
                        Instance.OnlineDeckButton.gameObject.SetActive(true);
                        Instance.SingleStartButton.gameObject.SetActive(false);
                        Instance.SingleResumeButton.gameObject.SetActive(false);
                        Instance.SingleDeckButton.gameObject.SetActive(false);
                        break;
                    case States.Show_Online_Matching:
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SettingButton.gameObject.SetActive(true);
                        Instance.AboutButton.gameObject.SetActive(false);
                        Instance.BackButton.gameObject.SetActive(true);
                        Instance.QuitGameButton.gameObject.SetActive(false);
                        Instance.OnlineStartButton.gameObject.SetActive(false);
                        Instance.CancelMatchButton.gameObject.SetActive(true);
                        Instance.OnlineDeckButton.gameObject.SetActive(true);
                        Instance.SingleStartButton.gameObject.SetActive(false);
                        Instance.SingleResumeButton.gameObject.SetActive(false);
                        Instance.SingleDeckButton.gameObject.SetActive(false);
                        break;
                    case States.Show_Single:
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SettingButton.gameObject.SetActive(true);
                        Instance.AboutButton.gameObject.SetActive(false);
                        Instance.BackButton.gameObject.SetActive(true);
                        Instance.QuitGameButton.gameObject.SetActive(false);
                        Instance.OnlineStartButton.gameObject.SetActive(false);
                        Instance.CancelMatchButton.gameObject.SetActive(false);
                        Instance.OnlineDeckButton.gameObject.SetActive(false);
                        Instance.SingleStartButton.gameObject.SetActive(true);
                        Instance.SingleResumeButton.gameObject.SetActive(StoryManager.Instance.M_CurrentStory != null);
                        Instance.SingleDeckButton.gameObject.SetActive(StoryManager.Instance.M_CurrentStory != null);
                        break;
                    case States.Show_Single_Preparing:
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SettingButton.gameObject.SetActive(false);
                        Instance.AboutButton.gameObject.SetActive(false);
                        Instance.BackButton.gameObject.SetActive(false);
                        Instance.QuitGameButton.gameObject.SetActive(false);
                        Instance.OnlineStartButton.gameObject.SetActive(false);
                        Instance.CancelMatchButton.gameObject.SetActive(false);
                        Instance.OnlineDeckButton.gameObject.SetActive(false);
                        Instance.SingleStartButton.gameObject.SetActive(false);
                        Instance.SingleResumeButton.gameObject.SetActive(false);
                        Instance.SingleDeckButton.gameObject.SetActive(false);
                        break;
                }

                previousState = state;
                state = newState;
            }
        }

        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        public States GetState()
        {
            return state;
        }

        public void Update()
        {
        }

        private void ShowMenu()
        {
            Instance.StartMenuCanvas.enabled = true;
            Instance.RefreshBuildInfoAbstract();

            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenuBGM0", "bgm/StartMenuBGM1"});
        }


        private void HideMenu()
        {
            Instance.StartMenuCanvas.enabled = false;
        }

        public void RefreshStoryState()
        {
            Instance.SingleResumeButton.gameObject.SetActive(StoryManager.Instance.M_CurrentStory != null);
            Instance.SingleDeckButton.gameObject.SetActive(StoryManager.Instance.M_CurrentStory != null);
        }
    }

    void Awake_TextFont()
    {
        OnlineMenuText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        SingleMenuText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        SettingText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        AboutText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DesignerText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        BackText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        QuitGameText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        OnlineMenuText.text = GameManager.Instance.IsEnglish ? "Online Mode" : "在线对战";
        SingleMenuText.text = GameManager.Instance.IsEnglish ? "Single Mode" : "单人模式";
        SettingText.text = GameManager.Instance.IsEnglish ? "Settings" : "游戏设置";
        AboutText.text = GameManager.Instance.IsEnglish ? "About Me" : "制作人员";
        DesignerText.text = GameManager.Instance.IsEnglish ? "Designer\nXue Bingsheng" : "制作人: 薛炳晟";
        BackText.text = GameManager.Instance.IsEnglish ? "Back" : "返回";
        QuitGameText.text = GameManager.Instance.IsEnglish ? "Quit Game" : "退出游戏";

        OnlineStartText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        CancelMatchText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        OnlineDeckText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        OnlineStartText.text = GameManager.Instance.IsEnglish ? "Start Matching" : "开始匹配";
        CancelMatchText.text = GameManager.Instance.IsEnglish ? "Cancel Matching" : "取消匹配";
        OnlineDeckText.text = GameManager.Instance.IsEnglish ? "My Decks" : "我的卡组";

        SingleStartText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        SingleResumeText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        SingleDeckText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        SingleStartText.text = GameManager.Instance.IsEnglish ? "New Story" : "新的游戏";
        SingleResumeText.text = GameManager.Instance.IsEnglish ? "Resume" : "继续游戏";
        SingleDeckText.text = GameManager.Instance.IsEnglish ? "My Decks" : "我的卡组";
    }

    [SerializeField] private Canvas StartMenuCanvas;

    [SerializeField] private Button OnlineMenuButton;
    [SerializeField] private Text OnlineMenuText;

    [SerializeField] private Button SingleMenuButton;
    [SerializeField] private Text SingleMenuText;

    [SerializeField] private Button SettingButton;
    [SerializeField] private Text SettingText;

    [SerializeField] private Button AboutButton;
    [SerializeField] private Text AboutText;

    [SerializeField] private Button BackButton;
    [SerializeField] private Text BackText;

    [SerializeField] private Button QuitGameButton;
    [SerializeField] private Text QuitGameText;

    #region OnlineMenu

    [SerializeField] private Button OnlineStartButton;
    [SerializeField] private Text OnlineStartText;

    [SerializeField] private Button CancelMatchButton;
    [SerializeField] private Text CancelMatchText;

    [SerializeField] private Button OnlineDeckButton;
    [SerializeField] private Text OnlineDeckText;

    #endregion

    #region SingleMenu

    [SerializeField] private Button SingleStartButton;
    [SerializeField] private Text SingleStartText;

    [SerializeField] private Button SingleResumeButton;
    [SerializeField] private Text SingleResumeText;

    [SerializeField] private Button SingleDeckButton;
    [SerializeField] private Text SingleDeckText;

    #endregion

    #region DeckAbstract

    void Awake_DeckAbstract()
    {
        if (GameManager.Instance.IsEnglish)
        {
            DeckAbstractText_DeckName.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Cards.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_CardNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Life.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_LifeNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Energy.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_EnergyNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Draws.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_DrawNum.fontStyle = FontStyle.BoldAndItalic;
        }
        else
        {
            DeckAbstractText_DeckName.fontStyle = FontStyle.Italic;
            DeckAbstractText_Cards.fontStyle = FontStyle.Italic;
            DeckAbstractText_CardNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Life.fontStyle = FontStyle.Italic;
            DeckAbstractText_LifeNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Energy.fontStyle = FontStyle.Italic;
            DeckAbstractText_EnergyNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Draws.fontStyle = FontStyle.Italic;
            DeckAbstractText_DrawNum.fontStyle = FontStyle.Italic;
        }

        DeckAbstractText_DeckName.text = GameManager.Instance.IsEnglish ? "No Deck" : "无卡组";
        DeckAbstractText_Cards.text = GameManager.Instance.IsEnglish ? "Cards" : "卡牌数:";
        DeckAbstractText_CardNum.text = "";
        DeckAbstractText_Life.text = GameManager.Instance.IsEnglish ? "Life" : "生命值:";
        DeckAbstractText_LifeNum.text = "";
        DeckAbstractText_Energy.text = GameManager.Instance.IsEnglish ? "Energy" : "能量上限:";
        DeckAbstractText_EnergyNum.text = "";
        DeckAbstractText_Draws.text = GameManager.Instance.IsEnglish ? "Draws" : "抽牌数:";
        DeckAbstractText_DrawNum.text = "";

        DeckAbstractText_DeckName.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Cards.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_CardNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Life.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_LifeNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Energy.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_EnergyNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Draws.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_DrawNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
    }

    [SerializeField] private GameObject DeckAbstract;

    [SerializeField] private Text DesignerText;

    [SerializeField] private Text DeckAbstractText_DeckName;
    [SerializeField] private Text DeckAbstractText_Cards;
    [SerializeField] private Text DeckAbstractText_CardNum;
    [SerializeField] private Text DeckAbstractText_Life;
    [SerializeField] private Text DeckAbstractText_LifeNum;
    [SerializeField] private Text DeckAbstractText_Energy;
    [SerializeField] private Text DeckAbstractText_EnergyNum;
    [SerializeField] private Text DeckAbstractText_Draws;
    [SerializeField] private Text DeckAbstractText_DrawNum;

    #endregion

    public void RefreshBuildInfoAbstract()
    {
        if (SelectBuildManager.Instance.CurrentSelectedBuildButton != null)
        {
            DeckAbstractText_DeckName.text = "[ " + SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildName + " ]";
            DeckAbstractText_CardNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.CardCount.ToString();
            DeckAbstractText_LifeNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.Life.ToString();
            DeckAbstractText_EnergyNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.Energy.ToString();
            DeckAbstractText_DrawNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.DrawCardNum.ToString();
        }
        else
        {
            DeckAbstractText_DeckName.text = "";
            DeckAbstractText_CardNum.text = "";
            DeckAbstractText_LifeNum.text = "";
            DeckAbstractText_EnergyNum.text = "";
            DeckAbstractText_DrawNum.text = "";
        }
    }

    public void OnOnlineMenuButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Show_Online);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Online);
        GameBoardManager.Instance.ChangeBoardBG();
    }

    public void OnSingleMenuButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Show_Single);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Single);
        GameBoardManager.Instance.ChangeBoardBG();
    }

    public void OnBackButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Show_Main);
        GameBoardManager.Instance.ChangeBoardBG();
    }

    public void OnOnlineStartButtonClick()
    {
        StartGameCore(false, -1, -1);
    }

    public void OnOnlineDeckButtonClick()
    {
        OnSelectCardDeckWindowButtonClick();
    }

    public void OnSingleDeckButtonClick()
    {
        OnSelectCardDeckWindowButtonClick();
    }

    public void OnSingleStartButtonClick()
    {
        ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
        string enDesc = "Do you want to start a new Single Game?" + (StoryManager.Instance.M_CurrentStory != null ? " It will remove your current game." : "");
        string zhDesc = "是否开始一个新的游戏？" + (StoryManager.Instance.M_CurrentStory != null ? " 当前游戏将被清除。" : "");

        UnityAction action = StartNewStory;
        cw.Initialize(
            GameManager.Instance.IsEnglish ? enDesc : zhDesc,
            GameManager.Instance.IsEnglish ? "Yes" : "是的",
            GameManager.Instance.IsEnglish ? "No" : "取消",
            action + ConfirmWindowManager.Instance.RemoveConfirmWindow,
            ConfirmWindowManager.Instance.RemoveConfirmWindow
        );
    }

    private void StartNewStory()
    {
        StartNewStoryRequest request = new StartNewStoryRequest(Client.Instance.Proxy.ClientId);
        Client.Instance.Proxy.SendMessage(request);
    }

    public void OnSingleResumeButtonClick()
    {
        StoryManager.Instance.M_StateMachine.SetState(StoryManager.StateMachine.States.Show);
        //StartGameCore(true);
    }

    public void StartGameCore(bool isStandAlone, int levelID, int bossID)
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Login)
        {
            if (SelectBuildManager.Instance.CurrentSelectedBuildButton == null) //未发送卡组则跳出选择卡组界面
            {
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.CardCount == 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Your deck is empty" : "您的卡组中没有卡牌", 0, 0.3f);
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (!SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.IsEnergyEnough)
            {
                UnityAction action;
                if (isStandAlone)
                {
                    action = delegate { StartNewSingleGame(levelID, bossID); };
                }
                else
                {
                    action = StartOnlineMatch;
                }

                ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
                if (GameManager.Instance.IsEnglish)
                {
                    cw.Initialize("Some cards consume more energy than your upper limit.", "Go ahead", "Edit",
                        action + ConfirmWindowManager.Instance.RemoveConfirmWindow,
                        new UnityAction(OnSelectCardDeckWindowButtonClick) + ConfirmWindowManager.Instance.RemoveConfirmWindow);
                }
                else
                {
                    cw.Initialize("您的卡组中有些牌的能量消耗大于您的能量上限，是否继续？", "继续", "编辑",
                        action + ConfirmWindowManager.Instance.RemoveConfirmWindow,
                        new UnityAction(OnSelectCardDeckWindowButtonClick) + ConfirmWindowManager.Instance.RemoveConfirmWindow);
                }

                return;
            }
            else
            {
                if (isStandAlone)
                {
                    StartNewSingleGame(levelID, bossID);
                }
                else
                {
                    StartOnlineMatch();
                }
            }
        }
    }

    private void StartOnlineMatch()
    {
        Client.Instance.Proxy.OnBeginMatch();
        ClientLog.Instance.Print(GameManager.Instance.IsEnglish ? "Begin matching" : "开始匹配");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Matching" : "匹配中", 0, float.PositiveInfinity);
        DeckAbstract.SetActive(true);
    }

    private void StartNewSingleGame(int levelID, int bossID)
    {
        Client.Instance.Proxy.OnBeginSingleMode(levelID, bossID);
        ClientLog.Instance.Print(GameManager.Instance.IsEnglish ? "Begin single mode" : "开始单人模式");
        AudioManager.Instance.SoundPlay("sfx/BattleStart" + Random.Range(0, 3));
    }

    public void OnCancelMatchGameButtonClick()
    {
        Client.Instance.Proxy.CancelMatch();
        ClientLog.Instance.Print(GameManager.Instance.IsEnglish ? "Matching canceled" : "取消匹配");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Match canceled" : "取消匹配", 0, 1f);
        DeckAbstract.SetActive(false);
    }

    public void OnSelectCardDeckWindowButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        if (Client.Instance.IsLogin() && !Client.Instance.IsMatching()) SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Show);
        else if (Client.Instance.IsPlaying() || Client.Instance.IsMatching()) SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Show_ReadOnly);
    }

    public void OnSettingButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        SettingMenuManager.Instance.M_StateMachine.SetState(SettingMenuManager.StateMachine.States.ShowFromStartMenu);
    }

    public void OnQuitGameButtonClick()
    {
        ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
        cw.Initialize(
            GameManager.Instance.IsEnglish ? "Are you sure to Quick the game?" : "退出游戏?",
            GameManager.Instance.IsEnglish ? "Yes" : "是",
            GameManager.Instance.IsEnglish ? "No" : "取消",
            Application.Quit,
            ConfirmWindowManager.Instance.RemoveConfirmWindow
        );

        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            Client.Instance.Proxy.CancelMatch();
        }

        //LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientId, Client.Instance.Proxy.Username);
        //Client.Instance.Proxy.SendMessage(request);
    }
}
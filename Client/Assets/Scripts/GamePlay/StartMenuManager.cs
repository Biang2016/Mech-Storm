using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartMenuManager : MonoSingleton<StartMenuManager>
{
    private StartMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        Awake_Text();
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
                if (M_StateMachine.GetState() == StateMachine.States.Show_SingleCustom) M_StateMachine.SetState(StateMachine.States.Show_SingleCustom_Preparing);
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
            Show_SingleCustom,
            Show_SingleCustom_Preparing,
        }

        public bool IsShow()
        {
            return state != States.Default && state != States.Hide;
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
                    {
                        HideMenu();
                        break;
                    }
                    case States.Show_Main:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenu_0", "bgm/StartMenu_1"});
                        Instance.OnlineMenuButton.gameObject.SetActive(true);
                        Instance.SingleMenuButton.gameObject.SetActive(true);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(true);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);
                        break;
                    }
                    case States.Show_Online:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenu_0", "bgm/StartMenu_1"});
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);
                        break;
                    }
                    case States.Show_Online_Matching:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);
                        break;
                    }
                    case States.Show_Single:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenu_0", "bgm/StartMenu_1"});
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);
                        Instance.CheckSingleModeNewAndUpgradeCards();
                    }
                        break;
                    case States.Show_Single_Preparing:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);

                        bool newCard = SelectBuildManager.Instance.JustGetNewCards.Count != 0 || SelectBuildManager.Instance.JustUpgradeCards.Count != 0;
                        Instance.SingleDeckNewImage.gameObject.SetActive(newCard);
                        Instance.SingleDeckNewText.gameObject.SetActive(newCard);
                        break;
                    }
                    case States.Show_SingleCustom:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenu_0", "bgm/StartMenu_1"});
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
                        Instance.SettingButton.gameObject.SetActive(true);
                        Instance.AboutButton.gameObject.SetActive(false);
                        Instance.BackButton.gameObject.SetActive(true);
                        Instance.QuitGameButton.gameObject.SetActive(false);
                        Instance.OnlineStartButton.gameObject.SetActive(false);
                        Instance.CancelMatchButton.gameObject.SetActive(false);
                        Instance.OnlineDeckButton.gameObject.SetActive(false);
                        Instance.SingleStartButton.gameObject.SetActive(false);
                        Instance.SingleResumeButton.gameObject.SetActive(false);
                        Instance.SingleDeckButton.gameObject.SetActive(false);
                        Instance.SingleCustomStartButton.gameObject.SetActive(true);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(true);
                        break;
                    }
                    case States.Show_SingleCustom_Preparing:
                    {
                        if (!Client.Instance.IsLogin()) return;
                        ShowMenu();
                        Instance.OnlineMenuButton.gameObject.SetActive(false);
                        Instance.SingleMenuButton.gameObject.SetActive(false);
                        Instance.SingleCustomBattleButton.gameObject.SetActive(false);
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
                        Instance.SingleCustomStartButton.gameObject.SetActive(false);
                        Instance.SingleCustomDeckButton.gameObject.SetActive(false);
                    }
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

    void Awake_Text()
    {
        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (OnlineMenuText, "StartMenu_OnlineMode"),
                (SingleMenuText, "StartMenu_SingleMode"),
                (SingleCustomBattleText, "StartMenu_CustomizeBattle"),
                (SettingText, "StartMenu_Settings"),
                (AboutText, "StartMenu_AboutMe"),
                (DesignerText, "StartMenu_Designer"),
                (BackText, "StartMenu_Back"),
                (DesignerText, "StartMenu_Designer"),
                (QuitGameText, "StartMenu_QuitGame"),

                (OnlineStartText, "StartMenu_OnlineStart"),
                (CancelMatchText, "StartMenu_CancelMatch"),
                (OnlineDeckText, "StartMenu_OnlineDeck"),

                (SingleStartText, "StartMenu_SingleStart"),
                (SingleResumeText, "StartMenu_SingleResume"),
                (SingleDeckText, "StartMenu_SingleDeck"),

                (SingleCustomStartText, "StartMenu_SingleCustomStart"),
                (SingleCustomDeckText, "StartMenu_SingleCustomDeck"),

                (OnlineMenuTipText, "StartMenu_OnlineMenuTipText"),
                (SingleMenuTipText, "StartMenu_SingleMenuTipText"),
                (SingleCustomBattleTipText, "StartMenu_SingleCustomBattleTipText"),
                (SettingTipText, "StartMenu_SettingTipText"),
                (BackTipText, "StartMenu_BackTipText"),
                (OnlineDeckTipText, "StartMenu_DeckTipText"),
                (SingleResumeTipText, "StartMenu_SingleResumeTipText"),
                (SingleDeckTipText, "StartMenu_DeckTipText"),
                (SingleCustomDeckTipText, "StartMenu_DeckTipText"),
            });
    }

    [SerializeField] private Canvas StartMenuCanvas;

    [SerializeField] private Button OnlineMenuButton;
    [SerializeField] private Text OnlineMenuText;
    [SerializeField] private Text OnlineMenuTipText;

    [SerializeField] private Button SingleMenuButton;
    [SerializeField] private Text SingleMenuText;
    [SerializeField] private Text SingleMenuTipText;

    [SerializeField] private Button SingleCustomBattleButton;
    [SerializeField] private Text SingleCustomBattleText;
    [SerializeField] private Text SingleCustomBattleTipText;

    [SerializeField] private Button SettingButton;
    [SerializeField] private Text SettingText;
    [SerializeField] private Text SettingTipText;

    [SerializeField] private Button AboutButton;
    [SerializeField] private Text AboutText;

    [SerializeField] private Button BackButton;
    [SerializeField] private Text BackText;
    [SerializeField] private Text BackTipText;

    [SerializeField] private Button QuitGameButton;
    [SerializeField] private Text QuitGameText;

    #region OnlineMenu

    [SerializeField] private Button OnlineStartButton;
    [SerializeField] private Text OnlineStartText;

    [SerializeField] private Button CancelMatchButton;
    [SerializeField] private Text CancelMatchText;

    [SerializeField] private Button OnlineDeckButton;
    [SerializeField] private Text OnlineDeckText;
    [SerializeField] private Text OnlineDeckTipText;

    #endregion

    #region SingleMenu

    [SerializeField] private Button SingleStartButton;
    [SerializeField] private Text SingleStartText;

    [SerializeField] private Button SingleResumeButton;
    [SerializeField] private Text SingleResumeText;
    [SerializeField] private Text SingleResumeTipText;

    [SerializeField] private Button SingleDeckButton;
    [SerializeField] private Text SingleDeckText;
    [SerializeField] private Text SingleDeckTipText;
    [SerializeField] private Image SingleDeckNewImage;
    [SerializeField] private Text SingleDeckNewText;

    #endregion

    #region SingleCustomMenu

    [SerializeField] private Button SingleCustomStartButton;
    [SerializeField] private Text SingleCustomStartText;

    [SerializeField] private Button SingleCustomDeckButton;
    [SerializeField] private Text SingleCustomDeckText;
    [SerializeField] private Text SingleCustomDeckTipText;

    #endregion

    #region DeckAbstract

    void Awake_DeckAbstract()
    {
        if (LanguageManager.Instance.IsEnglish)
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

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (DeckAbstractText_DeckName, "StartMenu_DeckAbstractText_DeckName"),
                (DeckAbstractText_Cards, "StartMenu_DeckAbstractText_Cards"),
                (DeckAbstractText_CardNum, "StartMenu_DeckAbstractText_CardNum"),
                (DeckAbstractText_Life, "StartMenu_DeckAbstractText_Life"),
                (DeckAbstractText_LifeNum, "StartMenu_DeckAbstractText_LifeNum"),
                (DeckAbstractText_Energy, "StartMenu_DeckAbstractText_Energy"),
                (DeckAbstractText_EnergyNum, "StartMenu_DeckAbstractText_EnergyNum"),
                (DeckAbstractText_Draws, "StartMenu_DeckAbstractText_Draws"),
                (DeckAbstractText_DrawNum, "StartMenu_DeckAbstractText_DrawNum"),
            });
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

    public void CheckSingleModeNewAndUpgradeCards()
    {
        bool newCard = SelectBuildManager.Instance.JustGetNewCards.Count != 0 || SelectBuildManager.Instance.JustUpgradeCards.Count != 0;
        SingleDeckNewImage.gameObject.SetActive(newCard);
        SingleDeckNewText.gameObject.SetActive(newCard);
    }

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

    public void OnSingleCustomMenuButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Show_SingleCustom);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Online); //自定义模式与online模式共用一套卡组
        GameBoardManager.Instance.ChangeBoardBG();
    }

    public void OnBackButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Show_Main);
        if (Client.Instance.IsMatching()) OnCancelMatchGameButtonClick();
        GameBoardManager.Instance.ChangeBoardBG();
    }

    public void OnOnlineStartButtonClick()
    {
        StartGameCore(RoundManager.PlayMode.Online, -1, -1);
    }

    public void OnOnlineDeckButtonClick()
    {
        OnSelectCardDeckWindowButtonClick();
    }

    public void OnSingleDeckButtonClick()
    {
        OnSelectCardDeckWindowButtonClick();
    }

    public void OnSingleCustomDeckButtonClick()
    {
        OnSelectCardDeckWindowButtonClick();
    }

    public void OnSingleStartButtonClick()
    {
        if (StoryManager.Instance.M_CurrentStory != null)
        {
            ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
            string desc = LanguageManager.Instance.GetText("Notice_StartMenu_DoYouWantToStartANewSingleGame") + (StoryManager.Instance.M_CurrentStory != null ? LanguageManager.Instance.GetText("Notice_StartMenu_CurrentGameWillBeRemoved") : "");

            UnityAction action = StartNewStory;
            cw.Initialize(
                desc,
                LanguageManager.Instance.GetText("Common_Yes"),
                LanguageManager.Instance.GetText("Common_Cancel"),
                action + ConfirmWindowManager.Instance.RemoveConfirmWindow,
                ConfirmWindowManager.Instance.RemoveConfirmWindow
            );
        }
        else
        {
            StartNewStory();
        }
    }

    public void OnSingleCustonStartButtonClick()
    {
        StartGameCore(RoundManager.PlayMode.SingleCustom, -1, -1);
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

    public void StartGameCore(RoundManager.PlayMode playMode, int levelID, int bossPicID)
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Login)
        {
            UnityAction startGameAction = null;
            switch (playMode)
            {
                case RoundManager.PlayMode.Online:
                {
                    startGameAction = StartOnlineMatch;
                    break;
                }
                case RoundManager.PlayMode.Single:
                {
                    startGameAction = delegate { StartNewSingleGame(levelID, bossPicID); };
                    break;
                }
                case RoundManager.PlayMode.SingleCustom:
                {
                    startGameAction = StartSingleCustomGame;
                    break;
                }
            }

            if (SelectBuildManager.Instance.CurrentSelectedBuildButton == null) //未发送卡组则跳出选择卡组界面
            {
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.CardCount == 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_StartMenu_NoCardInDeck"), 0, 0.3f);
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (!SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.IsEnergyEnough())
            {
                ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
                cw.Initialize(LanguageManager.Instance.GetText("Notice_StartMenu_CardEnergyOverYourEnergy"),
                    LanguageManager.Instance.GetText("Common_GoAhead"),
                    LanguageManager.Instance.GetText("Common_Edit"),
                    startGameAction + ConfirmWindowManager.Instance.RemoveConfirmWindow,
                    new UnityAction(OnSelectCardDeckWindowButtonClick) + ConfirmWindowManager.Instance.RemoveConfirmWindow + delegate { StoryManager.Instance.M_StateMachine.SetState(StoryManager.StateMachine.States.Hide); });
                return;
            }
            else
            {
                if (startGameAction != null) startGameAction();
            }
        }
    }

    private void StartOnlineMatch()
    {
        Client.Instance.Proxy.OnBeginMatch();
        ClientLog.Instance.Print(LanguageManager.Instance.GetText("StartMenu_BeginMatching"));
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("StartMenu_Matching"), 0, float.PositiveInfinity);
        DeckAbstract.SetActive(true);
        RoundManager.Instance.M_PlayMode = RoundManager.PlayMode.Online;
    }

    private void StartNewSingleGame(int levelID, int bossPicID)
    {
        Client.Instance.Proxy.OnBeginSingleMode(levelID, bossPicID);
        ClientLog.Instance.Print(LanguageManager.Instance.GetText("StartMenu_BeginSingleMode"));
        RoundManager.Instance.M_PlayMode = RoundManager.PlayMode.Single;
        StoryManager.Instance.Fighting_LevelID = levelID;
        StoryManager.Instance.Fighting_BossPicID = bossPicID;
    }

    private void StartSingleCustomGame()
    {
        Client.Instance.Proxy.OnBeginSingleMode(-1, -1);
        ClientLog.Instance.Print(LanguageManager.Instance.GetText("StartMenu_BeginCustomMode"));
        RoundManager.Instance.M_PlayMode = RoundManager.PlayMode.SingleCustom;
    }

    public void OnCancelMatchGameButtonClick()
    {
        Client.Instance.Proxy.CancelMatch();
        ClientLog.Instance.Print(LanguageManager.Instance.GetText("StartMenu_CancelMatch"));
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("StartMenu_CancelMatch"), 0, 1f);
        DeckAbstract.SetActive(false);
    }

    public void OnSelectCardDeckWindowButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        StoryManager.Instance.M_StateMachine.SetState(StoryManager.StateMachine.States.Hide);
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
            LanguageManager.Instance.GetText("Notice_StartMenu_QuitGameConfirm"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_Cancel"),
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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartMenuPanel : BaseUIForm
{
    private StartMenuPanel()
    {
    }

    [SerializeField] private Button SwitchToOtherModeButton;
    [SerializeField] private Text SwitchToOtherModeText;

    [SerializeField] private Button CardEditorButton;
    [SerializeField] private Text CardEditorText;

    [SerializeField] private Button StoryEditorButton;
    [SerializeField] private Text StoryEditorText;

    [SerializeField] private Transform ButtonContainer;
    private Dictionary<string, StartMenuButton> StartMenuButtonDict = new Dictionary<string, StartMenuButton>();
    private Dictionary<States, List<string>> StateMenuButtonListDict = new Dictionary<States, List<string>>();

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: true,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.ImPenetrable);

        CardEditorButton.onClick.AddListener(GameManager.Instance.OnCardEditorButtonClick);
        StoryEditorButton.onClick.AddListener(GameManager.Instance.OnStoryEditorButtonClick);

        LanguageManager.Instance.RegisterTextKey(SwitchToOtherModeText, "StartMenu_SwitchToOnlineModeText");
        LanguageManager.Instance.RegisterTextKey(CardEditorText, "LoginMenu_CardEditorButtonText");
        LanguageManager.Instance.RegisterTextKey(StoryEditorText, "LoginMenu_StoryEditorButtonText");

        Proxy.OnClientStateChange += OnClientChangeState;

        AddButton("OnlineMenu", "StartMenu_OnlineMode", "StartMenu_OnlineMenuTipText", OnOnlineMenuButtonClick);
        AddButton("SingleMenu", "StartMenu_SingleMode", "StartMenu_SingleMenuTipText", OnSingleMenuButtonClick);
        AddButton("SingleCustomBattle", "StartMenu_CustomizeBattle", "StartMenu_SingleCustomBattleTipText", OnSingleCustomMenuButtonClick);
        AddButton("Setting", "StartMenu_Settings", "StartMenu_SettingTipText", OnSettingButtonClick);
        AddButton("Back", "StartMenu_Back", "StartMenu_BackTipText", OnBackButtonClick);
        AddButton("QuitGame", "StartMenu_QuitGame", null, OnQuitGameButtonClick);

        AddButton("OnlineStart", "StartMenu_OnlineStart", null, OnOnlineStartButtonClick);
        AddButton("CancelMatch", "StartMenu_CancelMatch", null, OnCancelMatchGameButtonClick);
        AddButton("OnlineDeck", "StartMenu_OnlineDeck", "StartMenu_DeckTipText", OnOnlineDeckButtonClick, StartMenuButton.TipImageType.NewCard);

        AddButton("SingleStart", "StartMenu_SingleStart", null, OnSingleStartButtonClick);
        AddButton("SingleResume", "StartMenu_SingleResume", "StartMenu_SingleResumeTipText", OnSingleResumeButtonClick);
        AddButton("SingleDeck", "StartMenu_SingleDeck", "StartMenu_DeckTipText", OnSingleDeckButtonClick, StartMenuButton.TipImageType.NewCard);

        AddButton("SingleCustomStart", "StartMenu_SingleCustomStart", null, OnSingleCustomStartButtonClick);
        AddButton("SingleCustomDeck", "StartMenu_SingleCustomDeck", "StartMenu_DeckTipText", OnSingleCustomDeckButtonClick, StartMenuButton.TipImageType.NewCard);

        StateMenuButtonListDict.Add(States.Show_Main_Standalone, new List<string> {"SingleMenu", "SingleCustomBattle", "Setting", "QuitGame"});
        StateMenuButtonListDict.Add(States.Show_Main_Online, new List<string> {"OnlineMenu", "SingleMenu", "SingleCustomBattle", "Setting", "QuitGame"});
        StateMenuButtonListDict.Add(States.Show_Online, new List<string> {"OnlineStart", "OnlineDeck", "Back"});
        StateMenuButtonListDict.Add(States.Show_Online_Matching, new List<string> {"CancelMatch", "OnlineDeck", "Back"});
        StateMenuButtonListDict.Add(States.Show_Single, new List<string> {"SingleStart", "Back"});
        StateMenuButtonListDict.Add(States.Show_Single_HasStory, new List<string> {"SingleStart", "SingleResume", "SingleDeck", "Back"});
        StateMenuButtonListDict.Add(States.Show_SingleCustom, new List<string> {"SingleCustomStart", "SingleCustomDeck", "Back"});

        Awake_DeckAbstract();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (UIManager.Instance.GetPeekUIForm() == null)
            {
                if ((state & States.Show_SecondaryMenu) == state)
                {
                    OnBackButtonClick();
                }
                else if ((state & States.Show_FirstMenu) == state)
                {
                    UIManager.Instance.ShowUIForms<ExitMenuPanel>();
                }
            }
        }

        if ((state & States.Show_SecondaryMenu) == state)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                UIManager.Instance.ShowUIForms<SelectBuildPanel>();
            }
        }
    }

    public override void Display()
    {
        base.Display();
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
    }

    public override void Hide()
    {
        base.Hide();
    }

    private void AddButton(string goName, string textKey, string tipTextKey, UnityAction buttonClick, StartMenuButton.TipImageType tipImageType = StartMenuButton.TipImageType.None)
    {
        StartMenuButton smb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StartMenuButton].AllocateGameObject<StartMenuButton>(ButtonContainer);
        smb.name = goName + "Button";
        smb.BindTextKey(textKey, tipTextKey, buttonClick, tipImageType);
        StartMenuButtonDict.Add(goName, smb);
    }

    public void SetButtonTipImageShow(string goName, bool isShow)
    {
        StartMenuButtonDict.TryGetValue(goName, out StartMenuButton smb);
        if (smb != null)
        {
            smb.SetTipImageTextShow(isShow);
        }
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                UIManager.Instance.CloseUIForm<StartMenuPanel>();
                break;
            case ProxyBase.ClientStates.GetId:
                UIManager.Instance.CloseUIForm<StartMenuPanel>();
                break;
            case ProxyBase.ClientStates.Login:
                UIManager.Instance.ShowUIForms<StartMenuPanel>();
                BackGroundManager.Instance.ChangeBoardBG();
                if (Client.Instance.IsStandalone)
                {
                    SetState(States.Show_Main_Standalone);
                }
                else
                {
                    SetState(States.Show_Main_Online);
                }

                break;
            case ProxyBase.ClientStates.Matching:
                if (state == States.Show_Online) SetState(States.Show_Online_Matching);
                break;
            case ProxyBase.ClientStates.Playing:
                UIManager.Instance.CloseUIForm<StartMenuPanel>();
                break;
        }
    }

    private States state;

    [Flags]
    public enum States
    {
        Show_Main_Standalone = 1,
        Show_Main_Online = 2,
        Show_Online = 4,
        Show_Online_Matching = 8,
        Show_Single = 16,
        Show_Single_HasStory = 32,
        Show_SingleCustom = 64,
        Show_FirstMenu = Show_Main_Standalone | Show_Main_Online,
        Show_SecondaryMenu = Show_Online | Show_Single | Show_Single_HasStory | Show_SingleCustom,
    }

    public void SetState(States newState)
    {
        List<string> showButtons = StateMenuButtonListDict[newState];
        foreach (KeyValuePair<string, StartMenuButton> kv in StartMenuButtonDict)
        {
            bool isShow = showButtons.Contains(kv.Key);
            kv.Value.gameObject.SetActive(isShow);
        }

        foreach (string btnName in showButtons)
        {
            StartMenuButtonDict[btnName].transform.SetAsLastSibling();
        }

        RefreshBuildInfoAbstract(SelectBuildManager.Instance.CurrentSelectedBuildInfo);

        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenu_0", "bgm/StartMenu_1"});

        state = newState;

        if (newState == States.Show_Main_Standalone)
        {
            LanguageManager.Instance.UnregisterText(SwitchToOtherModeText);
            LanguageManager.Instance.RegisterTextKey(SwitchToOtherModeText, "StartMenu_SwitchToOnlineModeText");
            SwitchToOtherModeButton.onClick.RemoveAllListeners();
            SwitchToOtherModeButton.onClick.AddListener(OnSwitchToOnlineMode);
        }
        else if (newState == States.Show_Main_Online)
        {
            LanguageManager.Instance.UnregisterText(SwitchToOtherModeText);
            LanguageManager.Instance.RegisterTextKey(SwitchToOtherModeText, "StartMenu_SwitchToSingleModeText");
            SwitchToOtherModeButton.onClick.RemoveAllListeners();
            SwitchToOtherModeButton.onClick.AddListener(OnSwitchToSingleMode);
        }
    }

    #region DeckAbstract

    [SerializeField] private GameObject DeckAbstract;

    [SerializeField] private Text DeckAbstractText_DeckName;
    [SerializeField] private Text DeckAbstractText_Cards;
    [SerializeField] private Text DeckAbstractText_CardNum;
    [SerializeField] private Text DeckAbstractText_Life;
    [SerializeField] private Text DeckAbstractText_LifeNum;
    [SerializeField] private Text DeckAbstractText_Energy;
    [SerializeField] private Text DeckAbstractText_EnergyNum;
    [SerializeField] private Text DeckAbstractText_Draws;
    [SerializeField] private Text DeckAbstractText_DrawNum;

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

    #endregion

    public void RefreshBuildInfoAbstract(BuildInfo buildInfo)
    {
        if (buildInfo != null)
        {
            DeckAbstractText_DeckName.text = "[ " + buildInfo.BuildName + " ]";
            DeckAbstractText_CardNum.text = buildInfo.CardCount.ToString();
            DeckAbstractText_LifeNum.text = buildInfo.Life.ToString();
            DeckAbstractText_EnergyNum.text = buildInfo.Energy.ToString();
            DeckAbstractText_DrawNum.text = buildInfo.DrawCardNum.ToString();
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
        SetState(States.Show_Online);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Online);
        BackGroundManager.Instance.ChangeBoardBG();
    }

    public void OnSingleMenuButtonClick()
    {
        SetState(StoryManager.Instance.HasStory ? States.Show_Single_HasStory : States.Show_Single);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Single);
        BackGroundManager.Instance.ChangeBoardBG();
    }

    public void OnSingleCustomMenuButtonClick()
    {
        SetState(States.Show_SingleCustom);
        SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Online); //自定义模式与online模式共用一套卡组
        BackGroundManager.Instance.ChangeBoardBG();
    }

    public void OnBackButtonClick()
    {
        if (Client.Instance.IsStandalone)
        {
            SetState(States.Show_Main_Standalone);
        }
        else
        {
            SetState(States.Show_Main_Online);
        }

        if (Client.Instance.IsMatching()) OnCancelMatchGameButtonClick();
        BackGroundManager.Instance.ChangeBoardBG();
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
        if (StoryManager.Instance.HasStory)
        {
            ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
            string desc = LanguageManager.Instance.GetText("Notice_StartMenu_DoYouWantToStartANewSingleGame") + (StoryManager.Instance.HasStory ? LanguageManager.Instance.GetText("Notice_StartMenu_CurrentGameWillBeRemoved") : "");

            UnityAction action = StartNewStory;
            cp.Initialize(
                desc,
                LanguageManager.Instance.GetText("Common_Yes"),
                LanguageManager.Instance.GetText("Common_Cancel"),
                action + cp.CloseUIForm,
                cp.CloseUIForm
            );
        }
        else
        {
            StartNewStory();
        }
    }

    public void OnSingleCustomStartButtonClick()
    {
        StartGameCore(RoundManager.PlayMode.SingleCustom, -1, -1);
    }

    private void StartNewStory()
    {
        StartNewStoryRequest request = new StartNewStoryRequest(Client.Instance.Proxy.ClientID);
        Client.Instance.Proxy.SendMessage(request);
    }

    public void OnSingleResumeButtonClick()
    {
    }

    public void StartGameCore(RoundManager.PlayMode playMode, int chapterID, int levelID)
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
                    startGameAction = delegate { StartSingleGame(chapterID, levelID); };
                    break;
                }
                case RoundManager.PlayMode.SingleCustom:
                {
                    startGameAction = StartSingleCustomGame;
                    break;
                }
            }

            if (SelectBuildManager.Instance.CurrentSelectedBuildInfo == null) //未发送卡组则跳出选择卡组界面
            {
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (SelectBuildManager.Instance.CurrentSelectedBuildInfo.CardCount == 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_StartMenu_NoCardInDeck"), 0, 0.3f);
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (!SelectBuildManager.Instance.CurrentSelectedBuildInfo.IsEnergyEnough())
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(LanguageManager.Instance.GetText("Notice_StartMenu_CardEnergyOverYourEnergy"),
                    LanguageManager.Instance.GetText("Common_GoAhead"),
                    LanguageManager.Instance.GetText("Common_Edit"),
                    startGameAction + cp.CloseUIForm,
                    new UnityAction(OnSelectCardDeckWindowButtonClick) + cp.CloseUIForm + delegate
                    {
                        //StoryManager.Instance.SetState(StoryManager.States.Hide);
                    });
                return;
            }
            else
            {
                startGameAction?.Invoke();
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

    private void StartSingleGame(int chapterID, int levelID)
    {
        Client.Instance.Proxy.OnBeginSingleMode(chapterID, levelID);
        ClientLog.Instance.Print(LanguageManager.Instance.GetText("StartMenu_BeginSingleMode"));
        RoundManager.Instance.M_PlayMode = RoundManager.PlayMode.Single;
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
        UIManager.Instance.ShowUIForms<SelectBuildPanel>().Init();
    }

    public void OnSettingButtonClick()
    {
        UIManager.Instance.ShowUIForms<SettingPanel>();
    }

    public void OnQuitGameButtonClick()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            LanguageManager.Instance.GetText("Notice_StartMenu_QuitGameConfirm"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_Cancel"),
            delegate
            {
                LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientID, Client.Instance.Proxy.Username);
                Client.Instance.Proxy.SendMessage(request);
                cp.CloseUIForm();
            },
            cp.CloseUIForm
        );
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            Client.Instance.Proxy.CancelMatch();
        }
    }

    private void OnSwitchToOnlineMode()
    {
        LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientID, Client.Instance.Proxy.Username);
        Client.Instance.Proxy.SendMessage(request);
        UIManager.Instance.GetBaseUIForm<LoginPanel>().ReturnToOnlineMode();
    }

    private void OnSwitchToSingleMode()
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            Client.Instance.Proxy.CancelMatch();
        }

        LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientID, Client.Instance.Proxy.Username);
        Client.Instance.Proxy.SendMessage(request);
        UIManager.Instance.GetBaseUIForm<LoginPanel>().ReturnToSingleMode();
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitMenuManager : MonoSingleton<ExitMenuManager>
{
    private ExitMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (SettingMenuText, "ExitMenu_SettingMenuText"),
                (SurrenderText, "ExitMenu_SurrenderText"),
                (ConsumeText, "ExitMenu_ConsumeText"),
                (QuitText, "ExitMenu_QuitText"),
                (SettingMenuText, "ExitMenu_SettingMenuText"),
                (SurrenderText, "ExitMenu_SurrenderText"),
                (ConsumeText, "ExitMenu_ConsumeText"),
                (QuitText, "ExitMenu_QuitText"),
            });
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        SurrenderButton.gameObject.SetActive(false);
        ConsumeButton.gameObject.SetActive(true);
        QuitGameButton.gameObject.SetActive(true);
        Proxy.OnClientStateChange += OnClientChangeState;
    }

    void Update()
    {
        M_StateMachine.Update();
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.GetId:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.Login:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Matching:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Playing:
                SurrenderButton.gameObject.SetActive(true);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
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
            HideForSetting,
            Hide,
            Show,
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
                    case States.HideForSetting:
                        HideMenuForSetting();
                        break;
                    case States.Show:
                        if (state == States.HideForSetting) ShowMenuAfterSettingClose();
                        else if (Client.Instance.IsLogin() || Client.Instance.IsPlaying()) ShowMenu();
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
            if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
            if (StoryManager.Instance.M_StateMachine.GetState() == StoryManager.StateMachine.States.Show) return;
            if (WinLostPanelManager.Instance.IsShow) return;
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if ((SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.Hide || SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.HideForPlay) && SettingMenuManager.Instance.M_StateMachine.GetState() == SettingMenuManager.StateMachine.States.Hide)
                {
                    switch (state)
                    {
                        case States.Default:
                            SetState(States.Show);
                            break;
                        case States.Hide:
                            SetState(States.Show);
                            break;
                        case States.HideForSetting:
                            break;
                        case States.Show:
                            SetState(States.Hide);
                            break;
                    }
                }
            }

            bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
            if (isClickElseWhere)
            {
                if (state == States.Show)
                {
                    SetState(States.Hide);
                }
            }
        }

        public void ShowMenu()
        {
            Instance.ExitMenuCanvas.enabled = true;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.ExitMenu);
            if (Client.Instance.IsLogin()) StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Hide);
        }

        public void HideMenu()
        {
            Instance.ExitMenuCanvas.enabled = false;
            MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
            if (Client.Instance.IsLogin()) StartMenuManager.Instance.M_StateMachine.ReturnToPreviousState();
        }

        public void HideMenuForSetting()
        {
            Instance.ExitMenuCanvas.enabled = false;
        }

        public void ShowMenuAfterSettingClose()
        {
            Instance.ExitMenuCanvas.enabled = true;
        }
    }


    [SerializeField] private Canvas ExitMenuCanvas;
    [SerializeField] private Button SettingMenuButton;
    [SerializeField] private Button SurrenderButton;
    [SerializeField] private Button ConsumeButton;
    [SerializeField] private Button QuitGameButton;

    [SerializeField] private Text SettingMenuText;
    [SerializeField] private Text SurrenderText;
    [SerializeField] private Text ConsumeText;
    [SerializeField] private Text QuitText;

    public void OnSettingMenuButtonClick()
    {
        SettingMenuManager.Instance.M_StateMachine.SetState(SettingMenuManager.StateMachine.States.ShowFromExitMenu);
    }

    public void OnConsumeGameButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    public void OnSurrenderButtonClick()
    {
        ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
        cw.Initialize(
            LanguageManager.Instance.GetText("ExitMenu_SureToSurrender"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_Cancel"),
            (new UnityAction(SurrenderCore)) + ConfirmWindowManager.Instance.RemoveConfirmWindow,
            ConfirmWindowManager.Instance.RemoveConfirmWindow
        );
    }

    private void SurrenderCore()
    {
        Client.Instance.Proxy.LeaveGame();
        SelectBuildManager.Instance.ResetStoryBonusInfo();
        RoundManager.Instance.StopGame();
        ClientLog.Instance.Print("You have quit the game");
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("ExitMenu_YouHaveQuitGame"), 0, 1f);
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }
}
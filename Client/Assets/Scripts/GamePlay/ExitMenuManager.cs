using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitMenuManager : MonoSingletion<ExitMenuManager>
{
    private ExitMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();

        SettingMenuText.text = GameManager.Instance.isEnglish ? "Settings" : "设置";
        SurrenderText.text = GameManager.Instance.isEnglish ? "Surrender" : "认输";
        ConsumeText.text = GameManager.Instance.isEnglish ? "Consume" : "继续游戏";
        QuitText.text = GameManager.Instance.isEnglish ? "Quit" : "退出游戏";
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
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.Hide && SettingMenuManager.Instance.M_StateMachine.GetState() == SettingMenuManager.StateMachine.States.Hide)
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
            if (Client.Instance.IsLogin()) StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);
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
            GameManager.Instance.isEnglish ? "Are you sure to surrender?" : "您确定要认输吗?",
            GameManager.Instance.isEnglish ? "Yes" : "是",
            GameManager.Instance.isEnglish ? "No" : "取消",
            (new UnityAction(SurrenderCore)) + cw.PoolRecycle,
            cw.PoolRecycle
        );
    }

    private void SurrenderCore()
    {
        Client.Instance.Proxy.LeaveGame();
        RoundManager.Instance.StopGame();
        ClientLog.Instance.Print("You have quit the game");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "You have quit the game" : "您已退出比赛", 0, 1f);
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }
}
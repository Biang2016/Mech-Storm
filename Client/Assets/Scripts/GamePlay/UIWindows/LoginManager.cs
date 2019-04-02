using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginManager : MonoSingleton<LoginManager>
{
    private LoginManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (ServerText, "LoginMenu_ServerText"),
                (UserNameText, "LoginMenu_UserNameText"),
                (PasswordText, "LoginMenu_PasswordText"),
                (RegisterText, "LoginMenu_RegisterText"),
                (LoginText, "LoginMenu_LoginText"),
            });

        ServerDropdown.options[0] = new Dropdown.OptionData(LanguageManager.Instance.GetText("Server_FormalServer"));
        ServerDropdown.options[1] = new Dropdown.OptionData(LanguageManager.Instance.GetText("Server_TestServer"));
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Show);
        ServerDropdown.onValueChanged.AddListener(OnChangeServer);
        NetworkManager.Instance.ConnectToFormalServer();
        UserNameInputField.ActivateInputField();
    }

    void Update()
    {
        M_StateMachine.Update();
        if (M_StateMachine.GetState() == StateMachine.States.Show)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnLoginButtonClick();
            }
        }
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
                StoryManager.Instance.M_StateMachine.SetState(StoryManager.StateMachine.States.Hide);
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText(LanguageManager.Instance.GetText("LoginMenu_Disconnected"), 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText(LanguageManager.Instance.GetText("LoginMenu_Connected"), 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.Login:
                M_StateMachine.SetState(StateMachine.States.Hide);
                break;
            case ProxyBase.ClientStates.Matching:
                M_StateMachine.SetState(StateMachine.States.Hide);
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

                    case States.Show:
                        if (Client.Instance.IsLogin()) return;
                        ShowMenu();
                        break;
                }

                previousState = state;
                state = newState;
            }

            previousState = state;
            state = newState;
        }

        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        public States GetState()
        {
            return state;
        }

        private float thunderTicker = 0;
        private float thunderInterval = 10f;

        public void Update()
        {
            if (state == States.Show)
            {
                thunderTicker += Time.deltaTime;
                if (thunderTicker > thunderInterval)
                {
                    thunderTicker = 0;
                    AudioManager.Instance.SoundPlay("sfx/Thunder" + Random.Range(0, 3));
                }
            }
        }

        private void ShowMenu()
        {
            Instance.LoginCanvas.enabled = true;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.None);
            SelectBuildManager.Instance.GameMode_State = SelectBuildManager.GameMode.None;
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/Login_0", "bgm/Login_1"});
            AudioManager.Instance.SoundPlay("sfx/Thunder1");
        }

        private void HideMenu()
        {
            Instance.LoginCanvas.enabled = false;
        }
    }

    [SerializeField] private Canvas LoginCanvas;
    [SerializeField] private Dropdown ServerDropdown;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private InputField UserNameInputField;
    [SerializeField] private InputField PasswordInputField;
    [SerializeField] private Text TipText;

    [SerializeField] private Text ServerText;
    [SerializeField] private Text UserNameText;
    [SerializeField] private Text PasswordText;
    [SerializeField] private Text RegisterText;
    [SerializeField] private Text LoginText;

    public void OnChangeServer(int value)
    {
        switch (value)
        {
            case 0:
                NetworkManager.Instance.ConnectToFormalServer();
                break;
            case 1:
                NetworkManager.Instance.ConnectToTestServer();
                break;
        }
    }

    public void OnRegisterButtonClick()
    {
        if (Client.Instance.IsConnect())
        {
            RegisterRequest request = new RegisterRequest(Client.Instance.Proxy.ClientId, UserNameInputField.text, PasswordInputField.text);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            if (Client.Instance.ClientInvalid)
            {
                ShowUpdateConfirmWindow();
            }
            else
            {
                ShowTipText(LanguageManager.Instance.GetText("LoginMenu_Connecting"), 0, float.PositiveInfinity, true);
                OnChangeServer(ServerDropdown.value);
            }
        }
    }

    public void OnLoginButtonClick()
    {
        if (Client.Instance.IsConnect())
        {
            LoginRequest request = new LoginRequest(Client.Instance.Proxy.ClientId, UserNameInputField.text, PasswordInputField.text);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            if (Client.Instance.ClientInvalid)
            {
                ShowUpdateConfirmWindow();
            }
            else
            {
                ShowTipText(LanguageManager.Instance.GetText("LoginMenu_Connecting"), 0, float.PositiveInfinity, true);
                OnChangeServer(ServerDropdown.value);
            }
        }
    }

    public void OnQuitButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }

    public void ShowUpdateConfirmWindow()
    {
        ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(LoginManager.Instance.transform.parent);
        cw.Initialize(
            LanguageManager.Instance.GetText("LoginMenu_ClientNeedUpdate"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_Cancel"),
            (new UnityAction(delegate { Application.OpenURL("www.biangstudio.com/mech-storm"); })) + ConfirmWindowManager.Instance.RemoveConfirmWindow,
            ConfirmWindowManager.Instance.RemoveConfirmWindow
        );
    }

    IEnumerator ShowTipTextCoroutine;

    public void ShowTipText(string text, float delay, float last, bool showDots)
    {
        if (Instance == null) return;
        if (ShowTipTextCoroutine != null)
        {
            StopCoroutine(ShowTipTextCoroutine);
        }

        ShowTipTextCoroutine = Co_ShowTipText(text, delay, last, showDots);
        StartCoroutine(ShowTipTextCoroutine);
    }

    IEnumerator Co_ShowTipText(string text, float delay, float last, bool showDots)
    {
        yield return new WaitForSeconds(delay);
        TipText.text = text;
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            TipText.text = "";
        }
        else
        {
            if (showDots)
            {
                int dotCount = 0;
                while (true)
                {
                    TipText.text += ".";
                    yield return new WaitForSeconds(0.5f);
                    dotCount++;
                    if (dotCount == 3)
                    {
                        dotCount = 0;
                        TipText.text = text;
                    }
                }
            }
            else
            {
                while (true)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoSingletion<LoginManager>
{
    private LoginManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        ServerText.text = GameManager.Instance.isEnglish ? "Server: " : "服务器: ";
        UserNameText.text = GameManager.Instance.isEnglish ? "Username: " : "用户名: ";
        PasswordText.text = GameManager.Instance.isEnglish ? "Password: " : "密码: ";
        RegisterText.text = GameManager.Instance.isEnglish ? "Register" : "注册";
        LoginText.text = GameManager.Instance.isEnglish ? "Login" : "登录";
        ServerDropdown.options[0] = new Dropdown.OptionData(GameManager.Instance.isEnglish ? "Test Server" : "测试服");
        ServerDropdown.options[1] = new Dropdown.OptionData(GameManager.Instance.isEnglish ? "Formal Server" : "正式服");
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Show);
        ServerDropdown.onValueChanged.AddListener(OnChangeServer);
        NetworkManager.Instance.ConnectToTestServer();
    }

    void Update()
    {
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText(GameManager.Instance.isEnglish ? "Disconnected" : "已断开连接.", 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText(GameManager.Instance.isEnglish ? "Connected" : "已连接.", 0, float.PositiveInfinity, false);
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

        public void Update()
        {
        }

        private void ShowMenu()
        {
            Instance.LoginCanvas.enabled = true;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.None);
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/LoginMenu0", "bgm/LoginMenu1"});
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
                NetworkManager.Instance.ConnectToTestServer();
                break;
            case 1:
                NetworkManager.Instance.ConnectToFormalServer();
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
            ShowTipText(GameManager.Instance.isEnglish ? "Connecting" : "正在连接服务器", 0, float.PositiveInfinity, true);
            OnChangeServer(ServerDropdown.value);
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
            ShowTipText(GameManager.Instance.isEnglish ? "Connecting" : "正在连接服务器", 0, float.PositiveInfinity, true);
            OnChangeServer(ServerDropdown.value);
        }
    }

    public void OnQuitButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }

    IEnumerator ShowTipTextCoroutine;

    public void ShowTipText(string text, float delay, float last, bool showDots)
    {
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
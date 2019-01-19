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

        ServerText.text = GameManager.Instance.IsEnglish ? "Server: " : "服务器: ";
        UserNameText.text = GameManager.Instance.IsEnglish ? "Username: " : "用户名: ";
        PasswordText.text = GameManager.Instance.IsEnglish ? "Password: " : "密码: ";
        RegisterText.text = GameManager.Instance.IsEnglish ? "Register" : "注册";
        LoginText.text = GameManager.Instance.IsEnglish ? "Login" : "登录";

        ServerText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        PasswordText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        PasswordText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        RegisterText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        LoginText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        ServerDropdown.options[0] = new Dropdown.OptionData(GameManager.Instance.IsEnglish ? "Formal Server" : "正式服");
        ServerDropdown.options[1] = new Dropdown.OptionData(GameManager.Instance.IsEnglish ? "Test Server" : "测试服");
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
                ShowTipText(GameManager.Instance.IsEnglish ? "Disconnected" : "已断开连接.", 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText(GameManager.Instance.IsEnglish ? "Connected" : "已连接.", 0, float.PositiveInfinity, false);
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
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/LoginMenu0", "bgm/LoginMenu1"});
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
                ShowTipText(GameManager.Instance.IsEnglish ? "Connecting" : "正在连接服务器", 0, float.PositiveInfinity, true);
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
                ShowTipText(GameManager.Instance.IsEnglish ? "Connecting" : "正在连接服务器", 0, float.PositiveInfinity, true);
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
            GameManager.Instance.IsEnglish ? "Your client needs update! Open the download page in browser?" : "您的客户端需要更新,是否在浏览器中打开下载页面?",
            GameManager.Instance.IsEnglish ? "Yes" : "是",
            GameManager.Instance.IsEnglish ? "No" : "取消",
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
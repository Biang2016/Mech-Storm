using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

internal class LoginManager : MonoSingletion<LoginManager>
{
    private LoginManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;
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
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText("已断开连接.", 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Show);
                ShowTipText("已连接.", 0, float.PositiveInfinity, false);
                break;
            case ProxyBase.ClientStates.Login:
                M_StateMachine.SetState(StateMachine.States.Hide);
                break;
            case ProxyBase.ClientStates.SubmitCardDeck:
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
                        if (!Client.Instance.IsLogin()) return;
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
            ShowTipText("正在连接服务器", 0, float.PositiveInfinity, true);
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
            ShowTipText("正在连接服务器", 0, float.PositiveInfinity, true);
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
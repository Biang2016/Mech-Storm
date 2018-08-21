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
        Proxy.OnClientStateChange += OnClientChangeState;
    }

    void Start()
    {
        ShowCanvas();
        ServerDropdown.onValueChanged.AddListener(OnChangeServer);
        NetworkManager.Instance.ConnectToTestServer();
    }

    void Update()
    {
        if (Client.Instance.IsConnect())
        {
            EnableRegisterAndLoginButton();
        }
        else
        {
            UnenableRegisterAndLoginButton();
        }
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Nothing:
                ShowCanvas();
                break;
            case ProxyBase.ClientStates.Login:
                HideCanvas();
                break;
            case ProxyBase.ClientStates.SubmitCardDeck:
                break;
            case ProxyBase.ClientStates.Matching:
                break;
            case ProxyBase.ClientStates.Playing:
                break;
        }
    }

    [SerializeField] private Canvas LoginCanvas;
    [SerializeField] private Dropdown ServerDropdown;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private InputField UserNameInputField;
    [SerializeField] private InputField PasswordInputField;

    public void OnChangeServer(int value)
    {
        switch (ServerDropdown.value)
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
            RegisterRequest request = new RegisterRequest(UserNameInputField.text, PasswordInputField.text);
            Client.Instance.Proxy.SendMessage(request);
        }
    }

    public void OnLoginButtonClick()
    {
        if (Client.Instance.IsConnect())
        {
            LoginRequest request = new LoginRequest(UserNameInputField.text, PasswordInputField.text);
            Client.Instance.Proxy.SendMessage(request);
        }
    }

    public void ShowCanvas()
    {
        LoginCanvas.gameObject.SetActive(true);
    }

    public void HideCanvas()
    {
        LoginCanvas.gameObject.SetActive(false);
    }

    public void EnableRegisterAndLoginButton()
    {
        RegisterButton.enabled = true;
        LoginButton.enabled = true;
    }

    public void UnenableRegisterAndLoginButton()
    {
        RegisterButton.enabled = false;
        LoginButton.enabled = false;
    }
}
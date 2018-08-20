using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

internal class LoginManager : MonoSingletion<LoginManager>
{
    private LoginManager()
    {
    }

    void Start()
    {
        LoginCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    [SerializeField] private Canvas LoginCanvas;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private InputField UserNameInputField;
    [SerializeField] private InputField PasswordInputField;


    public void OnRegisterButtonClick()
    {
        RegisterRequest request=new RegisterRequest();
    }

    public void OnLoginButtonClick()
    {

    }
}
using UnityEditor;
using UnityEngine;

internal class MainMenuManager : MonoSingletion<MainMenuManager>
{
    private MainMenuManager()
    {
    }

    void Start()
    {
        HideMenu();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            switch (MainMenuState)
            {
                case MainMenuStates.Hide:
                    ShowMenu();
                    break;
                case MainMenuStates.Show:
                    HideMenu();
                    break;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (MainMenuState == MainMenuStates.Show)
            {
                HideMenu();
            }
        }
    }

    public enum MainMenuStates
    {
        Hide = 0,
        Show = 1,
    }

    public MainMenuStates MainMenuState;

    public void HideMenu()
    {
        MainMenuState = MainMenuStates.Hide;
        MainMenuCanvas.enabled = false;
    }

    public void ShowMenu()
    {
        MainMenuState = MainMenuStates.Show;
        MainMenuCanvas.enabled = true;
    }

    [SerializeField] private Canvas MainMenuCanvas;

    public void OnConsumeGameButtonClick()
    {
        HideMenu();
    }

    public void OnTerminateConnectionButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        HideMenu();
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }

    public void OnConnectToTestServerButtonClick()
    {
        NetworkManager.Instance.ConnectToTestServer();
        HideMenu();
    }

    public void OnConnectToFormalServerButtonClick()
    {
        NetworkManager.Instance.ConnectToFormalServer();
        HideMenu();
    }
}
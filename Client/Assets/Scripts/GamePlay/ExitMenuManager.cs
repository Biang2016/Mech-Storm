using UnityEditor;
using UnityEngine;

internal class ExitMenuManager : MonoSingletion<ExitMenuManager>
{
    private ExitMenuManager()
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
            switch (ExitMenuState)
            {
                case ExitMenuStates.Hide:
                    ShowMenu();
                    break;
                case ExitMenuStates.Show:
                    HideMenu();
                    break;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (ExitMenuState == ExitMenuStates.Show)
            {
                HideMenu();
            }
        }
    }

    public enum ExitMenuStates
    {
        Hide = 0,
        Show = 1,
    }

    public ExitMenuStates ExitMenuState;

    public void HideMenu()
    {
        ExitMenuState = ExitMenuStates.Hide;
        ExitMenuCanvas.enabled = false;
        MouseHoverManager.Instance.ReturnToPreviousState();
    }

    public void ShowMenu()
    {
        ExitMenuState = ExitMenuStates.Show;
        ExitMenuCanvas.enabled = true;
        MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.ExitMenu);
    }

    [SerializeField] private Canvas ExitMenuCanvas;

    public void OnConsumeGameButtonClick()
    {
        HideMenu();
    }

    public void OnSurrenderButtonClick()
    {
        Client.Instance.Proxy.LeaveGame();
        RoundManager.Instance.StopGame();
        ClientLog.Instance.Print("您已退出比赛");
        NoticeManager.Instance.ShowInfoPanel("您已退出比赛", 0, 1f);
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }
}
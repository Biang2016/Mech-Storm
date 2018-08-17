using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;
using Button = UnityEngine.UI.Button;

internal class StartMenuManager : MonoSingletion<StartMenuManager>
{
    private StartMenuManager()
    {
    }

    void Start()
    {
        ShowMenu();
        StartMatchButton.gameObject.SetActive(false);
        CancelMatchButton.gameObject.SetActive(false);
        SwitchServerButton.gameObject.SetActive(true);
        ShowServerList();
        Proxy.OnClientStateChange += OnClientChangeState;
    }

    void Update()
    {
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Nothing:
                ShowMenu();
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                SwitchServerButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.GetId:
                ShowMenu();
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                SwitchServerButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.SubmitCardDeck:
                ShowMenu();
                StartMatchButton.gameObject.SetActive(true);
                CancelMatchButton.gameObject.SetActive(false);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                SwitchServerButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Matching:
                ShowMenu();
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(true);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                SwitchServerButton.gameObject.SetActive(false);
                ServerList.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.Playing:
                HideMenu();
                break;
        }
    }

    public enum StartMenuStates
    {
        Hide = 0,
        Show = 1,
    }

    public StartMenuStates StartMenuState;

    public void HideMenu()
    {
        StartMenuState = StartMenuStates.Hide;
        StartMenuCanvas.enabled = false;
    }

    public void ShowMenu()
    {
        StartMenuState = StartMenuStates.Show;
        StartMenuCanvas.enabled = true;
        MouseHoverManager.Instance.SetState(MouseHoverManager.MHM_States.StartMenu);
    }

    [SerializeField] private Canvas StartMenuCanvas;
    [SerializeField] private Button StartMatchButton;
    [SerializeField] private Button CancelMatchButton;
    [SerializeField] private Button SelectCardDeckWindowButton;
    [SerializeField] private Button SwitchServerButton;
    [SerializeField] private Button QuitGameButton;
    [SerializeField] private Transform ServerList;

    public void OnStartMatchGameButtonClick()
    {
        Client.Instance.Proxy.OnBeginMatch();
        ClientLog.Instance.Print("开始匹配");
        NoticeManager.Instance.ShowInfoPanel("匹配中", 0, float.PositiveInfinity);
    }

    public void OnCancelMatchGameButtonClick()
    {
        Client.Instance.Proxy.CancelMatch();
        ClientLog.Instance.Print("取消匹配");
        NoticeManager.Instance.ShowInfoPanel("取消匹配", 0, 1f);
    }

    public void OnSelectCardDeckWindowButtonClick()
    {
        HideMenu();
        SelectCardDeckManager.Instance.ShowWindow();
    }

    public void OnMouseHoverSwitchServerButton()
    {
        ShowServerList();
    }

    public void OnMouseLeaveSwitchServerButton()
    {
        HideServerList();
    }

    public void OnConnectToTestServerButtonClick()
    {
        NetworkManager.Instance.ConnectToTestServer();
        HideServerList();
    }

    public void OnConnectToFormalServerButtonClick()
    {
        NetworkManager.Instance.ConnectToFormalServer();
        HideServerList();
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }

    private void HideServerList()
    {
        ServerList.gameObject.SetActive(false);
    }

    private void ShowServerList()
    {
        ServerList.gameObject.SetActive(true);
    }
}
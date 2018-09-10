using UnityEngine;
using UnityEngine.UI;

public class StartMenuManager : MonoSingletion<StartMenuManager>
{
    private StartMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        BeginMatchText.text = GameManager.Instance.isEnglish ? "Game Begin" : "开始匹配";
        CancelMatchText.text = GameManager.Instance.isEnglish ? "Cancel Match" : "取消匹配";
        MyDeckText.text = GameManager.Instance.isEnglish ? "My Decks" : "我的卡组";
        QuitGameText.text = GameManager.Instance.isEnglish ? "Quit Game" : "退出游戏";
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        StartMatchButton.gameObject.SetActive(false);
        CancelMatchButton.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                M_StateMachine.SetState(StateMachine.States.Hide);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.GetId:
                M_StateMachine.SetState(StateMachine.States.Hide);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.Login:
                M_StateMachine.SetState(StateMachine.States.Show);
                StartMatchButton.gameObject.SetActive(true);
                CancelMatchButton.gameObject.SetActive(false);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Matching:
                M_StateMachine.SetState(StateMachine.States.Show);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(true);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(false);
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
            Instance.StartMenuCanvas.enabled = true;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
        }

        private void HideMenu()
        {
            Instance.StartMenuCanvas.enabled = false;
        }
    }

    [SerializeField] private Canvas StartMenuCanvas;
    [SerializeField] private Button StartMatchButton;
    [SerializeField] private Button CancelMatchButton;
    [SerializeField] private Button SelectCardDeckWindowButton;
    [SerializeField] private Button QuitGameButton;

    [SerializeField] private Text BeginMatchText;
    [SerializeField] private Text CancelMatchText;
    [SerializeField] private Text MyDeckText;
    [SerializeField] private Text QuitGameText;

    public void OnStartMatchGameButtonClick()
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Login && SelectBuildManager.Instance.CurrentSelectedBuildButton == null) //未发送卡组则跳出选择卡组界面
        {
            OnSelectCardDeckWindowButtonClick();
        }
        else
        {
            Client.Instance.Proxy.OnBeginMatch();
            ClientLog.Instance.Print("开始匹配");
            NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Matching" : "匹配中", 0, float.PositiveInfinity);
        }
    }

    public void OnCancelMatchGameButtonClick()
    {
        Client.Instance.Proxy.CancelMatch();
        ClientLog.Instance.Print("取消匹配");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.isEnglish ? "Match canceled" : "取消匹配", 0, 1f);
    }

    public void OnSelectCardDeckWindowButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Show);
    }

    public void OnQuitGameButtonClick()
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            Client.Instance.Proxy.CancelMatch();
        }

        LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientId, Client.Instance.Proxy.Username);
        Client.Instance.Proxy.SendMessage(request);
    }
}
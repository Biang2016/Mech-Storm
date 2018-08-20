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

    void Awake()
    {
        M_StateMachine = new StateMachine();
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        StartMatchButton.gameObject.SetActive(false);
        CancelMatchButton.gameObject.SetActive(false);
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
                M_StateMachine.SetState(StateMachine.States.Hide);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.Login:
                M_StateMachine.SetState(StateMachine.States.Show);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(false);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.SubmitCardDeck:
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
            Instance.StartMenuCanvas.enabled = true;
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.Menu);
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
        M_StateMachine.SetState(StateMachine.States.Hide);
        SelectCardDeckManager.Instance.M_StateMachine.SetState(SelectCardDeckManager.StateMachine.States.Show);
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }
}
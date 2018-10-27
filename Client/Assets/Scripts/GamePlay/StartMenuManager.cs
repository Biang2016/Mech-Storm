using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartMenuManager : MonoSingleton<StartMenuManager>
{
    private StartMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        Proxy.OnClientStateChange += OnClientChangeState;

        BeginMatchText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        CancelMatchText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        MyDeckText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        SettingText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        AboutText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        QuitGameText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DesignerText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        BeginMatchText.text = GameManager.Instance.IsEnglish ? "Game Begin" : "开始匹配";
        CancelMatchText.text = GameManager.Instance.IsEnglish ? "Cancel Match" : "取消匹配";
        MyDeckText.text = GameManager.Instance.IsEnglish ? "My Decks" : "我的卡组";
        SettingText.text = GameManager.Instance.IsEnglish ? "Settings" : "游戏设置";
        AboutText.text = GameManager.Instance.IsEnglish ? "About Me" : "制作人员";
        QuitGameText.text = GameManager.Instance.IsEnglish ? "Quit Game" : "退出游戏";

        DesignerText.text = GameManager.Instance.IsEnglish
            ? "Designer\nXue Bingsheng"
            : "制作人: 薛炳晟";

        if (GameManager.Instance.IsEnglish)
        {
            DeckAbstractText_DeckName.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Cards.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_CardNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Life.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_LifeNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Energy.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_EnergyNum.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_Draws.fontStyle = FontStyle.BoldAndItalic;
            DeckAbstractText_DrawNum.fontStyle = FontStyle.BoldAndItalic;
        }
        else
        {
            DeckAbstractText_DeckName.fontStyle = FontStyle.Italic;
            DeckAbstractText_Cards.fontStyle = FontStyle.Italic;
            DeckAbstractText_CardNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Life.fontStyle = FontStyle.Italic;
            DeckAbstractText_LifeNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Energy.fontStyle = FontStyle.Italic;
            DeckAbstractText_EnergyNum.fontStyle = FontStyle.Italic;
            DeckAbstractText_Draws.fontStyle = FontStyle.Italic;
            DeckAbstractText_DrawNum.fontStyle = FontStyle.Italic;
        }

        DeckAbstractText_DeckName.text = GameManager.Instance.IsEnglish ? "No Deck" : "无卡组";
        DeckAbstractText_Cards.text = GameManager.Instance.IsEnglish ? "Cards" : "卡牌数:";
        DeckAbstractText_CardNum.text = "";
        DeckAbstractText_Life.text = GameManager.Instance.IsEnglish ? "Life" : "生命值:";
        DeckAbstractText_LifeNum.text = "";
        DeckAbstractText_Energy.text = GameManager.Instance.IsEnglish ? "Energy" : "能量上限:";
        DeckAbstractText_EnergyNum.text = "";
        DeckAbstractText_Draws.text = GameManager.Instance.IsEnglish ? "Draws" : "抽牌数:";
        DeckAbstractText_DrawNum.text = "";

        DeckAbstractText_DeckName.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Cards.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_CardNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Life.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_LifeNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Energy.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_EnergyNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_Draws.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        DeckAbstractText_DrawNum.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
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
                GameBoardManager.Instance.ChangeBoardBG();
                M_StateMachine.SetState(StateMachine.States.Show);
                StartMatchButton.gameObject.SetActive(true);
                CancelMatchButton.gameObject.SetActive(false);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                SettingButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Matching:
                M_StateMachine.SetState(StateMachine.States.Show);
                StartMatchButton.gameObject.SetActive(false);
                CancelMatchButton.gameObject.SetActive(true);
                SelectCardDeckWindowButton.gameObject.SetActive(true);
                SettingButton.gameObject.SetActive(true);
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
            Instance.RefreshBuildInfoAbstract();

            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.StartMenu);
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/StartMenuBGM0", "bgm/StartMenuBGM1"});
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
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button QuitGameButton;

    [SerializeField] private GameObject DeckAbstract;

    [SerializeField] private Text BeginMatchText;
    [SerializeField] private Text CancelMatchText;
    [SerializeField] private Text MyDeckText;
    [SerializeField] private Text SettingText;
    [SerializeField] private Text AboutText;
    [SerializeField] private Text QuitGameText;

    [SerializeField] private Text DesignerText;

    [SerializeField] private Text DeckAbstractText_DeckName;
    [SerializeField] private Text DeckAbstractText_Cards;
    [SerializeField] private Text DeckAbstractText_CardNum;
    [SerializeField] private Text DeckAbstractText_Life;
    [SerializeField] private Text DeckAbstractText_LifeNum;
    [SerializeField] private Text DeckAbstractText_Energy;
    [SerializeField] private Text DeckAbstractText_EnergyNum;
    [SerializeField] private Text DeckAbstractText_Draws;
    [SerializeField] private Text DeckAbstractText_DrawNum;

    public void RefreshBuildInfoAbstract()
    {
        if (SelectBuildManager.Instance.CurrentSelectedBuildButton != null)
        {
            DeckAbstractText_DeckName.text = "[ " + SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildName + " ]";
            DeckAbstractText_CardNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.CardCount().ToString();
            DeckAbstractText_LifeNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.Life.ToString();
            DeckAbstractText_EnergyNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.Energy.ToString();
            DeckAbstractText_DrawNum.text = SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.DrawCardNum.ToString();
        }
    }

    public void OnStartMatchGameButtonClick()
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Login)
        {
            if (SelectBuildManager.Instance.CurrentSelectedBuildButton == null) //未发送卡组则跳出选择卡组界面
            {
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.CardCount() == 0)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Your deck is empty" : "您的卡组中没有卡牌", 0, 0.3f);
                OnSelectCardDeckWindowButtonClick();
                return;
            }
            else if (!SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.IsEnergyEnough)
            {
                ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
                if (GameManager.Instance.IsEnglish)
                {
                    cw.Initialize("Some cards consume more energy than your upper limit.", "Go ahead", "Edit",
                        new UnityAction(StartMatchingCore) + cw.PoolRecycle,
                        new UnityAction(OnSelectCardDeckWindowButtonClick) + cw.PoolRecycle);
                }
                else
                {
                    cw.Initialize("您的卡组中有些牌的能量消耗大于您的能量上限，是否继续？", "继续", "编辑",
                        new UnityAction(StartMatchingCore) + cw.PoolRecycle,
                        new UnityAction(OnSelectCardDeckWindowButtonClick) + cw.PoolRecycle);
                }

                return;
            }
            else
            {
                StartMatchingCore();
            }
        }
    }

    private void StartMatchingCore()
    {
        Client.Instance.Proxy.OnBeginMatch();
        ClientLog.Instance.Print(GameManager.Instance.IsEnglish ? "Begin matching" : "开始匹配");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Matching" : "匹配中", 0, float.PositiveInfinity);
        DeckAbstract.SetActive(true);
    }

    public void OnCancelMatchGameButtonClick()
    {
        Client.Instance.Proxy.CancelMatch();
        ClientLog.Instance.Print(GameManager.Instance.IsEnglish ? "Matching canceled" : "取消匹配");
        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Match canceled" : "取消匹配", 0, 1f);
        DeckAbstract.SetActive(false);
    }

    public void OnSelectCardDeckWindowButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        if (Client.Instance.IsLogin() && !Client.Instance.IsMatching()) SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Show);
        else if (Client.Instance.IsPlaying() || Client.Instance.IsMatching()) SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Show_ReadOnly);
    }

    public void OnSettingButtonClick()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        SettingMenuManager.Instance.M_StateMachine.SetState(SettingMenuManager.StateMachine.States.ShowFromStartMenu);
    }

    public void OnQuitGameButtonClick()
    {
        ConfirmWindow cw = GameObjectPoolManager.Instance.Pool_ConfirmWindowPool.AllocateGameObject<ConfirmWindow>(transform.parent);
        cw.Initialize(
            GameManager.Instance.IsEnglish ? "Are you sure to Quick the game?" : "退出游戏?",
            GameManager.Instance.IsEnglish ? "Yes" : "是",
            GameManager.Instance.IsEnglish ? "No" : "取消",
            Application.Quit,
            cw.PoolRecycle
        );

        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Matching)
        {
            Client.Instance.Proxy.CancelMatch();
        }

        //LogoutRequest request = new LogoutRequest(Client.Instance.Proxy.ClientId, Client.Instance.Proxy.Username);
        //Client.Instance.Proxy.SendMessage(request);
    }
}
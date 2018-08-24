using UnityEngine;
using UnityEngine.UI;

internal partial class RoundManager : MonoSingletion<RoundManager>
{
    private RoundManager()
    {
    }

    internal int RoundNumber;
    internal RandomNumberGenerator RandomNumberGenerator;
    internal ClientPlayer SelfClientPlayer;
    internal ClientPlayer EnemyClientPlayer;
    internal ClientPlayer CurrentClientPlayer;
    internal ClientPlayer IdleClientPlayer;

    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private GameObject SelfTurnText;
    [SerializeField] private GameObject EnemyTurnText;
    [SerializeField] private GameObject EndRoundButton;
    [SerializeField] public Text SelfCostText;
    [SerializeField] public Text EnemyCostText;

    void Awake()
    {
        BattleCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isStop)
        {
            OnGameStop();
            isStop = false;
        }
    }

    private void Initialize()
    {
        RoundNumber = 0;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        BattleCanvas.gameObject.SetActive(true);
        SelfTurnText.SetActive(false);
        EnemyTurnText.SetActive(false);
        EndRoundButton.SetActive(false);
        SelfCostText.gameObject.SetActive(true);
        EnemyCostText.gameObject.SetActive(true);
        SelfCostText.text = "";
        EnemyCostText.text = "";

        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);

        CardDeckManager.Instance.ShowAll();
    }

    private void InitializePlayers(SetPlayerRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.costLeft, r.costMax, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.costLeft, r.costMax, Players.Enemy);
            EnemyClientPlayer.ClientId = r.clientId;
        }
    }


    private void BeginRound()
    {
        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    private void EndRound()
    {
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    bool isStop = false;

    public void StopGame()
    {
        isStop = true; //标记为，待Update的时候正式处理OnGameStop
    }

    private void OnGameStop()
    {
        NoticeManager.Instance.ShowInfoPanelTop("游戏结束", 0f, 0.5f);
        CardBase[] cardPreviews = GameBoardManager.Instance.CardDetailPreview.transform.GetComponentsInChildren<CardBase>();
        foreach (CardBase cardPreview in cardPreviews)
        {
            cardPreview.PoolRecycle();
        }

        ModuleBase[] modulePreviews = GameBoardManager.Instance.CardDetailPreview.transform.GetComponentsInChildren<ModuleBase>();
        foreach (ModuleBase modulePreview in modulePreviews)
        {
            modulePreview.PoolRecycle();
        }

        GameBoardManager.Instance.CardDetailPreview.transform.DetachChildren();

        GameBoardManager.Instance.SelfBattleGroundManager.Reset();
        GameBoardManager.Instance.EnemyBattleGroundManager.Reset();
        GameBoardManager.Instance.SelfHandManager.Reset();
        GameBoardManager.Instance.EnemyHandManager.Reset();
        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;
        SelfCostText.text = "";
        EnemyCostText.text = "";
        RoundNumber = 0;
        SelfTurnText.SetActive(false);
        EnemyTurnText.SetActive(false);
        EndRoundButton.SetActive(false);
        SelfCostText.gameObject.SetActive(false);
        EnemyCostText.gameObject.SetActive(false);
        BattleCanvas.gameObject.SetActive(false);

        CardDeckManager.Instance.HideAll();
        RandomNumberGenerator = null;

        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);

        BattleEffectsManager.Instance.Effect_Main.AllEffectsEnd();
        BattleEffectsManager.Instance.Effect_RefreshBattleGroundOnAddRetinue.AllEffectsEnd();
    }

    #region 交互

    public void OnEndRoundButtonClick()
    {
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            EndRoundRequest request = new EndRoundRequest(Client.Instance.Proxy.ClientId);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            ClientLog.Instance.PrintWarning("不是你的回合");
        }
    }

    #endregion

    #region Utils

    public ClientPlayer GetPlayerByClientId(int clientId)
    {
        if (Client.Instance.Proxy.ClientId == clientId) return SelfClientPlayer;
        return EnemyClientPlayer;
    }

    #endregion
}
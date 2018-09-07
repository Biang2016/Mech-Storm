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
    [SerializeField] private Ship SelfShip;
    [SerializeField] private Ship EnemyShip;
    internal ClientPlayer CurrentClientPlayer;
    internal ClientPlayer IdleClientPlayer;

    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private Button EndRoundButton;
    [SerializeField] private Text EndRoundButtonText;

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
        SelfShip.ClientPlayer = SelfClientPlayer;
        EnemyShip.ClientPlayer = EnemyClientPlayer;

        RoundNumber = 0;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        GameBoardManager.Instance.ShowBattleShip();
        BattleCanvas.gameObject.SetActive(true);
        SetEndRoundButtonState(false);

        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);

        CardDeckManager.Instance.ShowAll();
    }

    private void InitializePlayers(SetPlayerRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Enemy);
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

    public bool HasShowLostConnectNotice = true;

    private void OnGameStop()
    {
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

        GameBoardManager.Instance.SelfBattleGroundManager.ResetAll();
        GameBoardManager.Instance.EnemyBattleGroundManager.ResetAll();
        GameBoardManager.Instance.SelfHandManager.ResetAll();
        GameBoardManager.Instance.EnemyHandManager.ResetAll();
        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;
        RoundNumber = 0;

        BattleCanvas.gameObject.SetActive(false);
        GameBoardManager.Instance.ResetAll();

        CardDeckManager.Instance.HideAll();
        RandomNumberGenerator = null;

        if (Client.Instance.Proxy != null && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        }
        else if (!Client.Instance.IsConnect() && !Client.Instance.IsLogin() && !Client.Instance.IsPlaying())
        {
            LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Show);
            if (!HasShowLostConnectNotice)
            {
                NoticeManager.Instance.ShowInfoPanelCenter("您已离线", 0, 1f);
                HasShowLostConnectNotice = true;
            }
        }

        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);
        BattleEffectsManager.Instance.ResetAll();
    }

    private void SetEndRoundButtonState(bool enable)
    {
        EndRoundButton.enabled = enable;
        EndRoundButton.image.color = enable ? Color.yellow : Color.gray;
        EndRoundButtonText.text = enable ? "结束回合" : "敌方回合";
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

    public void ShowTargetPreviewArrow(TargetSideEffect.TargetRange targetRange)
    {
        switch (targetRange)
        {
            case TargetSideEffect.TargetRange.All:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.BattleGrounds:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.SelfBattleGround:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.EnemyBattleGround:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.Heros:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.SelfHeros:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.EnemyHeros:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.SelfSoldiers:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.EnemySoldiers:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                break;
        }
    }

    public void HideTargetPreviewArrow()
    {
        foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.HideTargetPreviewArrow();
        foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.HideTargetPreviewArrow();
    }

    #endregion

    #region Utils

    public ClientPlayer GetPlayerByClientId(int clientId)
    {
        if (Client.Instance.Proxy.ClientId == clientId) return SelfClientPlayer;
        return EnemyClientPlayer;
    }

    public ModuleRetinue FindRetinue(int retinueId)
    {
        ModuleRetinue selfRetinue = SelfClientPlayer.MyBattleGroundManager.GetRetinue(retinueId);
        if (selfRetinue) return selfRetinue;
        ModuleRetinue enemyRetinue = EnemyClientPlayer.MyBattleGroundManager.GetRetinue(retinueId);
        if (enemyRetinue) return enemyRetinue;
        return null;
    }

    #endregion
}
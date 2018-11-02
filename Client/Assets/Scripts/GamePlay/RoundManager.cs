using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal partial class RoundManager : MonoSingleton<RoundManager>
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
        BattleCanvas.enabled = false;
    }

    private void Update()
    {
        if (isStop)
        {
            OnGameStop();
            isStop = false;
        }
    }

    public void Initialize()
    {
        RoundNumber = 0;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        GameBoardManager.Instance.ChangeBoardBG();
        GameBoardManager.Instance.ShowBattleShip();
        BattleCanvas.enabled = true;
        SetEndRoundButtonState(false);

        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.HideForPlay);
        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/BattleBGM0", "bgm/BattleBGM1"}, 0.7f);
        CardDeckManager.Instance.ResetCardDeckNumberText();
        CardDeckManager.Instance.ShowAll();
    }

    private void InitializePlayers(SetPlayerRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
            SelfShip.ClientPlayer = SelfClientPlayer;
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Enemy);
            EnemyClientPlayer.ClientId = r.clientId;
            EnemyShip.ClientPlayer = EnemyClientPlayer;
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

    public void OnGameStop()
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
        GameBoardManager.Instance.SelfPlayerBuffManager.ResetAll();
        GameBoardManager.Instance.EnemyPlayerBuffManager.ResetAll();

        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;
        RoundNumber = 0;

        BattleCanvas.enabled = false;
        GameBoardManager.Instance.ResetAll();

        CardDeckManager.Instance.HideAll();
        RandomNumberGenerator = null;

        if (Client.Instance.Proxy != null && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        }
        else if (!Client.Instance.IsConnect() && !Client.Instance.IsLogin() && !Client.Instance.IsPlaying())
        {
            SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
            LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Show);
            if (!HasShowLostConnectNotice)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Disconnected" : "您已离线", 0, 1f);
                HasShowLostConnectNotice = true;
            }
        }

        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show_Main);
        BattleEffectsManager.Instance.ResetAll();
    }

    private void SetEndRoundButtonState(bool enable)
    {
        EndRoundButton.enabled = enable;
        EndRoundButton.image.color = enable ? Color.yellow : Color.gray;
        EndRoundButtonText.text = enable ? (GameManager.Instance.IsEnglish ? "End Turn" : "结束回合") : (GameManager.Instance.IsEnglish ? "Enemy's Turn" : "敌方回合");
    }

    #region 交互

    public void OnEndRoundButtonClick()
    {
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            EndRoundRequest request = new EndRoundRequest(Client.Instance.Proxy.ClientId);
            Client.Instance.Proxy.SendMessage(request);
            SetEndRoundButtonState(false);
        }
        else
        {
            ClientLog.Instance.PrintWarning("Not Your Round");
        }
    }

    public void ShowRetinueAttackPreviewArrow(ModuleRetinue attackRetinue) //当某机甲被拖出进攻时，显示可选目标标记箭头
    {
        foreach (ModuleRetinue targetRetinue in EnemyClientPlayer.MyBattleGroundManager.Retinues)
        {
            if (EnemyClientPlayer.MyBattleGroundManager.HasDefenceRetinue)
            {
                if (attackRetinue.M_Weapon != null && attackRetinue.M_Weapon.M_WeaponType == WeaponTypes.SniperGun && attackRetinue.M_RetinueWeaponEnergy != 0) targetRetinue.ShowTargetPreviewArrow(true);
                else if (targetRetinue.IsDefender) targetRetinue.ShowTargetPreviewArrow();
            }
            else targetRetinue.ShowTargetPreviewArrow();
        }
    }

    public void ShowTargetPreviewArrow(TargetSideEffect.TargetRange targetRange) //当某咒术被拖出进攻时，显示可选目标标记箭头
    {
        switch (targetRange)
        {
            case TargetSideEffect.TargetRange.AllLife:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.Mechs:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.SelfMechs:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetSideEffect.TargetRange.EnemyMechs:
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
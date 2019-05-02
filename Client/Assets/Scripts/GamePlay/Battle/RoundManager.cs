using System.Collections.Generic;
using UnityEngine;

public partial class RoundManager : MonoSingleton<RoundManager>
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

    public PlayMode M_PlayMode;

    public enum PlayMode
    {
        Online,
        Single,
        SingleCustom,
    }

    void Awake()
    {
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
        switch (M_PlayMode)
        {
            case PlayMode.Online:
            {
                TransitManager.Instance.ShowBlackShutTransit(1f, Preparation);
                break;
            }
            case PlayMode.Single:
            {
                TransitManager.Instance.ShowBlackShutTransit(1f, Preparation);
                break;
            }
            case PlayMode.SingleCustom:
            {
                TransitManager.Instance.ShowBlackShutTransit(1f, Preparation);
                break;
            }
        }
    }

    public void Preparation()
    {
        UIManager.Instance.ShowUIForms<GameBoardUIPanel>().SetEndRoundButtonState(false);
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        UIManager.Instance.CloseUIForms<SelectBuildPanel>();
        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/Battle_0", "bgm/Battle_1"}, 0.7f);
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
            EnemyClientPlayer.MyMetalLifeEnergyManager.SetEnemyIconImage();
        }
    }

    public bool InRound = false;

    private void BeginRound()
    {
        InRound = true;
        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    public void EndRound()
    {
        InRound = false;
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    bool isStop = false;

    public void StopGame()
    {
        if (Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            isStop = true; //标记为，待Update的时候正式处理OnGameStop
        }
        else
        {
            isStop = false;
        }
    }

    public bool HasShowLostConnectNotice = true;

    public void OnGameStop()
    {
        if (M_PlayMode == PlayMode.Single)
        {
            TransitManager.Instance.ShowBlackShutTransit(1f, GameStopPreparation);
        }
        else
        {
            GameStopPreparation();
        }
    }

    private void GameStopPreparation()
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
        GameBoardManager.Instance.SelfPlayerCoolDownCardManager.ResetAll();
        GameBoardManager.Instance.EnemyPlayerCoolDownCardManager.ResetAll();

        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;
        RoundNumber = 0;

        UIManager.Instance.CloseUIForms<GameBoardUIPanel>();
        GameBoardManager.Instance.ResetAll();

        CardDeckManager.Instance.HideAll();
        RandomNumberGenerator = null;

        if (Client.Instance.Proxy != null && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        }
        else if (!Client.Instance.IsConnect() && !Client.Instance.IsLogin() && !Client.Instance.IsPlaying())
        {
            UIManager.Instance.CloseUIForms<SelectBuildPanel>();
            UIManager.Instance.ShowUIForms<LoginPanel>();
            if (!HasShowLostConnectNotice)
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_YouAreOffline"), 0, 1f);
                HasShowLostConnectNotice = true;
            }
        }

        switch (M_PlayMode)
        {
            case PlayMode.Online:
            {
                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Online);
                break;
            }
            case PlayMode.Single:
            {
                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Single);
                break;
            }
            case PlayMode.SingleCustom:
            {
                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_SingleCustom);
                break;
            }
        }

        BattleEffectsManager.Instance.ResetAll();
//        TransitManager.Instance.HideTransit(Color.black, 0.1f);

        if (M_PlayMode == PlayMode.Single)
        {
            if (StoryManager.Instance.JustGetSomeCard)
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(
                    LanguageManager.Instance.GetText("RoundManager_JustGotANewCard"),
                    LanguageManager.Instance.GetText("RoundManager_GoToDeck"),
                    LanguageManager.Instance.GetText("RoundManager_GotIt"),
                    delegate
                    {
                        cp.CloseUIForm();
                        UIManager.Instance.ShowUIForms<SelectBuildPanel>();
                    },
                    cp.CloseUIForm);
            }
        }
    }

    #region 交互

    public void OnEndRoundButtonClick()
    {
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            EndRoundRequest request = new EndRoundRequest(Client.Instance.Proxy.ClientId);
            Client.Instance.Proxy.SendMessage(request);
            UIManager.Instance.GetBaseUIForm<GameBoardUIPanel>().SetEndRoundButtonState(false);
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

    public void ShowTargetPreviewArrow(TargetRange targetRange) //当某咒术被拖出进攻时，显示可选目标标记箭头
    {
        switch (targetRange)
        {
            case TargetRange.AllLife:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.Mechs:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfMechs:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemyMechs:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.Heroes:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfHeroes:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemyHeroes:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Heros) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.Soldiers:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfSoldiers:
                foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemySoldiers:
                foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Soldiers) retinue.ShowTargetPreviewArrow();
                break;
        }
    }

    public void HideTargetPreviewArrow()
    {
        if (SelfClientPlayer != null)
            foreach (ModuleRetinue retinue in SelfClientPlayer.MyBattleGroundManager.Retinues)
                retinue.HideTargetPreviewArrow();
        if (EnemyClientPlayer != null)
            foreach (ModuleRetinue retinue in EnemyClientPlayer.MyBattleGroundManager.Retinues)
                retinue.HideTargetPreviewArrow();
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
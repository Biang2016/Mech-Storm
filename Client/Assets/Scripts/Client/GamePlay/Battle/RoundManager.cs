using System.Collections;
using System.Collections.Generic;

public partial class RoundManager : MonoSingleton<RoundManager>
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

        BackGroundManager.Instance.ChangeBoardBG();
        BattleManager.Instance.ShowBattleShips();
    }

    public void Preparation()
    {
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().SetState(SelectBuildPanel.States.ReadOnly);
        UIManager.Instance.CloseUIForm<SelectBuildPanel>();
        UIManager.Instance.CloseUIForm<StoryPanel>();
        UIManager.Instance.CloseUIForm<StoryPlayerInformationPanel>();
        UIManager.Instance.ShowUIForms<ExitMenuPanel>().SetSurrenderButtonShow(true);
        UIManager.Instance.CloseUIForm<ExitMenuPanel>();
        DragManager.Instance.ResetCurrentDrag();
        DragManager.Instance.IsCanceling = false;
        DragManager.Instance.ForbidDrag = false;
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/Battle_0", "bgm/Battle_1"}, volume: 0.7f);
        SelfClientPlayer.BattlePlayer.CardDeckManager.ResetCardDeckNumberText();
        EnemyClientPlayer.BattlePlayer.CardDeckManager.ResetCardDeckNumberText();
    }

    private void InitializePlayers(SetPlayerRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientID)
        {
            SelfClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
            BattleManager.Instance.SelfBattlePlayer = SelfClientPlayer.BattlePlayer;
            SelfClientPlayer.BattlePlayer.MetalLifeEnergyManager.SetEnemyIconImage();
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.username, r.metalLeft, r.metalMax, r.lifeLeft, r.lifeMax, r.energyLeft, r.energyMax, Players.Enemy);
            EnemyClientPlayer.ClientId = r.clientId;
            BattleManager.Instance.EnemyBattlePlayer = EnemyClientPlayer.BattlePlayer;
            EnemyClientPlayer.BattlePlayer.MetalLifeEnergyManager.SetEnemyIconImage();
        }

        if (SelfClientPlayer != null && EnemyClientPlayer != null)
        {
            Preparation();
        }
    }

    public bool InRound = false;

    private void BeginRound()
    {
        InRound = true;
        CurrentClientPlayer.BattlePlayer.HandManager.BeginRound();
        CurrentClientPlayer.BattlePlayer.BattleGroundManager.BeginRound();
    }

    public void EndRound()
    {
        InRound = false;
        CurrentClientPlayer.BattlePlayer.HandManager.EndRound();
        CurrentClientPlayer.BattlePlayer.BattleGroundManager.EndRound();
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
        GameStopPreparation();
    }

    private void GameStopPreparation()
    {
        BattleEffectsManager.Instance.ResetAll();
        BattleManager.Instance.ResetAll();
        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().SetState(SelectBuildPanel.States.Normal);
        GameObjectPoolManager.Instance.OptimizeAllGameObjectPools();

        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        RoundNumber = 0;
        RandomNumberGenerator = null;

        if (Client.Instance.Proxy != null && Client.Instance.Proxy.ClientState == ProxyBase.ClientStates.Playing)
        {
            Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        }
        else if (!NetworkManager.Instance.IsConnect() && !Client.Instance.IsLogin() && !Client.Instance.IsPlaying())
        {
            UIManager.Instance.CloseUIForm<SelectBuildPanel>();
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
                UIManager.Instance.GetBaseUIForm<StartMenuPanel>().SetState(StoryManager.Instance.HasStory ? StartMenuPanel.States.Show_Single_HasStory : StartMenuPanel.States.Show_Single);
                UIManager.Instance.CloseUIForm<StartMenuPanel>();
                UIManager.Instance.ShowUIForms<StoryPanel>();
                break;
            }
            case PlayMode.SingleCustom:
            {
                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_SingleCustom);
                break;
            }
        }

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
            StartCoroutine(Co_OnEndRoundButtonClickSFX());
            EndRoundRequest request = new EndRoundRequest(Client.Instance.Proxy.ClientID);
            Client.Instance.Proxy.SendMessage(request);
            BattleManager.Instance.BattleUIPanel.SetEndRoundButtonState(false);
        }
        else
        {
            ClientLog.Instance.PrintWarning("Not Your Round");
        }
    }

    IEnumerator Co_OnEndRoundButtonClickSFX()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEndRoundButtonClick", 1f);
        yield return null;
    }

    public void ShowMechAttackPreviewArrow(ModuleMech attackMech) //当某机甲被拖出进攻时，显示可选目标标记箭头
    {
        foreach (ModuleMech targetMech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Mechs)
        {
            if (EnemyClientPlayer.BattlePlayer.BattleGroundManager.HasDefenceMech)
            {
                if (attackMech.MechEquipSystemComponent.M_Weapon != null && attackMech.MechEquipSystemComponent.M_Weapon.M_WeaponType == WeaponTypes.SniperGun && attackMech.M_MechWeaponEnergy != 0) targetMech.ShowTargetPreviewArrow(true);
                else if (targetMech.IsDefender) targetMech.ShowTargetPreviewArrow();
            }
            else targetMech.ShowTargetPreviewArrow();
        }
    }

    public void ShowTargetPreviewArrow(TargetRange targetRange) //当某咒术被拖出进攻时，显示可选目标标记箭头
    {
        switch (targetRange)
        {
            case TargetRange.AllLife:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.Mechs:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfMechs:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemyMechs:
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Mechs) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.Heroes:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Heros) mech.ShowTargetPreviewArrow();
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Heros) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfHeroes:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Heros) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemyHeroes:
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Heros) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.Soldiers:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Soldiers) mech.ShowTargetPreviewArrow();
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Soldiers) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.SelfSoldiers:
                foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Soldiers) mech.ShowTargetPreviewArrow();
                break;
            case TargetRange.EnemySoldiers:
                foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Soldiers) mech.ShowTargetPreviewArrow();
                break;
        }
    }

    public void HideTargetPreviewArrow()
    {
        if (SelfClientPlayer != null)
            foreach (ModuleMech mech in SelfClientPlayer.BattlePlayer.BattleGroundManager.Mechs)
                mech.HideTargetPreviewArrow();
        if (EnemyClientPlayer != null)
            foreach (ModuleMech mech in EnemyClientPlayer.BattlePlayer.BattleGroundManager.Mechs)
                mech.HideTargetPreviewArrow();
    }

    #endregion

    #region Utils

    public ClientPlayer GetPlayerByClientId(int clientId)
    {
        if (SelfClientPlayer.ClientId == clientId) return SelfClientPlayer;
        if (EnemyClientPlayer.ClientId == clientId) return EnemyClientPlayer;
        return null;
    }

    private ModuleMech FindMech(int mechId)
    {
        ModuleMech selfMech = SelfClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId);
        if (selfMech) return selfMech;
        ModuleMech enemyMech = EnemyClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId);
        if (enemyMech) return enemyMech;
        return null;
    }

    #endregion
}
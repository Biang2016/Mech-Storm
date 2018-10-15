using System.Collections.Generic;

internal class ServerPlayer : Player
{
    public ClientProxy MyClientProxy;
    public ServerPlayer MyEnemyPlayer;

    public int ClientId;
    public int EnemyClientId;
    public ServerGameManager MyGameManager;
    public ServerHandManager MyHandManager;
    public ServerCardDeckManager MyCardDeckManager;
    public ServerBattleGroundManager MyBattleGroundManager;

    public ServerPlayer(string username, int clientId, int enemyClientId, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax, ServerGameManager serverGameManager) : base(username, metalLeft, metalMax, lifeLeft, lifeMax, energyLeft, energyMax)
    {
        ClientId = clientId;
        EnemyClientId = enemyClientId;
        MyGameManager = serverGameManager;
        MyHandManager = new ServerHandManager(this);
        MyCardDeckManager = new ServerCardDeckManager(this);
        MyBattleGroundManager = new ServerBattleGroundManager(this);
    }

    public void OnDestroyed()
    {
        MyEnemyPlayer = null;
        MyGameManager = null;
        MyHandManager = null;
        MyCardDeckManager = null;
        MyBattleGroundManager = null;
    }

    #region MetalChange

    public void AddMetalWithoutLimit(int addMetalValue)
    {
        AddMetal(addMetalValue);
        if (addMetalValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Left, addMetal_left: addMetalValue);
            BroadCastRequest(request);
        }
    }

    public void AddMetalWithinMax(int addMetalValue)
    {
        int metalLeftBefore = MetalLeft;
        if (MetalMax - MetalLeft > addMetalValue)
            AddMetal(addMetalValue);
        else
            AddMetal(MetalMax - MetalLeft);
        if (addMetalValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Left, addMetal_left: MetalLeft - metalLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseMetalAboveZero(int useMetalValue)
    {
        int metalLeftBefore = MetalLeft;
        if (MetalLeft > useMetalValue)
            AddMetal(-useMetalValue);
        else
            AddMetal(-MetalLeft);
        if (useMetalValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Left, addMetal_left: MetalLeft - metalLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void AddAllMetal()
    {
        int metalLeftBefore = MetalLeft;
        AddMetal(MetalMax - MetalLeft);
        if (MetalLeft - metalLeftBefore != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Left, addMetal_left: MetalLeft - metalLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseAllMetal()
    {
        int metalLeftBefore = MetalLeft;
        AddMetal(-MetalLeft);
        if (MetalLeft - metalLeftBefore != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Left, addMetal_left: MetalLeft - metalLeftBefore);
            BroadCastRequest(request);
        }
    }


    public void IncreaseMetalMax(int increaseValue)
    {
        int metalMaxBefore = MetalMax;
        if (MetalMax + increaseValue <= GamePlaySettings.MaxMetal)
            AddMetalMax(increaseValue);
        else
            AddMetalMax(GamePlaySettings.MaxMetal - MetalMax);
        if (increaseValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Max, 0, addMetal_max: MetalMax - metalMaxBefore);
            BroadCastRequest(request);
        }
    }

    public void DecreaseMetalMax(int decreaseValue)
    {
        int metalMaxBefore = MetalMax;
        if (MetalMax <= decreaseValue)
            AddMetalMax(-MetalMax);
        else
            AddMetalMax(-decreaseValue);
        if (decreaseValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, PlayerMetalChangeRequest.MetalChangeFlag.Max, 0, addMetal_max: MetalMax - metalMaxBefore);
            BroadCastRequest(request);
        }
    }

    #endregion

    #region LifeChange

    public void AddLifeWithinMax(int addLifeValue)
    {
        int LifeLeftBefore = LifeLeft;
        if (LifeMax - LifeLeft > addLifeValue)
            AddLife(addLifeValue);
        else
            AddLife(LifeMax - LifeLeft);
        if (addLifeValue != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerAddLife, new SideEffectBase.ExecuterInfo(ClientId, value: addLifeValue));
        }
    }

    public void DamageLifeAboveZero(int useLifeValue)
    {
        int LifeLeftBefore = LifeLeft;
        if (LifeLeft > useLifeValue)
            AddLife(-useLifeValue);
        else
            AddLife(-LifeLeft);
        if (useLifeValue != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerLostLife, new SideEffectBase.ExecuterInfo(ClientId, value: useLifeValue));
        }

        if (LifeLeft <= 0)
        {
            MyGameManager.OnEndGame(MyEnemyPlayer);
        }
    }

    public void AddAllLife()
    {
        int LifeLeftBefore = LifeLeft;
        AddLife(LifeMax - LifeLeft);
        if (LifeLeft - LifeLeftBefore != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerAddLife, new SideEffectBase.ExecuterInfo(ClientId, value: LifeLeft - LifeLeftBefore));
        }
    }

    #endregion

    #region EnergyChange

    public void AddEnergyWithinMax(int addEnergyValue)
    {
        int EnergyLeftBefore = EnergyLeft;
        if (EnergyMax - EnergyLeft > addEnergyValue)
            AddEnergy(addEnergyValue);
        else
            AddEnergy(EnergyMax - EnergyLeft);
        if (addEnergyValue != 0)
        {
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, PlayerEnergyChangeRequest.EnergyChangeFlag.Left, addEnergy_left: EnergyLeft - EnergyLeftBefore);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerGetEnergy, new SideEffectBase.ExecuterInfo(ClientId, value: addEnergyValue));
        }
    }

    public void UseEnergyAboveZero(int useEnergyValue)
    {
        int EnergyLeftBefore = EnergyLeft;
        if (EnergyLeft > useEnergyValue)
            AddEnergy(-useEnergyValue);
        else
            AddEnergy(-EnergyLeft);
        if (useEnergyValue != 0)
        {
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, PlayerEnergyChangeRequest.EnergyChangeFlag.Left, addEnergy_left: EnergyLeft - EnergyLeftBefore);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerUseEnergy, new SideEffectBase.ExecuterInfo(ClientId, value: useEnergyValue));
        }
    }

    public void AddAllEnergy()
    {
        int EnergyLeftBefore = EnergyLeft;
        AddEnergy(EnergyMax - EnergyLeft);
        if (EnergyLeft - EnergyLeftBefore != 0)
        {
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, PlayerEnergyChangeRequest.EnergyChangeFlag.Left, addEnergy_left: EnergyLeft - EnergyLeftBefore);
            BroadCastRequest(request);
        }
    }

    #endregion

    public void OnCardDeckLeftChange(int count)
    {
        CardDeckLeftChangeRequest request = new CardDeckLeftChangeRequest(ClientId, count);
        BroadCastRequest(request);
    }

    #region SideEffectsAttachedToPlayer

    private Dictionary<int, SideEffectExecute> SideEffectBundles_Player = new Dictionary<int, SideEffectExecute>();

    public void AddSideEffectBundleForPlayerBuff(SideEffectExecute see)
    {
        SideEffectBundles_Player.Add(see.ID, see);
        MyGameManager.EventManager.RegisterEvent(see);
        PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, see.ID, see.RemoveTriggerTimes);
        BroadCastRequest(request);
    }

    public void ReduceSideEffectBundleForPlayerBuff(SideEffectExecute see)
    {
        if (SideEffectBundles_Player.ContainsKey(see.ID))
        {
            if (see.RemoveTriggerTimes == 0) //等于0清除buff
            {
                SideEffectBundles_Player.Remove(see.ID);
            }

            //通知客户端改变buff数字
            PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, see.ID, see.RemoveTriggerTimes);
            BroadCastRequest(request);
        }
    }

    #endregion


    private void BroadCastRequest(ServerRequestBase request)
    {
        MyClientProxy?.CurrentClientRequestResponseBundle?.AttachedRequests.Add(request);
        MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle?.AttachedRequests.Add(request);
    }
}
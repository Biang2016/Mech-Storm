using System.Collections.Generic;
using System.Linq;

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
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
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
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);
        }
    }

    public void UseMetalAboveZero(int useMetalValue)
    {
        int metalLeftBefore = MetalLeft;
        if (MetalLeft > useMetalValue)
        {
            AddMetal(-useMetalValue);
        }
        else
        {
            AddMetal(-MetalLeft);
        }

        if (useMetalValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);

            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnUseMetal, new SideEffectBase.ExecutorInfo(clientId: ClientId, value: useMetalValue));
        }
    }

    public void AddAllMetal()
    {
        int metalLeftBefore = MetalLeft;
        AddMetal(MetalMax - MetalLeft);
        if (MetalLeft - metalLeftBefore != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);
        }
    }

    public void UseAllMetal()
    {
        int metalLeftBefore = MetalLeft;
        AddMetal(-MetalLeft);
        if (MetalLeft - metalLeftBefore != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);
        }
    }

    public void IncreaseMetalMax(int increaseValue)
    {
        if (MetalMax + increaseValue <= GamePlaySettings.MaxMetal)
            AddMetalMax(increaseValue);
        else
            AddMetalMax(GamePlaySettings.MaxMetal - MetalMax);
        if (increaseValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);
        }
    }

    public void DecreaseMetalMax(int decreaseValue)
    {
        if (MetalMax <= decreaseValue)
            AddMetalMax(-MetalMax);
        else
            AddMetalMax(-decreaseValue);
        if (decreaseValue != 0)
        {
            PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
            BroadCastRequest(request);
        }
    }

    protected override void OnMetalChanged(int change)
    {
        base.OnMetalChanged(change);
        MyHandManager.RefreshAllCardUsable();
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
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, LifeLeft, LifeMax);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerAddLife, new SideEffectBase.ExecutorInfo(ClientId, value: addLifeValue));
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
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, LifeLeft, LifeMax);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerLostLife, new SideEffectBase.ExecutorInfo(ClientId, value: useLifeValue));
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
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, LifeLeft, LifeMax);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerAddLife, new SideEffectBase.ExecutorInfo(ClientId, value: LifeLeft - LifeLeftBefore));
        }
    }

    #endregion

    #region EnergyChange

    protected override void OnEnergyChanged(int change)
    {
        base.OnEnergyChanged(change);
        MyHandManager.RefreshAllCardUsable();
    }

    public void AddEnergyWithinMax(int addEnergyValue)
    {
        int EnergyLeftBefore = EnergyLeft;
        bool isOverflow = false;
        if (EnergyMax - EnergyLeft > addEnergyValue)
            AddEnergy(addEnergyValue);
        else
        {
            AddEnergy(EnergyMax - EnergyLeft);
            isOverflow = true;
        }

        if (addEnergyValue != 0)
        {
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, EnergyLeft, EnergyMax, isOverflow);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerGetEnergy, new SideEffectBase.ExecutorInfo(ClientId, value: addEnergyValue));
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
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, EnergyLeft, EnergyMax);
            BroadCastRequest(request);
            MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayerUseEnergy, new SideEffectBase.ExecutorInfo(ClientId, value: useEnergyValue));
        }
    }

    public void AddAllEnergy()
    {
        int EnergyLeftBefore = EnergyLeft;
        AddEnergy(EnergyMax - EnergyLeft);
        if (EnergyLeft - EnergyLeftBefore != 0)
        {
            PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, EnergyLeft, EnergyMax);
            BroadCastRequest(request);
        }
    }

    #endregion

    public void OnCardDeckLeftChange(int count)
    {
        CardDeckLeftChangeRequest request = new CardDeckLeftChangeRequest(ClientId, count);
        BroadCastRequest(request);
    }

    public bool CheckModuleRetinueCanAttackMe(ServerModuleRetinue attackRetinue)
    {
        if (attackRetinue.M_Weapon != null)
        {
            switch (attackRetinue.M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (MyBattleGroundManager.BattleGroundIsEmpty) return true;
                    return false;
                case WeaponTypes.Gun:
                    if (attackRetinue.M_RetinueWeaponEnergy != 0)
                    {
                        if (MyBattleGroundManager.HasDefenseRetinue) return false;
                        return true;
                    }
                    else
                    {
                        if (MyBattleGroundManager.BattleGroundIsEmpty) return true;
                        return false;
                    }
                case WeaponTypes.SniperGun:
                    if (attackRetinue.M_RetinueWeaponEnergy != 0) return true;
                    else
                    {
                        if (MyBattleGroundManager.BattleGroundIsEmpty) return true;
                        return false;
                    }
            }
        }
        else
        {
            if (MyBattleGroundManager.BattleGroundIsEmpty) return true;
        }

        return false;
    }

    #region PlayerBuffs

    private Dictionary<string, Dictionary<int, SideEffectExecute>> SideEffectBundles_Player = new Dictionary<string, Dictionary<int, SideEffectExecute>>();

    public void PlayerBuffTrigger(int seeID, PlayerBuffSideEffects buff)
    {
        if (SideEffectBundles_Player.ContainsKey(buff.Name))
        {
            Dictionary<int, SideEffectExecute> sees = SideEffectBundles_Player[buff.Name];
            if (sees.ContainsKey(seeID))
            {
                SideEffectExecute see = sees[seeID];
                PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, seeID, see);
                BroadCastRequest(request);
            }
        }
    }

    public void UpdatePlayerBuff(SideEffectExecute newSee, bool isAdd = false)
    {
        if (newSee.SideEffectBase is PlayerBuffSideEffects buff)
        {
            if (SideEffectBundles_Player.ContainsKey(buff.Name))
            {
                Dictionary<int, SideEffectExecute> sees = SideEffectBundles_Player[buff.Name];
                if (sees.Count != 0) //存在该buff
                {
                    if (buff.Singleton) //buff是单例，只能存在一个
                    {
                        if (sees.Count > 1) //多于一个，清空重来
                        {
                            int RemainRemoveTriggerTime = 0;
                            foreach (KeyValuePair<int, SideEffectExecute> kv in sees)
                            {
                                RemainRemoveTriggerTime = kv.Value.RemoveTriggerTimes;
                            }

                            ClearSEEByBuffName(buff.Name, sees);

                            if (buff.CanPiled) //可以堆叠
                            {
                                newSee.RemoveTriggerTimes = RemainRemoveTriggerTime;
                            }

                            CreateNewBuff(newSee, buff, sees);
                        }
                        else
                        {
                            SideEffectExecute see = sees.Values.ToList()[0];
                            if (isAdd)
                            {
                                PileBuff(see, newSee);
                            }
                            else
                            {
                                see.RemoveTriggerTimes = newSee.RemoveTriggerTimes;
                            }

                            PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, see.ID, see);
                            BroadCastRequest(request);
                        }
                    }
                    else //buff不是单例，则对应加到seeID上去
                    {
                        if (sees.ContainsKey(newSee.ID))
                        {
                            SideEffectExecute see = sees[newSee.ID];
                            if (isAdd)
                            {
                                PileBuff(see, newSee);
                            }
                            else
                            {
                                see.RemoveTriggerTimes = newSee.RemoveTriggerTimes;
                            }

                            PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, see.ID, see);
                            BroadCastRequest(request);
                        }
                        else //ID不存在场上，新建一个buff
                        {
                            CreateNewBuff(newSee, buff, sees);
                        }
                    }
                }
                else //不存在该buff
                {
                    CreateNewBuff(newSee, buff, sees);
                }
            }
            else
            {
                Dictionary<int, SideEffectExecute> sees = new Dictionary<int, SideEffectExecute>();
                SideEffectBundles_Player.Add(buff.Name, sees);
                CreateNewBuff(newSee, buff, sees);
            }
        }
    }

    private static void PileBuff(SideEffectExecute see, SideEffectExecute newSee)
    {
        PlayerBuffSideEffects buff = (PlayerBuffSideEffects) newSee.SideEffectBase;
        if (buff.CanPiled) //可以堆叠
        {
            if (buff.PiledBy == PlayerBuffSideEffects.BuffPiledBy.RemoveTriggerTimes)
            {
                see.RemoveTriggerTimes += newSee.RemoveTriggerTimes;
            }
            else if (buff.PiledBy == PlayerBuffSideEffects.BuffPiledBy.Value)
            {
                foreach (SideEffectBase ori_buff in see.SideEffectBase.Sub_SideEffect)
                {
                    foreach (SideEffectBase add_buff in buff.Sub_SideEffect)
                    {
                        if (ori_buff.GetType() == add_buff.GetType()) //同类buff同类值叠加
                        {
                            if (ori_buff is IEffectFactor ie_ori && add_buff is IEffectFactor ie_add)
                            {
                                for (int i = 0; i < ie_ori.Values.Count; i++)
                                {
                                    ie_ori.Values[i].Value += ie_add.Values[i].Value;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void RemovePlayerBuff(SideEffectExecute newSee, PlayerBuffSideEffects buff)
    {
        if (SideEffectBundles_Player.ContainsKey(buff.Name))
        {
            Dictionary<int, SideEffectExecute> sees = SideEffectBundles_Player[buff.Name];
            if (sees.Count != 0) //存在该buff
            {
                if (buff.Singleton) //buff是单例，全删
                {
                    ClearSEEByBuffName(buff.Name, sees);
                }
                else //buff不是单例，则对应到seeID上去
                {
                    if (sees.ContainsKey(newSee.ID))
                    {
                        SideEffectExecute see = sees[newSee.ID];
                        RemoveSEE(buff.Name, sees, see);
                    }
                }
            }
        }
    }

    private void ClearSEEByBuffName(string buffName, Dictionary<int, SideEffectExecute> sees)
    {
        List<SideEffectExecute> remove_sees = new List<SideEffectExecute>();
        foreach (KeyValuePair<int, SideEffectExecute> kv in sees)
        {
            remove_sees.Add(kv.Value);
        }

        foreach (SideEffectExecute remove_see in remove_sees)
        {
            RemoveSEE(buffName, sees, remove_see);
        }
    }

    private void RemoveSEE(string buffName, Dictionary<int, SideEffectExecute> sees, SideEffectExecute remove_see)
    {
        sees.Remove(remove_see.ID);
        MyGameManager.EventManager.UnRegisterEvent(remove_see);
        PlayerBuffRemoveRequest request = new PlayerBuffRemoveRequest(ClientId, remove_see.ID, buffName);
        BroadCastRequest(request);
    }

    private void CreateNewBuff(SideEffectExecute newSee, PlayerBuffSideEffects buff, Dictionary<int, SideEffectExecute> sees)
    {
        sees.Add(newSee.ID, newSee);
        MyGameManager.EventManager.RegisterEvent(newSee);
        PlayerBuffUpdateRequest request = new PlayerBuffUpdateRequest(ClientId, newSee.ID, newSee);
        BroadCastRequest(request);
    }

    #endregion

    private void BroadCastRequest(ServerRequestBase request)
    {
        MyClientProxy?.CurrentClientRequestResponseBundle?.AttachedRequests.Add(request);
        MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle?.AttachedRequests.Add(request);
    }
}
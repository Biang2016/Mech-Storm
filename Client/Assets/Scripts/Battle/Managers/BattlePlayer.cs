using System.Collections.Generic;
using System.Linq;

internal class BattlePlayer : Player
{
    public BattleClientProxy MyClientProxy;
    public BattlePlayer MyEnemyPlayer;

    public int ClientId;
    public GameManager GameManager;
    public Battle_HandManager HandManager;
    public CardDeckManager CardDeckManager;
    public BattleGroundManager BattleGroundManager;

    public BattlePlayer(string username, int clientId, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax, GameManager serverGameManager) : base(username, metalLeft, metalMax, lifeLeft, lifeMax, energyLeft, energyMax)
    {
        ClientId = clientId;
        GameManager = serverGameManager;
        HandManager = new Battle_HandManager(this);
        CardDeckManager = new CardDeckManager(this);
        BattleGroundManager = new BattleGroundManager(this);
    }

    public void OnDestroyed()
    {
        MyEnemyPlayer = null;
        GameManager = null;
        HandManager = null;
        CardDeckManager = null;
        BattleGroundManager = null;
    }

    #region MetalChange

    protected override void OnMetalChanged(int change)
    {
        base.OnMaxMetalChanged(change);
        HandManager.RefreshAllCardUsable();
        PlayerMetalChangeRequest request = new PlayerMetalChangeRequest(ClientId, MetalLeft, MetalMax);
        BroadCastRequest(request);
    }

    protected override void OnMetalIncrease(int change)
    {
        base.OnMetalIncrease(change);
    }

    protected override void OnMetalUsed(int change)
    {
        base.OnMetalUsed(change);
        GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnUseMetal, new ExecutorInfo(clientId: ClientId, value: change));
    }

    protected override void OnMetalReduce(int change)
    {
        base.OnMetalReduce(change);
    }

    #endregion

    #region LifeChange

    protected override void OnLifeChanged(int change, bool isOverflow)
    {
        base.OnLifeChanged(change, isOverflow);
        PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, LifeLeft, LifeMax);
        BroadCastRequest(request);
        if (LifeLeft <= 0)
        {
            GameManager.OnEndGame(MyEnemyPlayer);
        }
    }

    protected override void OnHeal(int change, bool isOverflow)
    {
        base.OnHeal(change, isOverflow);
        GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnPlayerAddLife, new ExecutorInfo(ClientId, value: change));
    }

    protected override void OnDamage(int change)
    {
        base.OnDamage(change);
        GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnPlayerLostLife, new ExecutorInfo(ClientId, value: change));
    }

    protected override void OnMaxLifeChanged(int change)
    {
        base.OnMaxLifeChanged(change);
    }

    protected override void OnMaxLifeIncrease(int change)
    {
        base.OnMaxLifeIncrease(change);
    }

    protected override void OnMaxLifeReduce(int change)
    {
        base.OnMaxLifeReduce(change);
    }

    #endregion

    #region EnergyChange

    protected override void OnEnergyChanged(int change, bool isOverflow)
    {
        base.OnEnergyChanged(change, isOverflow);
        HandManager.RefreshAllCardUsable();
        PlayerEnergyChangeRequest request = new PlayerEnergyChangeRequest(ClientId, EnergyLeft, EnergyMax, isOverflow);
        BroadCastRequest(request);
    }

    protected override void OnEnergyIncrease(int change, bool isOverflow)
    {
        base.OnEnergyIncrease(change, isOverflow);
        GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnPlayerGetEnergy, new ExecutorInfo(ClientId, value: change));
    }

    protected override void OnEnergyReduce(int change)
    {
        base.OnEnergyReduce(change);
    }

    protected override void OnEnergyUsed(int change)
    {
        base.OnEnergyUsed(change);
        GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnPlayerUseEnergy, new ExecutorInfo(ClientId, value: change));
    }

    #endregion

    public void OnCardDeckLeftChange(int count)
    {
        CardDeckLeftChangeRequest request = new CardDeckLeftChangeRequest(ClientId, count);
        BroadCastRequest(request);
    }

    public bool CheckModuleMechCanAttackMe(ModuleMech attackMech)
    {
        if (attackMech.M_Weapon != null)
        {
            switch (attackMech.M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (BattleGroundManager.BattleGroundIsEmpty) return true;
                    return false;
                case WeaponTypes.Gun:
                    if (attackMech.M_MechWeaponEnergy != 0)
                    {
                        if (BattleGroundManager.HasDefenseMech) return false;
                        return true;
                    }
                    else
                    {
                        if (BattleGroundManager.BattleGroundIsEmpty) return true;
                        return false;
                    }
                case WeaponTypes.SniperGun:
                    if (attackMech.M_MechWeaponEnergy != 0) return true;
                    else
                    {
                        if (BattleGroundManager.BattleGroundIsEmpty) return true;
                        return false;
                    }
            }
        }
        else
        {
            if (BattleGroundManager.BattleGroundIsEmpty) return true;
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
        foreach (SideEffectBase se in newSee.SideEffectBases)
        {
            if (se is PlayerBuffSideEffects buff)
            {
                if (SideEffectBundles_Player.ContainsKey(buff.Name))
                {
                    Dictionary<int, SideEffectExecute> sees = SideEffectBundles_Player[buff.Name];
                    if (sees.Count != 0) //存在该buff
                    {
                        if (buff.M_SideEffectParam.GetParam_Bool("Singleton")) //buff是单例，只能存在一个
                        {
                            if (sees.Count > 1) //多于一个，清空重来
                            {
                                int RemainRemoveTriggerTime = 0;
                                foreach (KeyValuePair<int, SideEffectExecute> kv in sees)
                                {
                                    RemainRemoveTriggerTime = kv.Value.M_ExecuteSetting.RemoveTriggerTimes;
                                }

                                ClearSEEByBuffName(buff.Name, sees);

                                if (buff.M_SideEffectParam.GetParam_Bool("CanPiled")) //可以堆叠
                                {
                                    newSee.M_ExecuteSetting.RemoveTriggerTimes = RemainRemoveTriggerTime;
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
                                    see.M_ExecuteSetting.RemoveTriggerTimes = newSee.M_ExecuteSetting.RemoveTriggerTimes;
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
                                    see.M_ExecuteSetting.RemoveTriggerTimes = newSee.M_ExecuteSetting.RemoveTriggerTimes;
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
    }

    private static void PileBuff(SideEffectExecute see, SideEffectExecute newSee)
    {
        PlayerBuffSideEffects buff = (PlayerBuffSideEffects) newSee.SideEffectBases[0];
        if (buff.M_SideEffectParam.GetParam_Bool("CanPiled")) //可以堆叠
        {
            if ((PlayerBuffSideEffects.BuffPiledBy) buff.M_SideEffectParam.GetParam_ConstInt("PiledBy") == PlayerBuffSideEffects.BuffPiledBy.RemoveTriggerTimes)
            {
                see.M_ExecuteSetting.RemoveTriggerTimes += newSee.M_ExecuteSetting.RemoveTriggerTimes;
            }
            else if ((PlayerBuffSideEffects.BuffPiledBy) buff.M_SideEffectParam.GetParam_ConstInt("PiledBy") == PlayerBuffSideEffects.BuffPiledBy.Value)
            {
                foreach (SideEffectBase ori_buff in see.SideEffectBases[0].Sub_SideEffect)
                {
                    foreach (SideEffectBase add_buff in buff.Sub_SideEffect)
                    {
                        if (ori_buff.GetType() == add_buff.GetType()) //同类buff同类值叠加,并将倍率重置为1
                        {
                            ori_buff.M_SideEffectParam.Plus(add_buff.M_SideEffectParam);
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
                if (buff.M_SideEffectParam.GetParam_Bool("Singleton")) //buff是单例，全删
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
        GameManager.EventManager.UnRegisterEvent(remove_see);
        PlayerBuffRemoveRequest request = new PlayerBuffRemoveRequest(ClientId, remove_see.ID, buffName);
        BroadCastRequest(request);
    }

    private void CreateNewBuff(SideEffectExecute newSee, PlayerBuffSideEffects buff, Dictionary<int, SideEffectExecute> sees)
    {
        sees.Add(newSee.ID, newSee);
        GameManager.EventManager.RegisterEvent(newSee);
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
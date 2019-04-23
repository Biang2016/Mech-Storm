using System;
using System.Collections.Generic;

internal class ServerBattleGroundManager
{
    private int retinueCount;

    public int RetinueCount
    {
        get { return retinueCount; }
        set { retinueCount = value; }
    }

    private int heroCount;

    public int HeroCount
    {
        get { return heroCount; }
        set { heroCount = value; }
    }

    private int soldierCount;

    public int SoldierCount
    {
        get { return soldierCount; }
        set { soldierCount = value; }
    }

    public bool BattleGroundIsFull
    {
        get { return RetinueCount == GamePlaySettings.MaxRetinueNumber; }
    }

    public bool BattleGroundIsEmpty
    {
        get { return RetinueCount == 0; }
    }

    public bool HerosIsEmpty
    {
        get { return HeroCount == 0; }
    }

    public bool SoldiersIsEmpty
    {
        get { return SoldierCount == 0; }
    }

    public bool HasDefenseRetinue
    {
        get
        {
            foreach (ServerModuleRetinue retinue in Retinues)
            {
                if (retinue.IsDefender) return true;
            }

            return false;
        }
    }

    public ServerPlayer ServerPlayer;
    public List<ServerModuleRetinue> Retinues = new List<ServerModuleRetinue>();
    public List<ServerModuleRetinue> Heroes = new List<ServerModuleRetinue>();
    public List<ServerModuleRetinue> Soldiers = new List<ServerModuleRetinue>();
    public HashSet<int> CanAttackRetinues = new HashSet<int>();

    public ServerBattleGroundManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    #region SideEffects

    private void BattleGroundAddRetinue(int retinuePlaceIndex, ServerModuleRetinue retinue)
    {
        int aliveIndex = GetIndexOfAliveRetinues(retinuePlaceIndex);
        Retinues.Insert(aliveIndex, retinue);
        RetinueCount = Retinues.Count;
        if (retinue.CardInfo.RetinueInfo.IsSoldier)
        {
            Soldiers.Add(retinue);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heroes.Add(retinue);
            HeroCount = Heroes.Count;
        }

        PrintRetinueInfos();
    }

    public void RemoveRetinues(List<int> retinueIds)
    {
        foreach (int retinueId in retinueIds)
        {
            ServerModuleRetinue retinue = GetRetinue(retinueId);
            if (retinue != null) RemoveRetinue(retinue);
        }
    }

    private void RemoveRetinue(ServerModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1)
        {
            ServerLog.PrintWarning("BattleGroundRemoveRetinue not exist retinue：" + retinue.M_RetinueID);

            return;
        }

        Retinues.Remove(retinue);
        RetinueCount = Retinues.Count;
        if (retinue.CardInfo.RetinueInfo.IsSoldier)
        {
            Soldiers.Remove(retinue);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heroes.Remove(retinue);
            HeroCount = Heroes.Count;
        }

        if (!retinue.CardInfo.BaseInfo.IsTemp) ServerPlayer.MyCardDeckManager.CardDeck.RecycleCardInstanceID(retinue.OriginCardInstanceId);
        retinue.UnRegisterSideEffect();
        PrintRetinueInfos();
    }

    private void BattleGroundRemoveAllRetinue()
    {
        foreach (ServerModuleRetinue retinue in Retinues.ToArray())
        {
            RemoveRetinue(retinue);
        }
    }

    public void AddRetinue(CardInfo_Retinue retinueCardInfo)
    {
        AddRetinue(retinueCardInfo, Retinues.Count, targetRetinueId: Const.TARGET_RETINUE_SELECT_NONE, clientRetinueTempId: Const.CLIENT_TEMP_RETINUE_ID_NORMAL, handCardInstanceId: Const.CARD_INSTANCE_ID_NONE);
    }

    public void AddRetinue(CardInfo_Retinue retinueCardInfo, int retinuePlaceIndex, int targetRetinueId, int clientRetinueTempId, int handCardInstanceId)
    {
        if (BattleGroundIsFull) return;
        int retinueId = ServerPlayer.MyGameManager.GenerateNewRetinueId();
        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex, retinueId, clientRetinueTempId);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        ServerModuleRetinue retinue = new ServerModuleRetinue();
        retinue.M_RetinueID = retinueId;
        retinue.M_UsedClientRetinueTempId = clientRetinueTempId;
        retinue.OriginCardInstanceId = handCardInstanceId;
        retinue.Initiate(retinueCardInfo, ServerPlayer);

        ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(retinueCardInfo.CardID, handCardInstanceId);

        BattleGroundAddRetinue(retinuePlaceIndex, retinue);

        ExecutorInfo info = new ExecutorInfo(clientId: ServerPlayer.ClientId, retinueId: retinueId, targetRetinueIds: new List<int> {targetRetinueId});
        if (retinueCardInfo.RetinueInfo.IsSoldier) ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierSummon, info);
        else ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroSummon, info);
    }

    public void EquipWeapon(EquipWeaponRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleWeapon weapon = new ServerModuleWeapon();
        CardInfo_Equip cardInfo_Weapon = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueId);
        weapon.M_ModuleRetinue = retinue;
        weapon.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        weapon.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Weapon = weapon;
        ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipShield(EquipShieldRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Equip cardInfo_Shield = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        shield.M_ModuleRetinue = retinue;
        shield.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        shield.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Shield = shield;
        ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipPack(EquipPackRequest r, CardInfo_Base cardInfo)
    {
        ServerModulePack pack = new ServerModulePack();
        CardInfo_Equip cardInfo_Pack = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        pack.M_ModuleRetinue = retinue;
        pack.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        pack.Initiate(cardInfo_Pack, ServerPlayer);
        pack.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Pack = pack;
        ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipMA(EquipMARequest r, CardInfo_Base cardInfo)
    {
        ServerModuleMA ma = new ServerModuleMA();
        CardInfo_Equip cardInfo_MA = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        ma.M_ModuleRetinue = retinue;
        ma.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        ma.Initiate(cardInfo_MA, ServerPlayer);
        ma.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_MA = ma;
        ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void KillAllRetinues()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues.ToArray())
        {
            serverModuleRetinue.OnDieTogether();
        }
    }

    public void KillAllHeroes()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (!Retinues[i].CardInfo.RetinueInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues.ToArray())
        {
            serverModuleRetinue.OnDieTogether();
        }
    }

    public void KillAllSoldiers()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (Retinues[i].CardInfo.RetinueInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues.ToArray())
        {
            serverModuleRetinue.OnDieTogether();
        }
    }

    private void KillOneRetinue(ServerModuleRetinue retinue)
    {
        if (retinue != null)
        {
            retinue.OnDieTogether();
            PrintRetinueInfos();
        }
    }

    public enum RetinueValueTypes
    {
        Life,
        Heal,
        Damage,
        Attack,
        Armor,
        Shield,
        WeaponEnergy,
    }

    private Dictionary<RetinueValueTypes, Action<ServerModuleRetinue, int>> RetinueValueChangeDelegates = new Dictionary<RetinueValueTypes, Action<ServerModuleRetinue, int>>
    {
        {RetinueValueTypes.Life, delegate(ServerModuleRetinue retinue, int value) { retinue?.AddLife(value); }},
        {RetinueValueTypes.Heal, delegate(ServerModuleRetinue retinue, int value) { retinue?.Heal(value); }},
        {
            RetinueValueTypes.Damage, delegate(ServerModuleRetinue targetRetinue, int value)
            {
                targetRetinue.BeAttacked(value);
                targetRetinue.CheckAlive();
            }
        },
        {RetinueValueTypes.Attack, delegate(ServerModuleRetinue retinue, int value) { retinue.M_RetinueAttack += value; }},
        {RetinueValueTypes.Armor, delegate(ServerModuleRetinue retinue, int value) { retinue.M_RetinueArmor += value; }},
        {RetinueValueTypes.Shield, delegate(ServerModuleRetinue retinue, int value) { retinue.M_RetinueShield += value; }},
        {RetinueValueTypes.WeaponEnergy, delegate(ServerModuleRetinue retinue, int value) { retinue.M_RetinueWeaponEnergy += value; }},
    };

    public void ChangeRetinuesValue(RetinueValueTypes retinueValueType, int value, int count, List<int> retinueIds, TargetSelect targetSelect, RetinueType retinueType, int exceptRetinueId = -1)
    {
        Action<ServerModuleRetinue, int> action = RetinueValueChangeDelegates[retinueValueType];
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerModuleRetinue retinue in GetRetinueByType(retinueType).ToArray())
                {
                    action(retinue, value);
                }

                break;
            }
            case TargetSelect.Multiple:
            {
                foreach (int retinueId in retinueIds)
                {
                    action(GetRetinue(retinueId), value);
                }

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                foreach (ServerModuleRetinue retinue in Utils.GetRandomFromList(GetRetinueByType(retinueType), count))
                {
                    action(retinue, value);
                }

                break;
            }
            case TargetSelect.Single:
            {
                action(GetRetinue(retinueIds[0]), value);
                break;
            }
            case TargetSelect.SingleRandom:
            {
                action(GetRandomRetinue(retinueType, exceptRetinueId), value);
                break;
            }
        }
    }

    public void RemoveEquip(RetinueType retinueType, int equipID)
    {
        List<ServerModuleRetinue> retinues = GetRetinueByType(retinueType);

        foreach (ServerModuleRetinue retinue in retinues)
        {
            if (retinue.M_Weapon != null && retinue.M_Weapon.M_EquipID == equipID)
            {
                retinue.M_Weapon = null;
            }

            if (retinue.M_Shield != null && retinue.M_Shield.M_EquipID == equipID)
            {
                retinue.M_Shield = null;
            }

            if (retinue.M_Pack != null && retinue.M_Pack.M_EquipID == equipID)
            {
                retinue.M_Pack = null;
            }

            if (retinue.M_MA != null && retinue.M_MA.M_EquipID == equipID)
            {
                retinue.M_MA = null;
            }
        }
    }

    #region Utils

    public ServerModuleRetinue GetRetinue(int retinueId)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            if (serverModuleRetinue.M_RetinueID == retinueId) return serverModuleRetinue;
        }

        return null;
    }

    private int GetIndexOfAliveRetinues(int battleGroundIndex)
    {
        //去除掉已经死亡但还没移除战场的随从（避免服务器指针错误）
        int countDieRetinue = 0;
        for (int i = 0; i < battleGroundIndex; i++)
        {
            if (ServerPlayer.MyGameManager.DieRetinueList.Contains(Retinues[i].M_RetinueID))
            {
                countDieRetinue++;
            }
        }

        int aliveIndex = battleGroundIndex - countDieRetinue;
        return aliveIndex;
    }

    public int GetRetinueIdByClientRetinueTempId(int clientRetinueTempId)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            if (serverModuleRetinue.M_UsedClientRetinueTempId == clientRetinueTempId)
            {
                return serverModuleRetinue.M_RetinueID;
            }
        }

        return -1;
    }

    private List<ServerModuleRetinue> GetRetinueByType(RetinueType retinueType)
    {
        List<ServerModuleRetinue> retinues;
        switch (retinueType)
        {
            case RetinueType.All:
                retinues = Retinues;
                break;
            case RetinueType.Soldier:
                retinues = Soldiers;
                break;
            case RetinueType.Hero:
                retinues = Heroes;
                break;
            default:
                retinues = Retinues;
                break;
        }

        return retinues;
    }

    public ServerModuleRetinue GetRandomRetinue(RetinueType retinueType, int exceptRetinueId)
    {
        List<ServerModuleRetinue> retinues = GetRetinueByType(retinueType);

        if (retinues.Count == 0)
        {
            return null;
        }
        else
        {
            int aliveCount = CountAliveRetinueExcept(retinueType, exceptRetinueId);
            Random rd = new Random();
            return GetAliveRetinueExcept(rd.Next(0, aliveCount), retinues, exceptRetinueId);
        }
    }

    public enum RetinueType
    {
        None,
        All,
        Soldier,
        Hero
    }

    public int CountAliveRetinueExcept(RetinueType retinueType, int exceptRetinueId)
    {
        List<ServerModuleRetinue> retinues = GetRetinueByType(retinueType);

        int count = 0;
        foreach (ServerModuleRetinue retinue in retinues)
        {
            if (!retinue.M_IsDead && retinue.M_RetinueID != exceptRetinueId) count++;
        }

        return count;
    }

    private ServerModuleRetinue GetAliveRetinue(int index, List<ServerModuleRetinue> retinues)
    {
        int count = -1;
        foreach (ServerModuleRetinue retinue in retinues)
        {
            if (!retinue.M_IsDead) count++;
            if (count == index) return retinue;
        }

        return null;
    }

    private ServerModuleRetinue GetAliveRetinueExcept(int index, List<ServerModuleRetinue> retinues, int exceptRetinueId)
    {
        int count = -1;
        foreach (ServerModuleRetinue retinue in retinues)
        {
            if (!retinue.M_IsDead && retinue.M_RetinueID != exceptRetinueId) count++;
            if (count == index) return retinue;
        }

        return null;
    }

    public void PrintRetinueInfos()
    {
        string log = "BattleGroundInfo: [ClientID]" + ServerPlayer.ClientId + " [Username]" + ServerPlayer.MyClientProxy.UserName;
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            log += " [RID]" + retinue.M_RetinueID + " [Name]" + retinue.CardInfo.BaseInfo.CardNames[LanguageManager_Common.GetCurrentLanguage()];
        }

        ServerLog.Print(log);
    }

    public static RetinueType GetRetinueTypeByTargetRange(TargetRange targetRange)
    {
        RetinueType retinueType = RetinueType.None;
        if ((targetRange & TargetRange.Heroes) == targetRange) // 若是Heroes子集
        {
            retinueType = RetinueType.Hero;
        }

        if ((targetRange & TargetRange.Soldiers) == targetRange) // 若是Soldiers子集
        {
            retinueType = RetinueType.Soldier;
        }
        else
        {
            retinueType = RetinueType.All;
        }

        return retinueType;
    }

    public static List<ServerPlayer> GetMechsPlayerByTargetRange(TargetRange targetRange, ServerPlayer player)
    {
        List<ServerPlayer> res = new List<ServerPlayer>();
        if ((targetRange & TargetRange.SelfMechs) == TargetRange.SelfMechs)
        {
            res.Add(player);
        }

        if ((targetRange & TargetRange.EnemyMechs) == TargetRange.EnemyMechs)
        {
            res.Add(player.MyEnemyPlayer);
        }

        return res;
    }
    public static List<ServerPlayer> GetShipsPlayerByTargetRange(TargetRange targetRange, ServerPlayer player)
    {
        List<ServerPlayer> res = new List<ServerPlayer>();
        if ((targetRange & TargetRange.SelfShip) == TargetRange.SelfShip)
        {
            res.Add(player);
        }

        if ((targetRange & TargetRange.EnemyShip) == TargetRange.EnemyShip)
        {
            res.Add(player.MyEnemyPlayer);
        }

        return res;
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnBeginRound, new ExecutorInfo(clientId: ServerPlayer.ClientId));
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            retinue.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEndRound, new ExecutorInfo(clientId: ServerPlayer.ClientId));
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            retinue.OnEndRound();
        }

        foreach (ServerModuleRetinue retinue in ServerPlayer.MyEnemyPlayer.MyBattleGroundManager.Retinues)
        {
            if (retinue.M_ImmuneLeftRounds > 0) retinue.M_ImmuneLeftRounds--;
            if (retinue.M_InactivityRounds > 0) retinue.M_InactivityRounds--;
        }
    }

    #endregion

    #endregion
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal class ServerBattleGroundManager
{
    private int retinueCount;

    public int RetinueCount
    {
        get { return retinueCount; }
        set
        {
            retinueCount = value;
            BattleGroundIsFull = retinueCount == GamePlaySettings.MaxRetinueNumber;
            BattleGroundIsEmpty = retinueCount == 0;
        }
    }

    private int heroCount;

    public int HeroCount
    {
        get { return heroCount; }
        set
        {
            heroCount = value;
            HerosIsEmpty = heroCount == 0;
        }
    }

    private int soldierCount;

    public int SoldierCount
    {
        get { return soldierCount; }
        set
        {
            soldierCount = value;
            SoldiersIsEmpty = soldierCount == 0;
        }
    }

    public bool BattleGroundIsFull;
    public bool BattleGroundIsEmpty;
    public bool HerosIsEmpty;
    public bool SoldiersIsEmpty;
    public ServerPlayer ServerPlayer;
    private List<ServerModuleRetinue> Retinues = new List<ServerModuleRetinue>();
    private List<ServerModuleRetinue> Heros = new List<ServerModuleRetinue>();
    private List<ServerModuleRetinue> Soldiers = new List<ServerModuleRetinue>();

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
            Heros.Add(retinue);
            HeroCount = Heros.Count;
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
            ServerLog.PrintWarning("BattleGroundRemoveRetinue不存在随从：" + retinue.M_RetinueID);
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
            Heros.Remove(retinue);
            HeroCount = Heros.Count;
        }

        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.RecycleCardInstanceID(retinue.OriginCardInstanceId);
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
        int retinueId = ServerPlayer.MyGameManager.GenerateNewRetinueId();
        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex, retinueId, clientRetinueTempId);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        ServerModuleRetinue retinue = new ServerModuleRetinue();
        retinue.M_RetinueID = retinueId;
        retinue.M_UsedClientRetinueTempId = clientRetinueTempId;
        retinue.OriginCardInstanceId = handCardInstanceId;
        retinue.Initiate(retinueCardInfo, ServerPlayer);

        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.AddCardInstanceId(retinueCardInfo.CardID, handCardInstanceId);

        BattleGroundAddRetinue(retinuePlaceIndex, retinue);

        SideEffectBase.ExecuterInfo info = new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId, retinueId: retinueId, targetRetinueId: targetRetinueId);
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnRetinueSummon, info);
        if (retinueCardInfo.RetinueInfo.IsSoldier) ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnSoldierSummon, info);
        else ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnHeroSummon, info);
    }

    public void EquipWeapon(EquipWeaponRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleWeapon weapon = new ServerModuleWeapon();
        CardInfo_Equip cardInfo_Weapon = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueId);
        weapon.M_ModuleRetinue = retinue;
        weapon.M_WeaponPlaceIndex = r.weaponPlaceIndex;
        weapon.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        weapon.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Weapon = weapon;
        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipShield(EquipShieldRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Equip cardInfo_Shield = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        shield.M_ModuleRetinue = retinue;
        shield.M_ShieldPlaceIndex = r.shieldPlaceIndex;
        shield.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        shield.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Shield = shield;
        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipPack(EquipPackRequest r, CardInfo_Base cardInfo)
    {
        ServerModulePack pack = new ServerModulePack();
        CardInfo_Equip cardInfo_Pack = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        pack.M_ModuleRetinue = retinue;
        pack.M_PackPlaceIndex = r.packPlaceIndex;
        pack.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        pack.Initiate(cardInfo_Pack, ServerPlayer);
        pack.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_Pack = pack;
        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipMA(EquipMARequest r, CardInfo_Base cardInfo)
    {
        ServerModuleMA ma = new ServerModuleMA();
        CardInfo_Equip cardInfo_MA = (CardInfo_Equip) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        ma.M_ModuleRetinue = retinue;
        ma.M_MAPlaceIndex = r.maPlaceIndex;
        ma.M_EquipID = ServerPlayer.MyGameManager.GenerateNewEquipId();
        ma.Initiate(cardInfo_MA, ServerPlayer);
        ma.OriginCardInstanceId = r.handCardInstanceId;
        retinue.M_MA = ma;
        ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void KillAllRetinues()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            serverModuleRetinue.OnDieTogather();
        }
    }


    public void KillAllHeros()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (!Retinues[i].CardInfo.RetinueInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            serverModuleRetinue.OnDieTogather();
        }
    }

    public void KillAllSodiers()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (Retinues[i].CardInfo.RetinueInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            serverModuleRetinue.OnDieTogather();
        }
    }


    public void KillOneRetinue(int retinueId)
    {
        KillOneRetinue(GetRetinue(retinueId));
    }

    public void KillRandomRetinue(int exceptRetinueId)
    {
        KillOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId));
    }

    public void KillRandomHero(int exceptRetinueId)
    {
        KillOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId));
    }

    public void KillRandomSoldier(int exceptRetinueId)
    {
        KillOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId));
    }

    private void KillOneRetinue(ServerModuleRetinue retinue)
    {
        if (retinue != null)
        {
            retinue.OnDieTogather();
            PrintRetinueInfos();
        }
    }

    private void AddLifeForOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            retinue.M_RetinueTotalLife += value;
            retinue.M_RetinueLeftLife += value;
        }
    }

    public void AddLifeForOneRetinue(int retinueId, int value)
    {
        AddLifeForOneRetinue(GetRetinue(retinueId), value);
    }

    public void AddLifeForRandomRetinue(int value, int exceptRetinueId)
    {
        AddLifeForOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void AddLifeForRandomHero(int value, int exceptRetinueId)
    {
        AddLifeForOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void AddLifeForRandomSoldier(int value, int exceptRetinueId)
    {
        AddLifeForOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void AddLifeForAllRetinues(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddLifeForAllHeros(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Heros)
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddLifeForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Soldiers)
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    private void AddAttackForOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            retinue.M_RetinueAttack += value;
        }
    }

    public void AddAttackForOneRetinue(int retinueId, int value)
    {
        AddAttackForOneRetinue(GetRetinue(retinueId), value);
    }

    public void AddAttackForAllRetinues(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            AddAttackForOneRetinue(serverModuleRetinue, value);
        }
    }
    public void AddAttackForRandomRetinue(int value, int exceptRetinueId)
    {
        AddAttackForOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void AddAttackForRandomHero(int value, int exceptRetinueId)
    {
        AddAttackForOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void AddAttackForRandomSoldier(int value, int exceptRetinueId)
    {
        AddAttackForOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void AddAttackForAllHeros(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Heros)
        {
            AddAttackForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddAttackForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Soldiers)
        {
            AddAttackForOneRetinue(serverModuleRetinue, value);
        }
    }

    private void HealOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            int healAmount = Math.Min(value, retinue.M_RetinueTotalLife - retinue.M_RetinueLeftLife);
            retinue.M_RetinueLeftLife += healAmount;
        }
    }

    public void HealRandomRetinue(int value, int exceptRetinueId)
    {
        HealOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void HealRandomHero(int value, int exceptRetinueId)
    {
        HealOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void HealRandomSoldier(int value, int exceptRetinueId)
    {
        HealOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void HealOneRetinue(int retinueId, int value)
    {
        HealOneRetinue(GetRetinue(retinueId), value);
    }

    public void HealAllRetinues(int value)
    {
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            HealOneRetinue(retinue, value);
        }
    }

    public void HealAllHeros(int value)
    {
        foreach (ServerModuleRetinue retinue in Heros)
        {
            HealOneRetinue(retinue, value);
        }
    }

    public void HealAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue retinue in Soldiers)
        {
            HealOneRetinue(retinue, value);
        }
    }

    public void DamageOneRetinue(int retinueId, int value)
    {
        DamageOneRetinue(GetRetinue(retinueId), value);
    }

    public void DamageRandomRetinue(int value, int exceptRetinueId)
    {
        DamageOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void DamageRandomHero(int value, int exceptRetinueId)
    {
        DamageOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void DamageRandomSoldier(int value, int exceptRetinueId)
    {
        DamageOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void DamageAllRetinues(int value)
    {
        for (int i = 0; i < Retinues.Count; i++)
        {
            ServerModuleRetinue retinue = Retinues[i];
            if (retinue != null)
            {
                retinue.BeAttacked(value);
                DamageOneRetinueRequest request = new DamageOneRetinueRequest(ServerPlayer.ClientId, retinue.M_RetinueID, value);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
                Retinues[i].CheckAlive();
            }
        }
    }

    public void DamageAllHeros(int value)
    {
        for (int i = 0; i < Heros.Count; i++)
        {
            ServerModuleRetinue retinue = Heros[i];
            if (retinue != null)
            {
                retinue.BeAttacked(value);
                DamageOneRetinueRequest request = new DamageOneRetinueRequest(ServerPlayer.ClientId, retinue.M_RetinueID, value);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
                Heros[i].CheckAlive();
            }
        }
    }

    public void DamageAllSoldiers(int value)
    {
        for (int i = 0; i < Soldiers.Count; i++)
        {
            ServerModuleRetinue retinue = Soldiers[i];
            if (retinue != null)
            {
                retinue.BeAttacked(value);
                DamageOneRetinueRequest request = new DamageOneRetinueRequest(ServerPlayer.ClientId, retinue.M_RetinueID, value);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
                Soldiers[i].CheckAlive();
            }
        }
    }

    private void DamageOneRetinue(ServerModuleRetinue targetRetinue, int value)
    {
        if (targetRetinue != null)
        {
            targetRetinue.BeAttacked(value);
            DamageOneRetinueRequest request = new DamageOneRetinueRequest(ServerPlayer.ClientId, targetRetinue.M_RetinueID, value);
            ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
            targetRetinue.CheckAlive();
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
                retinues = Heros;
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
            log += " [RID]" + retinue.M_RetinueID + " [Name]" + retinue.CardInfo.BaseInfo.CardName;
        }

        ServerLog.Print(log);
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnBeginRound, new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId));
    }

    internal void EndRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnEndRound, new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId));
    }

    #endregion

    #endregion
}
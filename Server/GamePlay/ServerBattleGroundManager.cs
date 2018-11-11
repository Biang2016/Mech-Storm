using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

    public bool HasDefenceRetinue
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

        retinue.CardInfo.SideEffectBundle_OnBattleGround.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnRetinueDie, SideEffectBundle.TriggerRange.Self);
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
            if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
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

        SideEffectBase.ExecuterInfo info = new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId, retinueId: retinueId, targetRetinueId: targetRetinueId);
        if (retinueCardInfo.RetinueInfo.IsSoldier) ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnSoldierSummon, info);
        else ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnHeroSummon, info);
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


    public void KillAllHeros()
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

    public void KillAllSodiers()
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
            retinue.OnDieTogether();
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
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues.ToArray())
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddLifeForAllHeros(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Heroes.ToArray())
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddLifeForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Soldiers.ToArray())
        {
            AddLifeForOneRetinue(serverModuleRetinue, value);
        }
    }

    private void AddArmorForOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            retinue.M_RetinueArmor += value;
        }
    }

    public void AddArmorForOneRetinue(int retinueId, int value)
    {
        AddArmorForOneRetinue(GetRetinue(retinueId), value);
    }

    public void AddArmorForRandomRetinue(int value, int exceptRetinueId)
    {
        AddArmorForOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void AddArmorForRandomHero(int value, int exceptRetinueId)
    {
        AddArmorForOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void AddArmorForRandomSoldier(int value, int exceptRetinueId)
    {
        AddArmorForOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void AddArmorForAllRetinues(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues.ToArray())
        {
            AddArmorForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddArmorForAllHeros(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Heroes.ToArray())
        {
            AddArmorForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddArmorForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Soldiers.ToArray())
        {
            AddArmorForOneRetinue(serverModuleRetinue, value);
        }
    }

    private void AddShieldForOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            retinue.M_RetinueShield += value;
        }
    }

    public void AddShieldForOneRetinue(int retinueId, int value)
    {
        AddShieldForOneRetinue(GetRetinue(retinueId), value);
    }

    public void AddShieldForRandomRetinue(int value, int exceptRetinueId)
    {
        AddShieldForOneRetinue(GetRandomRetinue(RetinueType.All, exceptRetinueId), value);
    }

    public void AddShieldForRandomHero(int value, int exceptRetinueId)
    {
        AddShieldForOneRetinue(GetRandomRetinue(RetinueType.Hero, exceptRetinueId), value);
    }

    public void AddShieldForRandomSoldier(int value, int exceptRetinueId)
    {
        AddShieldForOneRetinue(GetRandomRetinue(RetinueType.Soldier, exceptRetinueId), value);
    }

    public void AddShieldForAllRetinues(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues.ToArray())
        {
            AddShieldForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddShieldForAllHeros(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Heroes.ToArray())
        {
            AddShieldForOneRetinue(serverModuleRetinue, value);
        }
    }

    public void AddShieldForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Soldiers.ToArray())
        {
            AddShieldForOneRetinue(serverModuleRetinue, value);
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
        foreach (ServerModuleRetinue retinue in Retinues.ToArray())
        {
            AddAttackForOneRetinue(retinue, value);
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
        foreach (ServerModuleRetinue retinue in Heroes.ToArray())
        {
            AddAttackForOneRetinue(retinue, value);
        }
    }

    public void AddAttackForAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue retinue in Soldiers.ToArray())
        {
            AddAttackForOneRetinue(retinue, value);
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
        foreach (ServerModuleRetinue retinue in Retinues.ToArray())
        {
            HealOneRetinue(retinue, value);
        }
    }

    public void HealAllHeros(int value)
    {
        foreach (ServerModuleRetinue retinue in Heroes.ToArray())
        {
            HealOneRetinue(retinue, value);
        }
    }

    public void HealAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue retinue in Soldiers.ToArray())
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
        foreach (ServerModuleRetinue retinue in Retinues.ToArray())
        {
            retinue.BeAttacked(value);
            retinue.CheckAlive();
        }
    }

    public void DamageAllHeros(int value)
    {
        foreach (ServerModuleRetinue retinue in Heroes.ToArray())
        {
            retinue.BeAttacked(value);
            retinue.CheckAlive();
        }
    }

    public void DamageAllSoldiers(int value)
    {
        foreach (ServerModuleRetinue retinue in Soldiers.ToArray())
        {
            retinue.BeAttacked(value);
            retinue.CheckAlive();
        }
    }

    private void DamageOneRetinue(ServerModuleRetinue targetRetinue, int value)
    {
        if (targetRetinue != null)
        {
            targetRetinue.BeAttacked(value);
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

        if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
            ServerLog.Print(log);
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnBeginRound, new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId));
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            retinue.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnEndRound, new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId));
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
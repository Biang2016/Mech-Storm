using System;
using System.Collections.Generic;

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

    public void AddRetinue(CardInfo_Retinue retinueCardInfo)
    {
        AddRetinue(retinueCardInfo, Retinues.Count, -2, -1);
    }

    public void AddRetinue(CardInfo_Retinue retinueCardInfo, int retinuePlaceIndex, int targetRetinueId, int clientRetinueTempId)
    {
        int retinueId = ServerPlayer.MyGameManager.GeneratorNewRetinueId();
        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex, retinueId, clientRetinueTempId);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        ServerModuleRetinue retinue = new ServerModuleRetinue();
        retinue.M_RetinueID = retinueId;
        retinue.M_UsedClientRetinueTempId = clientRetinueTempId;
        retinue.Initiate(retinueCardInfo, ServerPlayer);

        retinue.OnSummoned(targetRetinueId); //先战吼，再进战场
        Retinues.Insert(retinuePlaceIndex, retinue);
        RetinueCount = Retinues.Count;
        if (retinue.CardInfo.BattleInfo.IsSoldier)
        {
            Soldiers.Add(retinue);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heros.Add(retinue);
            HeroCount = Heros.Count;
        }
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1) return;
        Retinues.Remove(retinue);
        RetinueCount = Retinues.Count;
        if (retinue.CardInfo.BattleInfo.IsSoldier)
        {
            Soldiers.Remove(retinue);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heros.Remove(retinue);
            HeroCount = Heros.Count;
        }

        BattleGroundRemoveRetinueRequest request = new BattleGroundRemoveRetinueRequest(new List<int> {retinue.M_RetinueID});
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    public void EquipWeapon(EquipWeaponRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleWeapon weapon = new ServerModuleWeapon();
        CardInfo_Weapon cardInfo_Weapon = (CardInfo_Weapon) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueId);
        weapon.M_ModuleRetinue = retinue;
        weapon.M_WeaponPlaceIndex = r.weaponPlaceIndex;
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        retinue.M_Weapon = weapon;
    }

    public void EquipShield(EquipShieldRequest r, CardInfo_Base cardInfo)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Shield cardInfo_Shield = (CardInfo_Shield) cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.retinueID);
        shield.M_ModuleRetinue = retinue;
        shield.M_ShieldPlaceIndex = r.shieldPlaceIndex;
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        retinue.M_Shield = shield;
    }

    public void UseSpellCard(UseSpellCardRequest r, CardInfo_Base cardInfo)
    {
        int targetRetinueId = r.targetRetinueId;
        if (r.isTargetRetinueIdTempId)
        {
            targetRetinueId = GetRetinueIdByClientRetinueTempId(r.targetRetinueId);
        }

        foreach (SideEffectBase se in cardInfo.SideEffects_OnSummoned)
        {
            if (se is TargetSideEffect)
            {
                if (((TargetSideEffect) se).IsNeedChoise) ((TargetSideEffect) se).TargetRetinueId = targetRetinueId;
            }

            se.Excute(ServerPlayer);
        }
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

        ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果

        Retinues.Clear();
        RetinueCount = Retinues.Count;
        Soldiers.Clear();
        SoldierCount = Soldiers.Count;
        Heros.Clear();
        HeroCount = Heros.Count;
    }

    public void KillAllHeros()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (!Retinues[i].CardInfo.BattleInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            serverModuleRetinue.OnDieTogather();
        }

        ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            Retinues.Remove(serverModuleRetinue);
            RetinueCount = Retinues.Count;
        }
    }

    public void KillAllSodiers()
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (Retinues[i].CardInfo.BattleInfo.IsSoldier) dieRetinues.Add(Retinues[i]);
        }

        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            serverModuleRetinue.OnDieTogather();
        }

        ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues)
        {
            Retinues.Remove(serverModuleRetinue);
            RetinueCount = Retinues.Count;
        }
    }

    public void KillOneRetinue(int retinueId)
    {
        KillOneRetinue(GetRetinue(retinueId));
    }

    public void KillRandomRetinue()
    {
        KillOneRetinue(GetRandomRetinue());
    }

    public void KillRandomHero()
    {
        KillOneRetinue(GetRandomHero());
    }

    public void KillRandomSoldier()
    {
        KillOneRetinue(GetRandomSoldier());
    }

    private void KillOneRetinue(ServerModuleRetinue retinue)
    {
        if (retinue != null)
        {
            retinue.OnDieTogather();
            ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果
            Retinues.Remove(retinue);
            RetinueCount = Retinues.Count;
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

    public void AddLifeForRandomRetinue(int value)
    {
        AddLifeForOneRetinue(GetRandomRetinue(), value);
    }

    public void AddLifeForRandomHero(int value)
    {
        AddLifeForOneRetinue(GetRandomHero(), value);
    }

    public void AddLifeForRandomSoldier(int value)
    {
        AddLifeForOneRetinue(GetRandomSoldier(), value);
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

    private void HealOneRetinue(ServerModuleRetinue retinue, int value)
    {
        if (retinue != null)
        {
            int healAmount = Math.Min(value, retinue.M_RetinueTotalLife - retinue.M_RetinueLeftLife);
            retinue.M_RetinueLeftLife += healAmount;
        }
    }

    public void HealRandomRetinue(int value)
    {
        HealOneRetinue(GetRandomRetinue(), value);
    }

    public void HealRandomHero(int value)
    {
        HealOneRetinue(GetRandomHero(), value);
    }

    public void HealRandomSoldier(int value)
    {
        HealOneRetinue(GetRandomSoldier(), value);
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

    public void DamageRandomRetinue(int value)
    {
        DamageOneRetinue(GetRandomRetinue(), value);
    }

    public void DamageRandomHero(int value)
    {
        DamageOneRetinue(GetRandomHero(), value);
    }

    public void DamageRandomSoldier(int value)
    {
        DamageOneRetinue(GetRandomSoldier(), value);
    }

    public void DamageAllRetinues(int value)
    {
        foreach (ServerModuleRetinue retinue in Retinues)
        {
            DamageOneRetinue(retinue, value);
        }
    }

    public void DamageAllHeros(int value)
    {
        foreach (ServerModuleRetinue retinue in Heros)
        {
            DamageOneRetinue(retinue, value);
        }
    }

    public void DamageAllSolders(int value)
    {
        foreach (ServerModuleRetinue retinue in Soldiers)
        {
            DamageOneRetinue(retinue, value);
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

    #endregion

    #region Utils

    public ServerModuleRetinue GetRetinue(int retinueId)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            if (serverModuleRetinue.M_RetinueID == retinueId) return serverModuleRetinue;
        }

        return null;
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

    public ServerModuleRetinue GetRandomRetinue()
    {
        if (Retinues.Count == 0)
        {
            return null;
        }
        else
        {
            Random rd = new Random();
            return Retinues[rd.Next(0, Retinues.Count)];
        }
    }

    public ServerModuleRetinue GetRandomHero()
    {
        if (Heros.Count == 0)
        {
            return null;
        }
        else
        {
            Random rd = new Random();
            return Heros[rd.Next(0, Heros.Count)];
        }
    }

    public ServerModuleRetinue GetRandomSoldier()
    {
        if (Soldiers.Count == 0)
        {
            return null;
        }
        else
        {
            Random rd = new Random();
            return Soldiers[rd.Next(0, Soldiers.Count)];
        }
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        foreach (ServerModuleRetinue mr in Retinues)
        {
            mr.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        foreach (ServerModuleRetinue mr in Retinues)
        {
            mr.OnEndRound();
        }
    }

    #endregion
}
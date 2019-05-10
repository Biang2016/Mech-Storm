using System;
using System.Collections.Generic;

internal partial class ServerGameManager
{
    public int CountMechsByTargetRange(TargetRange targetRange, ServerPlayer callerPlayer)
    {
        int count = 0;
        switch (targetRange)
        {
            case TargetRange.Mechs:
                count = callerPlayer.MyBattleGroundManager.RetinueCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                break;
            case TargetRange.Heroes:
                count = callerPlayer.MyBattleGroundManager.HeroCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                break;
            case TargetRange.Soldiers:
                count = callerPlayer.MyBattleGroundManager.SoldierCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                break;
            case TargetRange.SelfMechs:
                count = callerPlayer.MyBattleGroundManager.RetinueCount;
                break;
            case TargetRange.SelfHeroes:
                count = callerPlayer.MyBattleGroundManager.HeroCount;
                break;
            case TargetRange.SelfSoldiers:
                count = callerPlayer.MyBattleGroundManager.SoldierCount;
                break;
            case TargetRange.EnemyMechs:
                count = callerPlayer.MyEnemyPlayer.MyBattleGroundManager.RetinueCount;
                break;
            case TargetRange.EnemyHeroes:
                count = callerPlayer.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                break;
            case TargetRange.EnemySoldiers:
                count = callerPlayer.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                break;
        }

        return count;
    }

    public ServerModuleRetinue GetRetinue(int retinueId)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in PlayerA.MyBattleGroundManager.Retinues)
        {
            if (serverModuleRetinue.M_RetinueID == retinueId) return serverModuleRetinue;
        }

        foreach (ServerModuleRetinue serverModuleRetinue in PlayerB.MyBattleGroundManager.Retinues)
        {
            if (serverModuleRetinue.M_RetinueID == retinueId) return serverModuleRetinue;
        }

        return null;
    }

    public ServerModuleRetinue GetRandomRetinue(List<ServerPlayer> players, RetinueTypes retinueType, int exceptRetinueId)
    {
        List<ServerModuleRetinue> retinues = GetRetinuesByType(players, retinueType);

        if (retinues.Count == 0)
        {
            return null;
        }
        else
        {
            int aliveCount = CountAliveRetinueExcept(players, retinueType, exceptRetinueId);
            Random rd = new Random();
            return GetAliveRetinueExcept(rd.Next(0, aliveCount), retinues, exceptRetinueId);
        }
    }

    public int CountAliveRetinueExcept(List<ServerPlayer> players, RetinueTypes retinueType, int exceptRetinueId)
    {
        int count = 0;
        foreach (ServerPlayer serverPlayer in players)
        {
            count += serverPlayer.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        }

        return count;
    }

    public List<ServerModuleRetinue> GetRetinuesByType(List<ServerPlayer> players, RetinueTypes retinueType)
    {
        List<ServerModuleRetinue> retinues = new List<ServerModuleRetinue>();
        foreach (ServerPlayer ServerPlayer in players)
        {
            switch (retinueType)
            {
                case RetinueTypes.All:
                    retinues.AddRange(ServerPlayer.MyBattleGroundManager.Retinues.ToArray());
                    break;
                case RetinueTypes.Soldier:
                    retinues.AddRange(ServerPlayer.MyBattleGroundManager.Soldiers.ToArray());
                    break;
                case RetinueTypes.Hero:
                    retinues.AddRange(ServerPlayer.MyBattleGroundManager.Heroes.ToArray());
                    break;
            }
        }

        return retinues;
    }

    public static ServerModuleRetinue GetAliveRetinueExcept(int index, List<ServerModuleRetinue> retinues, int exceptRetinueId)
    {
        int count = -1;
        foreach (ServerModuleRetinue retinue in retinues)
        {
            if (!retinue.M_IsDead && retinue.M_RetinueID != exceptRetinueId) count++;
            if (count == index) return retinue;
        }

        return null;
    }

    public void SideEffect_ShipAction(Action<ServerPlayer> action, ServerPlayer callerPlayer, int count, TargetRange targetRange, TargetSelect targetSelect, List<int> targetClientIds)
    {
        List<ServerPlayer> players = GetShipsPlayerByTargetRange(targetRange, callerPlayer);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerPlayer p in players)
                {
                    action(p);
                }

                break;
            }
            case TargetSelect.Multiple:
            {
                foreach (int clientId in targetClientIds)
                {
                    action(GetPlayerByClientId(clientId));
                }

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                List<ServerPlayer> players_selected = Utils.GetRandomFromList(players, count);
                foreach (ServerPlayer p in players_selected)
                {
                    action(p);
                }

                break;
            }
            case TargetSelect.Single:
            {
                if (targetRange == TargetRange.SelfShip || targetRange == TargetRange.EnemyShip)
                {
                    action(players[0]);
                }
                else if (targetRange == TargetRange.Ships)
                {
                    action(GetPlayerByClientId(targetClientIds[0]));
                }

                break;
            }
            case TargetSelect.SingleRandom:
            {
                action(GetRandomPlayer(1)[0]);
                break;
            }
        }
    }

    public void SideEffect_ILifeAction(Action<ILife> action, ServerPlayer callerPlayer, int count, TargetRange targetRange, TargetSelect targetSelect, List<int> targetClientIds, List<int> targetRetinueIds)
    {
        List<ServerPlayer> mech_players = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        List<ServerPlayer> ship_players = GetShipsPlayerByTargetRange(targetRange, callerPlayer);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerPlayer player in mech_players)
                {
                    foreach (ServerModuleRetinue retinue in player.MyBattleGroundManager.Retinues)
                    {
                        action(retinue);
                    }
                }

                foreach (ServerPlayer player in ship_players)
                {
                    action(player);
                }

                break;
            }
            case TargetSelect.Multiple:
            {
                foreach (int clientId in targetClientIds)
                {
                    action(GetPlayerByClientId(clientId));
                }

                foreach (int retinueId in targetRetinueIds)
                {
                    action(GetRetinueOnBattleGround(retinueId));
                }

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                List<ILife> lives = GetAllLifeInBattleGround(mech_players, ship_players);
                List<ILife> selectedLives = Utils.GetRandomFromList(lives, count);
                foreach (ILife life in selectedLives)
                {
                    action(life);
                }

                break;
            }
            case TargetSelect.Single:
            {
                if (targetRange == TargetRange.SelfShip || targetRange == TargetRange.EnemyShip)
                {
                    action(ship_players[0]);
                }

                foreach (int targetClientId in targetClientIds)
                {
                    action(GetPlayerByClientId(targetClientId));
                    break;
                }

                foreach (int targetRetinueId in targetRetinueIds)
                {
                    if (targetRetinueId != Const.TARGET_RETINUE_SELECT_NONE)
                    {
                        action(GetRetinueOnBattleGround(targetRetinueId));
                        break;
                    }
                }

                break;
            }
            case TargetSelect.SingleRandom:
            {
                List<ILife> lives = GetAllLifeInBattleGround(mech_players, ship_players);
                List<ILife> selectedLives = Utils.GetRandomFromList(lives, 1);
                foreach (ILife life in selectedLives)
                {
                    action(life);
                    break;
                }

                break;
            }
        }
    }

    public List<ILife> GetAllLifeInBattleGround(List<ServerPlayer> mech_players, List<ServerPlayer> ship_players)
    {
        List<ILife> res = new List<ILife>();
        foreach (ServerPlayer player in mech_players)
        {
            foreach (ServerModuleRetinue smr in player.MyBattleGroundManager.Retinues)
            {
                res.Add(smr);
            }
        }

        foreach (ServerPlayer player in ship_players)
        {
            res.Add(player);
        }

        return res;
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

    public void SideEffect_RetinueAction(Action<ServerModuleRetinue> action, ServerPlayer callerPlayer, int count, List<int> retinueIds, TargetRange targetRange, TargetSelect targetSelect, int exceptRetinueId = -1)
    {
        List<ServerPlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        RetinueTypes retinueType = GetRetinueTypeByTargetRange(targetRange);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerModuleRetinue retinue in GetRetinuesByType(serverPlayers, retinueType).ToArray())
                {
                    action(retinue);
                }

                break;
            }
            case TargetSelect.Multiple:
            {
                foreach (int retinueId in retinueIds)
                {
                    action(GetRetinue(retinueId));
                }

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                foreach (ServerModuleRetinue retinue in Utils.GetRandomFromList(GetRetinuesByType(serverPlayers, retinueType), count))
                {
                    action(retinue);
                }

                break;
            }
            case TargetSelect.Single:
            {
                action(GetRetinue(retinueIds[0]));
                break;
            }
            case TargetSelect.SingleRandom:
            {
                action(GetRandomRetinue(serverPlayers, retinueType, exceptRetinueId));
                break;
            }
        }
    }

    public void KillRetinues(List<int> retinueIds)
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        foreach (int retinueId in retinueIds)
        {
            dieRetinues.Add(GetRetinue(retinueId));
        }

        KillRetinues(dieRetinues);
    }

    private void KillRetinues(List<ServerModuleRetinue> dieRetinues)
    {
        dieRetinues.Sort((a, b) => a.M_RetinueID.CompareTo(b.M_RetinueID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleRetinue serverModuleRetinue in dieRetinues.ToArray())
        {
            serverModuleRetinue.OnDieTogether();
        }
    }

    public void KillRetinues(int count, List<int> retinueIds, ServerPlayer callerPlayer, TargetRange targetRange, TargetSelect targetSelect, int exceptRetinueId = -1)
    {
        List<ServerPlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        RetinueTypes retinueType = GetRetinueTypeByTargetRange(targetRange);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                KillRetinues(GetRetinuesByType(serverPlayers, retinueType));
                break;
            }
            case TargetSelect.Multiple:
            {
                List<ServerModuleRetinue> retinues = new List<ServerModuleRetinue>();
                foreach (int retinueId in retinueIds)
                {
                    retinues.Add(GetRetinue(retinueId));
                }

                KillRetinues(retinues);

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                List<ServerModuleRetinue> retinues = new List<ServerModuleRetinue>();
                foreach (ServerModuleRetinue retinue in Utils.GetRandomFromList(GetRetinuesByType(serverPlayers, retinueType), count))
                {
                    retinues.Add(retinue);
                }

                KillRetinues(retinues);

                break;
            }
            case TargetSelect.Single:
            {
                KillRetinues(new List<ServerModuleRetinue> {GetRetinue(retinueIds[0])});
                break;
            }
            case TargetSelect.SingleRandom:
            {
                KillRetinues(new List<ServerModuleRetinue> {GetRandomRetinue(serverPlayers, retinueType, exceptRetinueId)});
                break;
            }
        }
    }

    public static RetinueTypes GetRetinueTypeByTargetRange(TargetRange targetRange)
    {
        RetinueTypes retinueType = RetinueTypes.None;
        if ((targetRange & TargetRange.Heroes) == targetRange) // 若是Heroes子集
        {
            retinueType = RetinueTypes.Hero;
        }

        if ((targetRange & TargetRange.Soldiers) == targetRange) // 若是Soldiers子集
        {
            retinueType = RetinueTypes.Soldier;
        }
        else
        {
            retinueType = RetinueTypes.All;
        }

        return retinueType;
    }

    public static ILifeCompositionTypes GetILifeCompositionTypesByTargetRange(TargetRange targetRange)
    {
        if ((targetRange & TargetRange.Mechs) != 0 && (targetRange & TargetRange.Ships) != 0)
        {
            return ILifeCompositionTypes.Mixed;
        }
        else if ((targetRange & TargetRange.Mechs) != 0 && (targetRange & TargetRange.Ships) == 0)
        {
            return ILifeCompositionTypes.OnlyMechs;
        }
        else if ((targetRange & TargetRange.Mechs) == 0 && (targetRange & TargetRange.Ships) != 0)
        {
            return ILifeCompositionTypes.OnlyShips;
        }

        return ILifeCompositionTypes.None;
    }

    public static List<ServerPlayer> GetMechsPlayerByTargetRange(TargetRange targetRange, ServerPlayer player)
    {
        List<ServerPlayer> res = new List<ServerPlayer>();
        if ((targetRange & TargetRange.SelfMechs) != 0)
        {
            res.Add(player);
        }

        if ((targetRange & TargetRange.EnemyMechs) != 0)
        {
            res.Add(player.MyEnemyPlayer);
        }

        return res;
    }

    public static List<ServerPlayer> GetShipsPlayerByTargetRange(TargetRange targetRange, ServerPlayer player)
    {
        List<ServerPlayer> res = new List<ServerPlayer>();
        if ((targetRange & TargetRange.SelfShip) != 0)
        {
            res.Add(player);
        }

        if ((targetRange & TargetRange.EnemyShip) != 0)
        {
            res.Add(player.MyEnemyPlayer);
        }

        return res;
    }

    public List<ServerPlayer> GetRandomPlayer(int count)
    {
        return Utils.GetRandomFromList(new List<ServerPlayer> {PlayerA, PlayerB}, count);
    }

    public ServerModuleRetinue GetRetinueOnBattleGround(int retinueId)
    {
        ServerModuleRetinue retinue = PlayerA.MyBattleGroundManager.GetRetinue(retinueId);
        if (retinue == null) retinue = PlayerB.MyBattleGroundManager.GetRetinue(retinueId);
        return retinue;
    }

    public ServerModuleRetinue GetRandomAliveRetinueExcept(RetinueTypes retinueType, int exceptRetinueId)
    {
        int countA = PlayerA.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        int countB = PlayerB.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        Random rd = new Random();
        int ranResult = rd.Next(0, countA + countB);
        if (ranResult < countA)
        {
            return PlayerA.MyBattleGroundManager.GetRandomRetinue(retinueType, exceptRetinueId);
        }
        else
        {
            return PlayerB.MyBattleGroundManager.GetRandomRetinue(retinueType, exceptRetinueId);
        }
    }

    public ClientProxy GetClientProxyByClientId(int clientId)
    {
        if (ClientA.ClientId == clientId)
        {
            return ClientA;
        }
        else if (ClientB.ClientId == clientId)
        {
            return ClientB;
        }

        return null;
    }

    public ClientProxy GetEnemyClientProxyByClientId(int clientId)
    {
        if (ClientA.ClientId == clientId)
        {
            return ClientB;
        }
        else if (ClientB.ClientId == clientId)
        {
            return ClientA;
        }

        return null;
    }

    public ServerPlayer GetPlayerByClientId(int clientId)
    {
        if (PlayerA.ClientId == clientId)
        {
            return PlayerA;
        }
        else if (PlayerB.ClientId == clientId)
        {
            return PlayerB;
        }

        return null;
    }

    public void RemoveEquipByEquipID(int equipID)
    {
        foreach (ServerModuleRetinue retinue in GetRetinuesByType(new List<ServerPlayer> {PlayerA, PlayerB}, RetinueTypes.All))
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
}

public enum RetinueTypes
{
    None,
    All,
    Soldier,
    Hero
}

public enum ILifeCompositionTypes
{
    None,
    Mixed,
    OnlyMechs,
    OnlyShips
}
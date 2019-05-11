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
                count = callerPlayer.MyBattleGroundManager.MechCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.MechCount;
                break;
            case TargetRange.Heroes:
                count = callerPlayer.MyBattleGroundManager.HeroCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.HeroCount;
                break;
            case TargetRange.Soldiers:
                count = callerPlayer.MyBattleGroundManager.SoldierCount + callerPlayer.MyEnemyPlayer.MyBattleGroundManager.SoldierCount;
                break;
            case TargetRange.SelfMechs:
                count = callerPlayer.MyBattleGroundManager.MechCount;
                break;
            case TargetRange.SelfHeroes:
                count = callerPlayer.MyBattleGroundManager.HeroCount;
                break;
            case TargetRange.SelfSoldiers:
                count = callerPlayer.MyBattleGroundManager.SoldierCount;
                break;
            case TargetRange.EnemyMechs:
                count = callerPlayer.MyEnemyPlayer.MyBattleGroundManager.MechCount;
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

    public ServerModuleMech GetMech(int mechId)
    {
        foreach (ServerModuleMech serverModuleMech in PlayerA.MyBattleGroundManager.Mechs)
        {
            if (serverModuleMech.M_MechID == mechId) return serverModuleMech;
        }

        foreach (ServerModuleMech serverModuleMech in PlayerB.MyBattleGroundManager.Mechs)
        {
            if (serverModuleMech.M_MechID == mechId) return serverModuleMech;
        }

        return null;
    }

    public ServerModuleMech GetRandomMech(List<ServerPlayer> players, MechTypes mechType, int exceptMechId)
    {
        List<ServerModuleMech> mechs = GetMechsByType(players, mechType);

        if (mechs.Count == 0)
        {
            return null;
        }
        else
        {
            int aliveCount = CountAliveMechExcept(players, mechType, exceptMechId);
            Random rd = new Random();
            return GetAliveMechExcept(rd.Next(0, aliveCount), mechs, exceptMechId);
        }
    }

    public int CountAliveMechExcept(List<ServerPlayer> players, MechTypes mechType, int exceptMechId)
    {
        int count = 0;
        foreach (ServerPlayer serverPlayer in players)
        {
            count += serverPlayer.MyBattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        }

        return count;
    }

    public List<ServerModuleMech> GetMechsByType(List<ServerPlayer> players, MechTypes mechType)
    {
        List<ServerModuleMech> mechs = new List<ServerModuleMech>();
        foreach (ServerPlayer ServerPlayer in players)
        {
            switch (mechType)
            {
                case MechTypes.All:
                    mechs.AddRange(ServerPlayer.MyBattleGroundManager.Mechs.ToArray());
                    break;
                case MechTypes.Soldier:
                    mechs.AddRange(ServerPlayer.MyBattleGroundManager.Soldiers.ToArray());
                    break;
                case MechTypes.Hero:
                    mechs.AddRange(ServerPlayer.MyBattleGroundManager.Heroes.ToArray());
                    break;
            }
        }

        return mechs;
    }

    public static ServerModuleMech GetAliveMechExcept(int index, List<ServerModuleMech> mechs, int exceptMechId)
    {
        int count = -1;
        foreach (ServerModuleMech mech in mechs)
        {
            if (!mech.M_IsDead && mech.M_MechID != exceptMechId) count++;
            if (count == index) return mech;
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

    public void SideEffect_ILifeAction(Action<ILife> action, ServerPlayer callerPlayer, int count, TargetRange targetRange, TargetSelect targetSelect, List<int> targetClientIds, List<int> targetMechIds)
    {
        List<ServerPlayer> mech_players = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        List<ServerPlayer> ship_players = GetShipsPlayerByTargetRange(targetRange, callerPlayer);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerPlayer player in mech_players)
                {
                    foreach (ServerModuleMech mech in player.MyBattleGroundManager.Mechs)
                    {
                        action(mech);
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

                foreach (int mechId in targetMechIds)
                {
                    action(GetMechOnBattleGround(mechId));
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
                else
                {
                    foreach (int targetClientId in targetClientIds)
                    {
                        action(GetPlayerByClientId(targetClientId));
                        break;
                    }

                    foreach (int targetMechId in targetMechIds)
                    {
                        if (targetMechId != Const.TARGET_MECH_SELECT_NONE)
                        {
                            action(GetMechOnBattleGround(targetMechId));
                            break;
                        }
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
            foreach (ServerModuleMech smr in player.MyBattleGroundManager.Mechs)
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

    public enum MechValueTypes
    {
        Life,
        Heal,
        Damage,
        Attack,
        Armor,
        Shield,
        WeaponEnergy,
    }

    private Dictionary<MechValueTypes, Action<ServerModuleMech, int>> MechValueChangeDelegates = new Dictionary<MechValueTypes, Action<ServerModuleMech, int>>
    {
        {MechValueTypes.Life, delegate(ServerModuleMech mech, int value) { mech?.AddLife(value); }},
        {MechValueTypes.Heal, delegate(ServerModuleMech mech, int value) { mech?.Heal(value); }},
        {
            MechValueTypes.Damage, delegate(ServerModuleMech targetMech, int value)
            {
                targetMech.BeAttacked(value);
                targetMech.CheckAlive();
            }
        },
        {MechValueTypes.Attack, delegate(ServerModuleMech mech, int value) { mech.M_MechAttack += value; }},
        {MechValueTypes.Armor, delegate(ServerModuleMech mech, int value) { mech.M_MechArmor += value; }},
        {MechValueTypes.Shield, delegate(ServerModuleMech mech, int value) { mech.M_MechShield += value; }},
        {MechValueTypes.WeaponEnergy, delegate(ServerModuleMech mech, int value) { mech.M_MechWeaponEnergy += value; }},
    };

    public void SideEffect_MechAction(Action<ServerModuleMech> action, ServerPlayer callerPlayer, int count, List<int> mechIds, TargetRange targetRange, TargetSelect targetSelect, int exceptMechId = -1)
    {
        List<ServerPlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        MechTypes mechType = GetMechTypeByTargetRange(targetRange);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ServerModuleMech mech in GetMechsByType(serverPlayers, mechType).ToArray())
                {
                    action(mech);
                }

                break;
            }
            case TargetSelect.Multiple:
            {
                foreach (int mechId in mechIds)
                {
                    action(GetMech(mechId));
                }

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                foreach (ServerModuleMech mech in Utils.GetRandomFromList(GetMechsByType(serverPlayers, mechType), count))
                {
                    action(mech);
                }

                break;
            }
            case TargetSelect.Single:
            {
                action(GetMech(mechIds[0]));
                break;
            }
            case TargetSelect.SingleRandom:
            {
                action(GetRandomMech(serverPlayers, mechType, exceptMechId));
                break;
            }
        }
    }

    public void KillMechs(List<int> mechIds)
    {
        List<ServerModuleMech> dieMechs = new List<ServerModuleMech>();
        foreach (int mechId in mechIds)
        {
            dieMechs.Add(GetMech(mechId));
        }

        KillMechs(dieMechs);
    }

    private void KillMechs(List<ServerModuleMech> dieMechs)
    {
        dieMechs.Sort((a, b) => a.M_MechID.CompareTo(b.M_MechID)); //按照上场顺序加入死亡队列

        foreach (ServerModuleMech serverModuleMech in dieMechs.ToArray())
        {
            serverModuleMech.OnDieTogether();
        }
    }

    public void KillMechs(int count, List<int> mechIds, ServerPlayer callerPlayer, TargetRange targetRange, TargetSelect targetSelect, int exceptMechId = -1)
    {
        List<ServerPlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        MechTypes mechType = GetMechTypeByTargetRange(targetRange);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                KillMechs(GetMechsByType(serverPlayers, mechType));
                break;
            }
            case TargetSelect.Multiple:
            {
                List<ServerModuleMech> mechs = new List<ServerModuleMech>();
                foreach (int mechId in mechIds)
                {
                    mechs.Add(GetMech(mechId));
                }

                KillMechs(mechs);

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                List<ServerModuleMech> mechs = new List<ServerModuleMech>();
                foreach (ServerModuleMech mech in Utils.GetRandomFromList(GetMechsByType(serverPlayers, mechType), count))
                {
                    mechs.Add(mech);
                }

                KillMechs(mechs);

                break;
            }
            case TargetSelect.Single:
            {
                KillMechs(new List<ServerModuleMech> {GetMech(mechIds[0])});
                break;
            }
            case TargetSelect.SingleRandom:
            {
                KillMechs(new List<ServerModuleMech> {GetRandomMech(serverPlayers, mechType, exceptMechId)});
                break;
            }
        }
    }

    public static MechTypes GetMechTypeByTargetRange(TargetRange targetRange)
    {
        MechTypes mechType = MechTypes.None;
        if ((targetRange & TargetRange.Heroes) == targetRange) // 若是Heroes子集
        {
            mechType = MechTypes.Hero;
        }

        if ((targetRange & TargetRange.Soldiers) == targetRange) // 若是Soldiers子集
        {
            mechType = MechTypes.Soldier;
        }
        else
        {
            mechType = MechTypes.All;
        }

        return mechType;
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

    public ServerModuleMech GetMechOnBattleGround(int mechId)
    {
        ServerModuleMech mech = PlayerA.MyBattleGroundManager.GetMech(mechId);
        if (mech == null) mech = PlayerB.MyBattleGroundManager.GetMech(mechId);
        return mech;
    }

    public ServerModuleMech GetRandomAliveMechExcept(MechTypes mechType, int exceptMechId)
    {
        int countA = PlayerA.MyBattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        int countB = PlayerB.MyBattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        Random rd = new Random();
        int ranResult = rd.Next(0, countA + countB);
        if (ranResult < countA)
        {
            return PlayerA.MyBattleGroundManager.GetRandomMech(mechType, exceptMechId);
        }
        else
        {
            return PlayerB.MyBattleGroundManager.GetRandomMech(mechType, exceptMechId);
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
        foreach (ServerModuleMech mech in GetMechsByType(new List<ServerPlayer> {PlayerA, PlayerB}, MechTypes.All))
        {
            if (mech.M_Weapon != null && mech.M_Weapon.M_EquipID == equipID)
            {
                mech.M_Weapon = null;
            }

            if (mech.M_Shield != null && mech.M_Shield.M_EquipID == equipID)
            {
                mech.M_Shield = null;
            }

            if (mech.M_Pack != null && mech.M_Pack.M_EquipID == equipID)
            {
                mech.M_Pack = null;
            }

            if (mech.M_MA != null && mech.M_MA.M_EquipID == equipID)
            {
                mech.M_MA = null;
            }
        }
    }
}

public enum MechTypes
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
using System;
using System.Collections.Generic;

internal partial class GameManager
{
    public int CountMechsByTargetRange(TargetRange targetRange, BattlePlayer callerPlayer)
    {
        int count = 0;
        switch (targetRange)
        {
            case TargetRange.Mechs:
                count = callerPlayer.BattleGroundManager.MechCount + callerPlayer.MyEnemyPlayer.BattleGroundManager.MechCount;
                break;
            case TargetRange.Heroes:
                count = callerPlayer.BattleGroundManager.HeroCount + callerPlayer.MyEnemyPlayer.BattleGroundManager.HeroCount;
                break;
            case TargetRange.Soldiers:
                count = callerPlayer.BattleGroundManager.SoldierCount + callerPlayer.MyEnemyPlayer.BattleGroundManager.SoldierCount;
                break;
            case TargetRange.SelfMechs:
                count = callerPlayer.BattleGroundManager.MechCount;
                break;
            case TargetRange.SelfHeroes:
                count = callerPlayer.BattleGroundManager.HeroCount;
                break;
            case TargetRange.SelfSoldiers:
                count = callerPlayer.BattleGroundManager.SoldierCount;
                break;
            case TargetRange.EnemyMechs:
                count = callerPlayer.MyEnemyPlayer.BattleGroundManager.MechCount;
                break;
            case TargetRange.EnemyHeroes:
                count = callerPlayer.MyEnemyPlayer.BattleGroundManager.HeroCount;
                break;
            case TargetRange.EnemySoldiers:
                count = callerPlayer.MyEnemyPlayer.BattleGroundManager.SoldierCount;
                break;
        }

        return count;
    }

    public ModuleMech GetMech(int mechId)
    {
        foreach (ModuleMech serverModuleMech in PlayerA.BattleGroundManager.Mechs)
        {
            if (serverModuleMech.M_MechID == mechId) return serverModuleMech;
        }

        foreach (ModuleMech serverModuleMech in PlayerB.BattleGroundManager.Mechs)
        {
            if (serverModuleMech.M_MechID == mechId) return serverModuleMech;
        }

        return null;
    }

    public ModuleMech GetRandomMech(List<BattlePlayer> players, MechTypes mechType, int exceptMechId)
    {
        List<ModuleMech> mechs = GetMechsByType(players, mechType);

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

    public int CountAliveMechExcept(List<BattlePlayer> players, MechTypes mechType, int exceptMechId)
    {
        int count = 0;
        foreach (BattlePlayer serverPlayer in players)
        {
            count += serverPlayer.BattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        }

        return count;
    }

    public List<ModuleMech> GetMechsByType(List<BattlePlayer> players, MechTypes mechType)
    {
        List<ModuleMech> mechs = new List<ModuleMech>();
        foreach (BattlePlayer ServerPlayer in players)
        {
            switch (mechType)
            {
                case MechTypes.All:
                    mechs.AddRange(ServerPlayer.BattleGroundManager.Mechs.ToArray());
                    break;
                case MechTypes.Soldier:
                    mechs.AddRange(ServerPlayer.BattleGroundManager.Soldiers.ToArray());
                    break;
                case MechTypes.Hero:
                    mechs.AddRange(ServerPlayer.BattleGroundManager.Heroes.ToArray());
                    break;
            }
        }

        return mechs;
    }

    public static ModuleMech GetAliveMechExcept(int index, List<ModuleMech> mechs, int exceptMechId)
    {
        int count = -1;
        foreach (ModuleMech mech in mechs)
        {
            if (!mech.M_IsDead && mech.M_MechID != exceptMechId) count++;
            if (count == index) return mech;
        }

        return null;
    }

    public void SideEffect_ShipAction(Action<BattlePlayer> action, BattlePlayer callerPlayer, int count, TargetRange targetRange, TargetSelect targetSelect, List<int> targetClientIds)
    {
        List<BattlePlayer> players = GetShipsPlayerByTargetRange(targetRange, callerPlayer);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (BattlePlayer p in players)
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
                List<BattlePlayer> players_selected = Utils.GetRandomFromList(players, count);
                foreach (BattlePlayer p in players_selected)
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

    public void SideEffect_ILifeAction(Action<ILife> action, BattlePlayer callerPlayer, int count, TargetRange targetRange, TargetSelect targetSelect, List<int> targetClientIds, List<int> targetMechIds)
    {
        List<BattlePlayer> mech_players = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        List<BattlePlayer> ship_players = GetShipsPlayerByTargetRange(targetRange, callerPlayer);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (BattlePlayer player in mech_players)
                {
                    foreach (ModuleMech mech in player.BattleGroundManager.Mechs)
                    {
                        action(mech);
                    }
                }

                foreach (BattlePlayer player in ship_players)
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

    public List<ILife> GetAllLifeInBattleGround(List<BattlePlayer> mech_players, List<BattlePlayer> ship_players)
    {
        List<ILife> res = new List<ILife>();
        foreach (BattlePlayer player in mech_players)
        {
            foreach (ModuleMech smr in player.BattleGroundManager.Mechs)
            {
                res.Add(smr);
            }
        }

        foreach (BattlePlayer player in ship_players)
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

    private Dictionary<MechValueTypes, Action<ModuleMech, int>> MechValueChangeDelegates = new Dictionary<MechValueTypes, Action<ModuleMech, int>>
    {
        {MechValueTypes.Life, delegate(ModuleMech mech, int value) { mech?.AddLife(value); }},
        {MechValueTypes.Heal, delegate(ModuleMech mech, int value) { mech?.Heal(value); }},
        {
            MechValueTypes.Damage, delegate(ModuleMech targetMech, int value)
            {
                targetMech.BeAttacked(value);
                targetMech.CheckAlive();
            }
        },
        {MechValueTypes.Attack, delegate(ModuleMech mech, int value) { mech.M_MechAttack += value; }},
        {MechValueTypes.Armor, delegate(ModuleMech mech, int value) { mech.M_MechArmor += value; }},
        {MechValueTypes.Shield, delegate(ModuleMech mech, int value) { mech.M_MechShield += value; }},
        {MechValueTypes.WeaponEnergy, delegate(ModuleMech mech, int value) { mech.M_MechWeaponEnergy += value; }},
    };

    public void SideEffect_MechAction(Action<ModuleMech> action, BattlePlayer callerPlayer, int count, List<int> mechIds, TargetRange targetRange, TargetSelect targetSelect, int exceptMechId = -1)
    {
        List<BattlePlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
        MechTypes mechType = GetMechTypeByTargetRange(targetRange);
        switch (targetSelect)
        {
            case TargetSelect.All:
            {
                foreach (ModuleMech mech in GetMechsByType(serverPlayers, mechType).ToArray())
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
                foreach (ModuleMech mech in Utils.GetRandomFromList(GetMechsByType(serverPlayers, mechType), count))
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
        List<ModuleMech> dieMechs = new List<ModuleMech>();
        foreach (int mechId in mechIds)
        {
            dieMechs.Add(GetMech(mechId));
        }

        KillMechs(dieMechs);
    }

    private void KillMechs(List<ModuleMech> dieMechs)
    {
        dieMechs.Sort((a, b) => a.M_MechID.CompareTo(b.M_MechID)); //按照上场顺序加入死亡队列

        foreach (ModuleMech serverModuleMech in dieMechs.ToArray())
        {
            serverModuleMech.OnDieTogether();
        }
    }

    public void KillMechs(int count, List<int> mechIds, BattlePlayer callerPlayer, TargetRange targetRange, TargetSelect targetSelect, int exceptMechId = -1)
    {
        List<BattlePlayer> serverPlayers = GetMechsPlayerByTargetRange(targetRange, callerPlayer);
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
                List<ModuleMech> mechs = new List<ModuleMech>();
                foreach (int mechId in mechIds)
                {
                    mechs.Add(GetMech(mechId));
                }

                KillMechs(mechs);

                break;
            }
            case TargetSelect.MultipleRandom:
            {
                List<ModuleMech> mechs = new List<ModuleMech>();
                foreach (ModuleMech mech in Utils.GetRandomFromList(GetMechsByType(serverPlayers, mechType), count))
                {
                    mechs.Add(mech);
                }

                KillMechs(mechs);

                break;
            }
            case TargetSelect.Single:
            {
                KillMechs(new List<ModuleMech> {GetMech(mechIds[0])});
                break;
            }
            case TargetSelect.SingleRandom:
            {
                KillMechs(new List<ModuleMech> {GetRandomMech(serverPlayers, mechType, exceptMechId)});
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

    public static List<BattlePlayer> GetMechsPlayerByTargetRange(TargetRange targetRange, BattlePlayer player)
    {
        List<BattlePlayer> res = new List<BattlePlayer>();
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

    public static List<BattlePlayer> GetShipsPlayerByTargetRange(TargetRange targetRange, BattlePlayer player)
    {
        List<BattlePlayer> res = new List<BattlePlayer>();
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

    public List<BattlePlayer> GetRandomPlayer(int count)
    {
        return Utils.GetRandomFromList(new List<BattlePlayer> {PlayerA, PlayerB}, count);
    }

    public ModuleMech GetMechOnBattleGround(int mechId)
    {
        ModuleMech mech = PlayerA.BattleGroundManager.GetMech(mechId);
        if (mech == null) mech = PlayerB.BattleGroundManager.GetMech(mechId);
        return mech;
    }

    public ModuleMech GetRandomAliveMechExcept(MechTypes mechType, int exceptMechId)
    {
        int countA = PlayerA.BattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        int countB = PlayerB.BattleGroundManager.CountAliveMechExcept(mechType, exceptMechId);
        Random rd = new Random();
        int ranResult = rd.Next(0, countA + countB);
        if (ranResult < countA)
        {
            return PlayerA.BattleGroundManager.GetRandomMech(mechType, exceptMechId);
        }
        else
        {
            return PlayerB.BattleGroundManager.GetRandomMech(mechType, exceptMechId);
        }
    }

    public BattleClientProxy GetClientProxyByClientId(int clientId)
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

    public BattleClientProxy GetEnemyClientProxyByClientId(int clientId)
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

    public BattlePlayer GetPlayerByClientId(int clientId)
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
        foreach (ModuleMech mech in GetMechsByType(new List<BattlePlayer> {PlayerA, PlayerB}, MechTypes.All))
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
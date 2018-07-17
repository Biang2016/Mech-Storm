using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class ServerBattleGroundManager
{
    public bool BattleGroundIsFull;
    public ServerPlayer ServerPlayer;
    private List<ServerModuleRetinue> Retinues = new List<ServerModuleRetinue>();

    public ServerBattleGroundManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }


    #region SideEffects

    public void SummonRetinue(SummonRetinueRequest r)
    {
        ServerModuleRetinue retinue = new ServerModuleRetinue();
        CardInfo_Retinue cardInfoRetinue = r.cardInfo;
        retinue.Initiate(cardInfoRetinue, ServerPlayer);
        retinue.M_RetinuePlaceIndex = r.battleGroundIndex;
        Retinues.Insert(r.battleGroundIndex, retinue);
        if (Retinues.Count == GamePlaySettings.MaxRetinueNumber) BattleGroundIsFull = true;
    }

    public void EquipWeapon(EquipWeaponRequest r)
    {
        ServerModuleWeapon weapon = new ServerModuleWeapon();
        CardInfo_Weapon cardInfo_Weapon = r.cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.battleGroundIndex);
        weapon.M_ModuleRetinue = retinue;
        weapon.M_RetinuePlaceIndex = r.battleGroundIndex;
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        retinue.M_Weapon = weapon;
    }

    public void EquipShield(EquipShieldRequest r)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Shield cardInfoShield = r.cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.battleGroundIndex);
        shield.M_ModuleRetinue = retinue;
        shield.M_RetinuePlaceIndex = r.battleGroundIndex;
        shield.Initiate(cardInfoShield, ServerPlayer);
        retinue.M_Shield = shield;
    }

    public void KillAllInBattleGround() //杀死清场
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            serverModuleRetinue.OnDie();
        }

        Retinues.Clear();
    }

    public void AddLifeForSomeRetinue(int retinuePlaceIndex, int addLife) //增加某随从生命
    {
        Retinues[retinuePlaceIndex].M_RetinueLeftLife += addLife;
    }

    #endregion

    #region Utils

    public ServerModuleRetinue GetRetinue(int retinuePlaceIndex)
    {
        return Retinues[retinuePlaceIndex];
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        Retinues.Remove(retinue);
        if (Retinues.Count < GamePlaySettings.MaxRetinueNumber)
        {
            BattleGroundIsFull = false;
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
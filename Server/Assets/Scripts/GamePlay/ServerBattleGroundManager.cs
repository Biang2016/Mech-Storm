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

    public void AddRetinue(CardInfo_Retinue retinueCardInfo, int retinuePlaceIndex)
    {
        ServerModuleRetinue retinue = new ServerModuleRetinue();
        retinue.Initiate(retinueCardInfo, ServerPlayer);
        retinue.M_RetinuePlaceIndex = retinuePlaceIndex;
        Retinues.Insert(retinuePlaceIndex, retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;

        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }


    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        Retinues.Remove(retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;

        BattleGroundRemoveRetinueRequest request = new BattleGroundRemoveRetinueRequest(ServerPlayer.ClientId, Retinues.IndexOf(retinue));
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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

        EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, cardInfo_Weapon, r.battleGroundIndex, r.weaponPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    public void EquipShield(EquipShieldRequest r)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Shield cardInfo_Shield = r.cardInfo;
        ServerModuleRetinue retinue = GetRetinue(r.battleGroundIndex);
        shield.M_ModuleRetinue = retinue;
        shield.M_RetinuePlaceIndex = r.battleGroundIndex;
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        retinue.M_Shield = shield;

        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, cardInfo_Shield, r.battleGroundIndex, r.shieldPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }


    public void KillAllInBattleGround() //杀死本方清场
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            serverModuleRetinue.OnDie();
        }

        while (Retinues.Count > 0)
        {
            RemoveRetinue(Retinues[0]);
        }
    }

    public void AddLifeForSomeRetinue(int retinuePlaceIndex, int value) //本方增加某随从生命
    {
        Retinues[retinuePlaceIndex].M_RetinueLeftLife += value;
    }

    #endregion

    #region Utils

    public ServerModuleRetinue GetRetinue(int retinuePlaceIndex)
    {
        return Retinues[retinuePlaceIndex];
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
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
        retinue.OnSummoned();

        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            serverModuleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(serverModuleRetinue);
        }

        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1) return;
        Retinues.Remove(retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;

        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            serverModuleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(serverModuleRetinue);
        }

        BattleGroundRemoveRetinueRequest request = new BattleGroundRemoveRetinueRequest(new List<RetinuePlaceInfo> {new RetinuePlaceInfo(ServerPlayer.ClientId, battleGroundIndex)});
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    public void RemoveRetinueTogather(ServerModuleRetinue retinue)//群杀时采用的方法
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1) return;
        Retinues.Remove(retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;

        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            serverModuleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(serverModuleRetinue);
        }
    }

    public void EquipWeapon(EquipWeaponRequest r)
    {
        ServerModuleWeapon weapon = new ServerModuleWeapon();
        CardInfo_Weapon cardInfo_Weapon = (CardInfo_Weapon) ServerPlayer.MyHandManager.GetHandCardInfo(r.handCardIndex);
        ServerModuleRetinue retinue = GetRetinue(r.battleGroundIndex);
        weapon.M_ModuleRetinue = retinue;
        weapon.M_WeaponPlaceIndex = r.weaponPlaceIndex;
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        retinue.M_Weapon = weapon;
    }

    public void EquipShield(EquipShieldRequest r)
    {
        ServerModuleShield shield = new ServerModuleShield();
        CardInfo_Shield cardInfo_Shield = (CardInfo_Shield) ServerPlayer.MyHandManager.GetHandCardInfo(r.handCardIndex);
        ServerModuleRetinue retinue = GetRetinue(r.battleGroundIndex);
        shield.M_ModuleRetinue = retinue;
        shield.M_ShieldPlaceIndex = r.shieldPlaceIndex;
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        retinue.M_Shield = shield;
    }

    public void KillAllInBattleGround() //杀死本方清场
    {
        for (int i = 0; i < Retinues.Count; i++)
        {
            Retinues[i].OnDieTogather();
        }

        ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果


        while (Retinues.Count > 0)
        {
            RemoveRetinueTogather(Retinues[0]);
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

    public ServerModuleRetinue GetRandomRetinue()
    {
        Random rd = new Random();
        if (Retinues.Count == 0)
        {
            return null;
        }
        else
        {
            return Retinues[rd.Next(0, Retinues.Count)];
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
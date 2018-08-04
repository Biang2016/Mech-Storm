using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCardGameCommon;

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

    public void AddRetinue(CardInfo_Retinue retinueCardInfo)
    {
        AddRetinue(retinueCardInfo, Retinues.Count);
    }

    public void AddRetinue(CardInfo_Retinue retinueCardInfo, int retinuePlaceIndex)
    {
        int retinueId = ServerPlayer.MyGameManager.GeneratorNewRetinueId();
        BattleGroundAddRetinueRequest request = new BattleGroundAddRetinueRequest(ServerPlayer.ClientId, retinueCardInfo, retinuePlaceIndex, retinueId);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        ServerModuleRetinue retinue = new ServerModuleRetinue();
        retinue.M_RetinueID = retinueId;
        retinue.Initiate(retinueCardInfo, ServerPlayer);

        retinue.OnSummoned(); //先战吼，再进战场
        Retinues.Insert(retinuePlaceIndex, retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1) return;
        Retinues.Remove(retinue);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;

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

    public void KillAllInBattleGround() //杀死本方清场
    {
        for (int i = 0; i < Retinues.Count; i++)
        {
            Retinues[i].OnDieTogather();
        }

        ServerPlayer.MyGameManager.ExecuteAllSideEffects(); //触发全部死亡效果


        while (Retinues.Count > 0)
        {
            Retinues.Clear();
        }

        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
    }

    public void AddLifeForRandomRetinue(int value) //本方增加随机随从生命
    {
        ServerModuleRetinue retinue = GetRandomRetinue();
        if (retinue == null) return;
        AddLifeForSomeRetinue(retinue, value);
    }

    public void AddLifeForSomeRetinue(ServerModuleRetinue retinue, int value) //本方增加某随从生命
    {
        retinue.M_RetinueTotalLife += value;
        retinue.M_RetinueLeftLife += value;
    }

    #endregion

    #region Utils

    public ServerModuleRetinue GetRetinue(int retinueId)
    {
        foreach (ServerModuleRetinue serverModuleRetinue in Retinues)
        {
            if (serverModuleRetinue.M_RetinueID == retinueId)
            {
                return serverModuleRetinue;
            }
        }

        return null;
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
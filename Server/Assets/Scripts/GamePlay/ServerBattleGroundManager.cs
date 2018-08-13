using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCardGameCommon;

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

    public bool BattleGroundIsFull;
    public bool BattleGroundIsEmpty;
    public ServerPlayer ServerPlayer;
    private List<ServerModuleRetinue> Retinues = new List<ServerModuleRetinue>();

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
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        if (battleGroundIndex == -1) return;
        Retinues.Remove(retinue);
        RetinueCount = Retinues.Count;

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

    public void KillAllInBattleGround() //杀死本方清场
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
    }

    public void KillAllHerosInBattleGround() //杀死本方所有英雄
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (!Retinues[i].CardInfo.BattleInfo.IsSodier) dieRetinues.Add(Retinues[i]);
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

    public void KillAllSodiersInBattleGround() //杀死本方所有士兵
    {
        List<ServerModuleRetinue> dieRetinues = new List<ServerModuleRetinue>();
        for (int i = 0; i < Retinues.Count; i++)
        {
            if (Retinues[i].CardInfo.BattleInfo.IsSodier) dieRetinues.Add(Retinues[i]);
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

    public void AddLifeForRandomRetinue(int value) //增加本方随机随从生命
    {
        ServerModuleRetinue retinue = GetRandomRetinue();
        if (retinue == null) return;
        AddLifeForSomeRetinue(retinue, value);
    }

    public void AddLifeForSomeRetinue(int retinueId, int value) //本方增加某随从生命
    {
        ServerModuleRetinue retinue = GetRetinue(retinueId);
        if (retinue != null)
        {
            retinue.M_RetinueTotalLife += value;
            retinue.M_RetinueLeftLife += value;
        }
    }

    public void AddLifeForSomeRetinue(ServerModuleRetinue retinue, int value) //增加本方某随从生命
    {
        retinue.M_RetinueTotalLife += value;
        retinue.M_RetinueLeftLife += value;
    }

    public void HealSomeRetinue(int retinueId, int value) //治疗本方某随从
    {
        ServerModuleRetinue retinue = GetRetinue(retinueId);
        if (retinue != null)
        {
            int healAmount = Math.Min(value, retinue.M_RetinueTotalLife - retinue.M_RetinueLeftLife);
            retinue.M_RetinueLeftLife += healAmount;
        }
    }

    public void DamageSomeRetinue(int retinueId, int value) //对本方某随从造成伤害
    {
        ServerModuleRetinue retinue = GetRetinue(retinueId);
        retinue?.BeAttacked(value);
    }

    public void DamageRandomRetinue(int value) //对本方某随机随从造成伤害
    {
        ServerModuleRetinue targetRetinue = GetRandomRetinue();
        if (targetRetinue != null) DamageSomeRetinue(targetRetinue.M_RetinueID, value);
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
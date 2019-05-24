using System;
using System.Collections.Generic;

internal class BattleGroundManager
{
    private int mechCount;

    public int MechCount
    {
        get { return mechCount; }
        set { mechCount = value; }
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
        get { return MechCount == GamePlaySettings.MaxMechNumber; }
    }

    public bool BattleGroundIsEmpty
    {
        get { return MechCount == 0; }
    }

    public bool HerosIsEmpty
    {
        get { return HeroCount == 0; }
    }

    public bool SoldiersIsEmpty
    {
        get { return SoldierCount == 0; }
    }

    public bool HasDefenseMech
    {
        get
        {
            foreach (ModuleMech mech in Mechs)
            {
                if (mech.IsDefender) return true;
            }

            return false;
        }
    }

    public BattlePlayer ServerPlayer;
    public List<ModuleMech> Mechs = new List<ModuleMech>();
    public List<ModuleMech> Heroes = new List<ModuleMech>();
    public List<ModuleMech> Soldiers = new List<ModuleMech>();
    public HashSet<int> CanAttackMechs = new HashSet<int>();

    public BattleGroundManager(BattlePlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    #region SideEffects

    private void BattleGroundAddMech(int mechPlaceIndex, ModuleMech mech)
    {
        int aliveIndex = GetIndexOfAliveMechs(mechPlaceIndex);
        Mechs.Insert(aliveIndex, mech);
        MechCount = Mechs.Count;
        if (mech.CardInfo.MechInfo.IsSoldier)
        {
            Soldiers.Add(mech);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heroes.Add(mech);
            HeroCount = Heroes.Count;
        }
    }

    public void RemoveMechs(List<int> mechIds)
    {
        foreach (int mechId in mechIds)
        {
            ModuleMech mech = GetMech(mechId);
            if (mech != null) RemoveMech(mech);
        }
    }

    private void RemoveMech(ModuleMech mech)
    {
        int battleGroundIndex = Mechs.IndexOf(mech);
        if (battleGroundIndex == -1)
        {
            BattleLog.Instance.Log.PrintWarning("BattleGroundRemoveMech not exist mech：" + mech.M_MechID);

            return;
        }

        Mechs.Remove(mech);
        MechCount = Mechs.Count;
        if (mech.CardInfo.MechInfo.IsSoldier)
        {
            Soldiers.Remove(mech);
            SoldierCount = Soldiers.Count;
        }
        else
        {
            Heroes.Remove(mech);
            HeroCount = Heroes.Count;
        }

        if (!mech.CardInfo.BaseInfo.IsTemp) ServerPlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(mech.OriginCardInstanceId);
        mech.UnRegisterSideEffect();
    }

    private void BattleGroundRemoveAllMech()
    {
        foreach (ModuleMech mech in Mechs.ToArray())
        {
            RemoveMech(mech);
        }
    }

    public void AddMech(CardInfo_Mech mechCardInfo)
    {
        AddMech(mechCardInfo, Mechs.Count, targetMechId: Const.TARGET_MECH_SELECT_NONE, clientMechTempId: Const.CLIENT_TEMP_MECH_ID_NORMAL, handCardInstanceId: Const.CARD_INSTANCE_ID_NONE);
    }

    public void AddMech(CardInfo_Mech mechCardInfo, int mechPlaceIndex, int targetMechId, int clientMechTempId, int handCardInstanceId)
    {
        if (BattleGroundIsFull) return;
        int mechId = ServerPlayer.GameManager.GenerateNewMechId();
        BattleGroundAddMechRequest request = new BattleGroundAddMechRequest(ServerPlayer.ClientId, mechCardInfo, mechPlaceIndex, mechId, clientMechTempId);
        ServerPlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);

        ModuleMech mech = new ModuleMech();
        mech.M_MechID = mechId;
        mech.M_UsedClientMechTempId = clientMechTempId;
        mech.OriginCardInstanceId = handCardInstanceId;
        mech.Initiate(mechCardInfo, ServerPlayer);

        ServerPlayer.CardDeckManager.CardDeck.AddCardInstanceId(mechCardInfo.CardID, handCardInstanceId);

        BattleGroundAddMech(mechPlaceIndex, mech);

        ExecutorInfo info = new ExecutorInfo(clientId: ServerPlayer.ClientId, mechId: mechId, targetMechIds: new List<int> {targetMechId});
        if (mechCardInfo.MechInfo.IsSoldier) ServerPlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierSummon, info);
        else ServerPlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroSummon, info);
    }

    public void EquipWeapon(EquipWeaponRequest r, CardInfo_Base cardInfo)
    {
        ModuleWeapon weapon = new ModuleWeapon();
        CardInfo_Equip cardInfo_Weapon = (CardInfo_Equip) cardInfo;
        ModuleMech mech = GetMech(r.mechId);
        weapon.M_ModuleMech = mech;
        weapon.M_EquipID = ServerPlayer.GameManager.GenerateNewEquipId();
        weapon.Initiate(cardInfo_Weapon, ServerPlayer);
        weapon.OriginCardInstanceId = r.handCardInstanceId;
        mech.M_Weapon = weapon;
        ServerPlayer.CardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipShield(EquipShieldRequest r, CardInfo_Base cardInfo)
    {
        ModuleShield shield = new ModuleShield();
        CardInfo_Equip cardInfo_Shield = (CardInfo_Equip) cardInfo;
        ModuleMech mech = GetMech(r.mechID);
        shield.M_ModuleMech = mech;
        shield.M_EquipID = ServerPlayer.GameManager.GenerateNewEquipId();
        shield.Initiate(cardInfo_Shield, ServerPlayer);
        shield.OriginCardInstanceId = r.handCardInstanceId;
        mech.M_Shield = shield;
        ServerPlayer.CardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipPack(EquipPackRequest r, CardInfo_Base cardInfo)
    {
        ModulePack pack = new ModulePack();
        CardInfo_Equip cardInfo_Pack = (CardInfo_Equip) cardInfo;
        ModuleMech mech = GetMech(r.mechID);
        pack.M_ModuleMech = mech;
        pack.M_EquipID = ServerPlayer.GameManager.GenerateNewEquipId();
        pack.Initiate(cardInfo_Pack, ServerPlayer);
        pack.OriginCardInstanceId = r.handCardInstanceId;
        mech.M_Pack = pack;
        ServerPlayer.CardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    public void EquipMA(EquipMARequest r, CardInfo_Base cardInfo)
    {
        ModuleMA ma = new ModuleMA();
        CardInfo_Equip cardInfo_MA = (CardInfo_Equip) cardInfo;
        ModuleMech mech = GetMech(r.mechID);
        ma.M_ModuleMech = mech;
        ma.M_EquipID = ServerPlayer.GameManager.GenerateNewEquipId();
        ma.Initiate(cardInfo_MA, ServerPlayer);
        ma.OriginCardInstanceId = r.handCardInstanceId;
        mech.M_MA = ma;
        ServerPlayer.CardDeckManager.CardDeck.AddCardInstanceId(cardInfo.CardID, r.handCardInstanceId);
    }

    #region Utils

    public ModuleMech GetMech(int mechId)
    {
        foreach (ModuleMech serverModuleMech in Mechs)
        {
            if (serverModuleMech.M_MechID == mechId) return serverModuleMech;
        }

        return null;
    }

    private int GetIndexOfAliveMechs(int battleGroundIndex)
    {
        //去除掉已经死亡但还没移除战场的随从（避免服务器指针错误）
        int countDieMech = 0;
        for (int i = 0; i < battleGroundIndex; i++)
        {
            if (ServerPlayer.GameManager.DieMechList.Contains(Mechs[i].M_MechID))
            {
                countDieMech++;
            }
        }

        int aliveIndex = battleGroundIndex - countDieMech;
        return aliveIndex;
    }

    public int GetMechIdByClientMechTempId(int clientMechTempId)
    {
        foreach (ModuleMech serverModuleMech in Mechs)
        {
            if (serverModuleMech.M_UsedClientMechTempId == clientMechTempId)
            {
                return serverModuleMech.M_MechID;
            }
        }

        return -1;
    }

    public List<ModuleMech> GetMechByType(MechTypes mechType)
    {
        List<ModuleMech> mechs;
        switch (mechType)
        {
            case MechTypes.All:
                mechs = Mechs;
                break;
            case MechTypes.Soldier:
                mechs = Soldiers;
                break;
            case MechTypes.Hero:
                mechs = Heroes;
                break;
            default:
                mechs = Mechs;
                break;
        }

        return mechs;
    }

    public ModuleMech GetRandomMech(MechTypes mechType, int exceptMechId)
    {
        List<ModuleMech> mechs = GetMechByType(mechType);

        if (mechs.Count == 0)
        {
            return null;
        }
        else
        {
            int aliveCount = CountAliveMechExcept(mechType, exceptMechId);
            Random rd = new Random();
            return GameManager.GetAliveMechExcept(rd.Next(0, aliveCount), mechs, exceptMechId);
        }
    }

    public int CountAliveMechExcept(MechTypes mechType, int exceptMechId)
    {
        List<ModuleMech> mechs = GetMechByType(mechType);

        int count = 0;
        foreach (ModuleMech mech in mechs)
        {
            if (!mech.M_IsDead && mech.M_MechID != exceptMechId) count++;
        }

        return count;
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        ServerPlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnBeginRound, new ExecutorInfo(clientId: ServerPlayer.ClientId));
        foreach (ModuleMech mech in Mechs)
        {
            mech.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        ServerPlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEndRound, new ExecutorInfo(clientId: ServerPlayer.ClientId));
        foreach (ModuleMech mech in Mechs)
        {
            mech.OnEndRound();
        }

        foreach (ModuleMech mech in ServerPlayer.MyEnemyPlayer.BattleGroundManager.Mechs)
        {
            if (mech.M_ImmuneLeftRounds > 0) mech.M_ImmuneLeftRounds--;
            if (mech.M_InactivityRounds > 0) mech.M_InactivityRounds--;
        }
    }

    #endregion

    #endregion
}
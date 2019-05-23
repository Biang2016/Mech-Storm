using System.Collections.Generic;

/// <summary>
/// 为了兼容联机对战模式，单人模式的AI也用一个ClientProxy来处理逻辑
/// AI不需要任何Socket有关的功能
/// 固定PlayerA为玩家，PlayerB为AI
/// </summary>
internal class ClientProxyAI : ClientProxy
{
    public override ClientStates ClientState
    {
        get => clientState;
        set => clientState = value;
    }

    public ClientProxyAI(int clientId, bool isStopReceive) : base(null, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId, Server.ServerVersion);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    public override void SendMessage(ServerRequestBase request)
    {
        if (request is ResponseBundleBase r)
        {
            foreach (ServerRequestBase req in r.AttachedRequests)
            {
                if (req is PlayerTurnRequest ptr) //只监听回合开始
                {
                    if (ptr.clientId == ClientId)
                    {
                        AIOperation();
                    }
                }
            }
        }
    }

    public override void ReceiveMessage(ClientRequestBase request)
    {
    }

    protected override void Response()
    {
    }

    #region AIOperation

    HashSet<int> TriedCards = new HashSet<int>();
    HashSet<int> TriedMechs = new HashSet<int>();

    public void ResetTriedCards()
    {
        TriedCards.Clear();
    }

    public void AIOperation()
    {
        while (true)
        {
            bool lastOperation = false;
            bool failedAgain = false;
            while (true)
            {
                ServerCardBase card = FindCardUsable();
                ServerModuleMech mech = FindMechMovable();
                if (card == null && mech == null)
                {
                    if (!lastOperation)
                    {
                        failedAgain = true; //如果连续两次失败，则停止
                    }

                    break;
                }
                else
                {
                    if (card != null)
                    {
                        if (TryUseCard(card)) //成功
                        {
                            lastOperation = true;
                        }
                        else
                        {
                            TriedCards.Add(card.M_CardInstanceId); //尝试过的卡牌不再尝试
                        }
                    }

                    if (mech != null)
                    {
                        if (TryAttack(mech)) //成功
                        {
                            lastOperation = true;
                        }
                        else
                        {
                            TriedMechs.Add(mech.M_MechID); //尝试过的随从不再尝试
                        }
                    }
                }
            }

            TriedCards.Clear();
            TriedMechs.Clear();

            if (failedAgain) break;
        }

        MyServerGameManager.OnEndRoundRequest(new EndRoundRequest(ClientId));
    }

    /// <summary>
    /// 尝试使用卡牌，如果卡牌需要指定目标，但没有合适目标，则使用失败，返回false
    /// </summary>
    /// <returns></returns>
    private bool TryUseCard(ServerCardBase card)
    {
        TargetInfo ti = card.CardInfo.TargetInfo;
        if (card.CardInfo.BaseInfo.CardType == CardTypes.Spell || card.CardInfo.BaseInfo.CardType == CardTypes.Energy)
        {
            if (ti.HasNoTarget)
            {
                MyServerGameManager.OnClientUseSpellCardRequest(new UseSpellCardRequest(ClientId, card.M_CardInstanceId));
                return true;
            }
            else if (ti.HasTargetMech)
            {
                ServerModuleMech mech = GetTargetMechByTargetInfo(ti);
                if (mech != null)
                {
                    MyServerGameManager.OnClientUseSpellCardToMechRequest(new UseSpellCardToMechRequest(ClientId, card.M_CardInstanceId, mech.M_MechID, false, 0));
                    return true;
                }
            }
            else if (ti.HasTargetShip)
            {
                switch (ti.targetShipRange)
                {
                    case TargetRange.EnemyShip:
                    {
                        MyServerGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientId, card.M_CardInstanceId, MyServerGameManager.PlayerA.ClientId));
                        return true;
                    }
                    case TargetRange.SelfShip:
                    {
                        MyServerGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientId, card.M_CardInstanceId, ClientId));
                        return true;
                    }
                }
            }
            else if (ti.HasTargetEquip) //Todo
            {
                switch (ti.targetMechRange)
                {
                    case TargetRange.EnemyMechs:
                    {
                        break;
                    }
                    case TargetRange.EnemyHeroes:
                    {
                        break;
                    }
                    case TargetRange.EnemySoldiers:
                    {
                        break;
                    }
                    case TargetRange.SelfMechs:
                    {
                        break;
                    }
                    case TargetRange.SelfHeroes:
                    {
                        break;
                    }
                    case TargetRange.SelfSoldiers:
                    {
                        break;
                    }
                }
            }
        }
        else if (card.CardInfo.BaseInfo.CardType == CardTypes.Mech)
        {
            if (MyBattleGroundManager.BattleGroundIsFull) return false;

            bool canSummonDirectly = false;
            canSummonDirectly |= ti.HasNoTarget;
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfMechs && MyBattleGroundManager.MechCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfHeroes && MyBattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfSoldiers && MyBattleGroundManager.SoldierCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemyMechs && EnemyBattleGroundManager.MechCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemyHeroes && EnemyBattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemySoldiers && EnemyBattleGroundManager.SoldierCount == 0);
            //Todo 针对装备等还没处理

            if (canSummonDirectly)
            {
                MyServerGameManager.OnClientSummonMechRequest(new SummonMechRequest(ClientId, card.M_CardInstanceId, MyBattleGroundManager.MechCount, Const.TARGET_MECH_SELECT_NONE, false, Const.CLIENT_TEMP_MECH_ID_NORMAL));
                return true;
            }

            if (ti.HasTargetMech)
            {
                ServerModuleMech mech = GetTargetMechByTargetInfo(ti);
                if (mech != null)
                {
                    MyServerGameManager.OnClientSummonMechRequest(new SummonMechRequest(ClientId, card.M_CardInstanceId, MyBattleGroundManager.MechCount, mech.M_MechID, false, Const.CLIENT_TEMP_MECH_ID_NORMAL));
                    return true;
                }
            }
            else if (ti.HasTargetShip)
            {
                //Todo 针对战舰等还没处理
            }
            else if (ti.HasTargetEquip)
            {
                //Todo 针对装备等还没处理
            }
        }
        else if (card.CardInfo.BaseInfo.CardType == CardTypes.Equip)
        {
            ServerModuleMech mech = SelectMechToEquipWeapon(MyBattleGroundManager.Heroes, (CardInfo_Equip) card.CardInfo); //优先装备英雄
            if (mech == null) mech = SelectMechToEquipWeapon(MyBattleGroundManager.Soldiers, (CardInfo_Equip) card.CardInfo);

            if (mech != null)
            {
                switch (card.CardInfo.EquipInfo.SlotType)
                {
                    case SlotTypes.Weapon:
                    {
                        MyServerGameManager.OnClientEquipWeaponRequest(new EquipWeaponRequest(ClientId, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.Shield:
                    {
                        MyServerGameManager.OnClientEquipShieldRequest(new EquipShieldRequest(ClientId, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.Pack:
                    {
                        MyServerGameManager.OnClientEquipPackRequest(new EquipPackRequest(ClientId, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.MA:
                    {
                        MyServerGameManager.OnClientEquipMARequest(new EquipMARequest(ClientId, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool TryAttack(ServerModuleMech mech)
    {
        if (EnemyPlayer.CheckModuleMechCanAttackMe(mech)) //优先打脸
        {
            MyServerGameManager.OnClientMechAttackShipRequest(new MechAttackShipRequest(ClientId, mech.M_MechID));
            return true;
        }

        foreach (ServerModuleMech targetMech in EnemyBattleGroundManager.Mechs)
        {
            if (targetMech.CheckMechCanAttackMe(mech))
            {
                MyServerGameManager.OnClientMechAttackMechRequest(new MechAttackMechRequest(ClientId, mech.M_MechID, EnemyPlayer.ClientId, targetMech.M_MechID));
                return true;
            }
        }

        return false;
    }

    private ServerModuleMech GetTargetMechByTargetInfo(TargetInfo ti)
    {
        ServerPlayer targetPlayer = null;
        MechTypes targetMechType = MechTypes.All;
        if ((ti.targetMechRange | TargetRange.EnemyMechs) == TargetRange.EnemyMechs)
        {
            targetPlayer = MyServerGameManager.PlayerA;
        }
        else if ((ti.targetMechRange | TargetRange.SelfMechs) == TargetRange.SelfMechs)
        {
            targetPlayer = MyServerGameManager.PlayerB;
        }

        if ((ti.targetMechRange | TargetRange.Heroes) == TargetRange.Heroes)
        {
            targetMechType = MechTypes.Hero;
        }
        else if ((ti.targetMechRange | TargetRange.Soldiers) == TargetRange.Soldiers)
        {
            targetMechType = MechTypes.Soldier;
        }
        else
        {
            targetMechType = MechTypes.All;
        }

        ServerModuleMech mech = null;
        if (targetPlayer != null)
        {
            mech = targetPlayer.MyBattleGroundManager.GetRandomMech(targetMechType, -1);
        }
        else
        {
            mech = MyServerGameManager.GetRandomAliveMechExcept(targetMechType, -1);
        }

        return mech;
    }

    public bool CheckMechCanEquipMe(ServerModuleMech mech, CardInfo_Equip equipInfo)
    {
        if (MyPlayer == mech.ServerPlayer && mech.CardInfo.MechInfo.HasSlotType(equipInfo.EquipInfo.SlotType) && !mech.M_IsDead)
        {
            if (equipInfo.EquipInfo.SlotType == SlotTypes.Weapon && equipInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                if (mech.CardInfo.MechInfo.IsSniper)
                {
                    return true; //狙击枪只能装在狙击手上
                }
                else
                {
                    return false;
                }
            }
            else if (equipInfo.EquipInfo.SlotType == SlotTypes.MA)
            {
                if (mech.IsAllEquipExceptMA)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    private ServerCardBase FindCardUsable()
    {
        List<int> noTriedUsableCards = new List<int>();
        foreach (int id in MyHandManager.UsableCards)
        {
            if (!TriedCards.Contains(id))
            {
                noTriedUsableCards.Add(id);
            }
        }

        if (noTriedUsableCards.Count == 1) return MyHandManager.GetCardByCardInstanceId(noTriedUsableCards[0]);
        ServerCardBase energyCardID = null;
        ServerCardBase spellCardID = null;
        ServerCardBase equipCardID = null;
        ServerCardBase mechCardID = null;
        foreach (int id in noTriedUsableCards)
        {
            ServerCardBase card = MyHandManager.GetCardByCardInstanceId(id);
            if (card.CardInfo.BaseInfo.CardType == CardTypes.Energy)
            {
                energyCardID = card;
            }

            if (card.CardInfo.BaseInfo.CardType == CardTypes.Spell)
            {
                spellCardID = card;
            }

            if (card.CardInfo.BaseInfo.CardType == CardTypes.Equip)
            {
                equipCardID = card;
            }

            if (card.CardInfo.BaseInfo.CardType == CardTypes.Mech)
            {
                mechCardID = card;
            }
        }

        if (energyCardID != null)
        {
            return energyCardID;
        }

        if (spellCardID != null)
        {
            return spellCardID;
        }

        if (equipCardID != null)
        {
            return equipCardID;
        }

        if (mechCardID != null)
        {
            return mechCardID;
        }

        return null;
    }

    private ServerModuleMech FindMechMovable()
    {
        foreach (int id in MyBattleGroundManager.CanAttackMechs)
        {
            if (!TriedMechs.Contains(id))
            {
                return MyBattleGroundManager.GetMech(id);
            }
        }

        return null;
    }

    private bool TryAttack()
    {
        return false;
    }

    #endregion

    #region AI_Mark_System 评分系统

    private ServerModuleMech SelectMechToEquipWeapon(List<ServerModuleMech> mechs, CardInfo_Equip cardInfo)
    {
        ServerModuleMech res = null;
        List<ServerModuleMech> mechs_NoWeapon = new List<ServerModuleMech>(); //优先给没有武器的装备

        List<ServerModuleMech> optionalMech = new List<ServerModuleMech>();
        foreach (ServerModuleMech mech in mechs) //满足可以装备的前提
        {
            if (CheckMechCanEquipMe(mech, cardInfo))
            {
                optionalMech.Add(mech);
            }
        }

        foreach (ServerModuleMech mech in optionalMech)
        {
            if (mech.M_Weapon == null)
            {
                mechs_NoWeapon.Add(mech);
            }
        }

        if (mechs_NoWeapon.Count != 0) //没有武器的里面，挑最强的
        {
            return GetMechByEvaluation(mechs_NoWeapon, EvaluationOption.Mech, EvaluationDirection.Max);
        }
        else //都有武器情况下，给武器最差的装备
        {
            return GetMechByEvaluation(mechs_NoWeapon, EvaluationOption.Weapon, EvaluationDirection.Min);
        }
    }

    enum EvaluationOption
    {
        Mech,
        Weapon,
        Shield,
        Pack,
        MA
    }

    enum EvaluationDirection
    {
        Max,
        Min
    }

    delegate float GetMechByEvaluationDelegate(ServerModuleMech mech);

    Dictionary<EvaluationOption, GetMechByEvaluationDelegate> EvaluationMethodDict = new Dictionary<EvaluationOption, GetMechByEvaluationDelegate>
    {
        {EvaluationOption.Mech, CountMechValue},
        {EvaluationOption.Weapon, CountMechWeaponValue},
        {EvaluationOption.Shield, CountMechShieldValue},
        {EvaluationOption.Pack, CountMechPackValue},
        {EvaluationOption.MA, CountMechMAValue}
    };

    private ServerModuleMech GetMechByEvaluation(List<ServerModuleMech> optionalMech, EvaluationOption evaluationOption, EvaluationDirection evaluationDirection)
    {
        ServerModuleMech res = null;
        float resMard = 0;
        foreach (ServerModuleMech mech in optionalMech)
        {
            float mark = EvaluationMethodDict[evaluationOption](mech);

            if (evaluationDirection == EvaluationDirection.Max)
            {
                if (mark > resMard)
                {
                    res = mech;
                    resMard = mark;
                }
            }
            else
            {
                if (mark < resMard)
                {
                    res = mech;
                    resMard = mark;
                }
            }
        }

        return res;
    }

    private static float CountMechValue(ServerModuleMech mech)
    {
        float mark = 0;

        mark += mech.CardInfo.BaseInfo.BaseValue();
        mark += mech.M_MechLeftLife;
        mark += mech.M_MechShield * 3 + mech.M_MechArmor;

        mark += CountMechWeaponValue(mech);
        mark += CountMechShieldValue(mech);
        mark += CountMechPackValue(mech);
        mark += CountMechMAValue(mech);

        return mark;
    }

    private static float CountMechWeaponValue(ServerModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Weapon != null)
        {
            mark += mech.M_Weapon.CardInfo.BaseInfo.BaseValue();
            mark += mech.M_MechWeaponEnergy * mech.M_MechAttack;
        }

        return mark;
    }

    private static float CountMechShieldValue(ServerModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Shield != null)
        {
            mark += mech.M_Shield.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountMechPackValue(ServerModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Pack != null)
        {
            mark += mech.M_Pack.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountMechMAValue(ServerModuleMech mech)
    {
        float mark = 0;

        if (mech.M_MA != null)
        {
            mark += mech.M_MA.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    #endregion
}
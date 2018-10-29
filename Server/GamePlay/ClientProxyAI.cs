using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Net.Sockets;
using System.Threading;

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
        ClientIdRequest request = new ClientIdRequest(clientId);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    public override void SendMessage(ServerRequestBase request)
    {
        if (request is ResponseBundleBase r)
        {
            foreach (ServerRequestBase req in r.AttachedRequests)
            {
                if (req is PlayerTurnRequest ptr)
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

    private void AIOperation()
    {
        while (true)
        {
            ServerCardBase card = FindCardUsable();
            if (card == null)
            {
                break;
            }
            else
            {
                if (!TryUseCard(card))
                {
                    TriedCards.Add(card.M_CardInstanceId);
                }
            }
        }

        TriedCards.Clear();
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
            else if (ti.HasTargetRetinue)
            {
                ServerModuleRetinue retinue = GetTargetRetinueByTargetInfo(ti);
                if (retinue != null)
                {
                    MyServerGameManager.OnClientUseSpellCardToRetinueRequest(new UseSpellCardToRetinueRequest(ClientId, card.M_CardInstanceId, retinue.M_RetinueID, false, 0));
                    return true;
                }
            }
            else if (ti.HasTargetShip)
            {
                switch (ti.targetShipRange)
                {
                    case TargetSideEffect.TargetRange.EnemyShip:
                    {
                        MyServerGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientId, card.M_CardInstanceId, MyServerGameManager.PlayerA.ClientId));
                        return true;
                    }
                    case TargetSideEffect.TargetRange.SelfShip:
                    {
                        MyServerGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientId, card.M_CardInstanceId, ClientId));
                        return true;
                    }
                }
            }
            else if (ti.HasTargetEquip) //Todo
            {
                switch (ti.targetRetinueRange)
                {
                    case TargetSideEffect.TargetRange.EnemyMechs:
                    {
                        break;
                    }
                    case TargetSideEffect.TargetRange.EnemyHeros:
                    {
                        break;
                    }
                    case TargetSideEffect.TargetRange.EnemySoldiers:
                    {
                        break;
                    }
                    case TargetSideEffect.TargetRange.SelfMechs:
                    {
                        break;
                    }
                    case TargetSideEffect.TargetRange.SelfHeros:
                    {
                        break;
                    }
                    case TargetSideEffect.TargetRange.SelfSoldiers:
                    {
                        break;
                    }
                }
            }
        }
        else if (card.CardInfo.BaseInfo.CardType == CardTypes.Retinue)
        {
            if (MyBattleGroundManager.BattleGroundIsFull) return false;

            bool canSummonDirectly = false;
            canSummonDirectly |= ti.HasNoTarget;
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.SelfMechs && MyBattleGroundManager.RetinueCount == 0);
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.SelfHeros && MyBattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.SelfSoldiers && MyBattleGroundManager.SoldierCount == 0);
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.EnemyMechs && EnemyBattleGroundManager.RetinueCount == 0);
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.EnemyHeros && EnemyBattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetRetinueRange == TargetSideEffect.TargetRange.EnemySoldiers && EnemyBattleGroundManager.SoldierCount == 0);
            //Todo 针对装备等还没处理

            if (canSummonDirectly)
            {
                MyServerGameManager.OnClientSummonRetinueRequest(new SummonRetinueRequest(ClientId, card.M_CardInstanceId, MyBattleGroundManager.RetinueCount, Const.TARGET_RETINUE_SELECT_NONE, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL));
                return true;
            }

            if (ti.HasTargetRetinue)
            {
                ServerModuleRetinue retinue = GetTargetRetinueByTargetInfo(ti);
                if (retinue != null)
                {
                    MyServerGameManager.OnClientSummonRetinueRequest(new SummonRetinueRequest(ClientId, card.M_CardInstanceId, MyBattleGroundManager.RetinueCount, retinue.M_RetinueID, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL));
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
            ServerModuleRetinue retinue = SelectRetinueToEquipWeapon(MyBattleGroundManager.Heroes, (CardInfo_Equip) card.CardInfo); //优先装备英雄
            if (retinue == null) retinue = SelectRetinueToEquipWeapon(MyBattleGroundManager.Soldiers, (CardInfo_Equip) card.CardInfo);

            if (retinue != null)
            {
                switch (card.CardInfo.EquipInfo.SlotType)
                {
                    case SlotTypes.Weapon:
                    {
                        MyServerGameManager.OnClientEquipWeaponRequest(new EquipWeaponRequest(ClientId, card.M_CardInstanceId, retinue.M_RetinueID));
                        return true;
                    }
                    case SlotTypes.Shield:
                    {
                        MyServerGameManager.OnClientEquipShieldRequest(new EquipShieldRequest(ClientId, card.M_CardInstanceId, retinue.M_RetinueID));
                        return true;
                    }
                    case SlotTypes.Pack:
                    {
                        MyServerGameManager.OnClientEquipPackRequest(new EquipPackRequest(ClientId, card.M_CardInstanceId, retinue.M_RetinueID));
                        return true;
                    }
                    case SlotTypes.MA:
                    {
                        MyServerGameManager.OnClientEquipMARequest(new EquipMARequest(ClientId, card.M_CardInstanceId, retinue.M_RetinueID));
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private ServerModuleRetinue GetTargetRetinueByTargetInfo(TargetInfo ti)
    {
        ServerPlayer targetPlayer = null;
        ServerBattleGroundManager.RetinueType targetRetinueType = ServerBattleGroundManager.RetinueType.All;
        if ((ti.targetRetinueRange | TargetSideEffect.TargetRange.EnemyMechs) == TargetSideEffect.TargetRange.EnemyMechs)
        {
            targetPlayer = MyServerGameManager.PlayerA;
        }
        else if ((ti.targetRetinueRange | TargetSideEffect.TargetRange.SelfMechs) == TargetSideEffect.TargetRange.SelfMechs)
        {
            targetPlayer = MyServerGameManager.PlayerB;
        }

        if ((ti.targetRetinueRange | TargetSideEffect.TargetRange.Heros) == TargetSideEffect.TargetRange.Heros)
        {
            targetRetinueType = ServerBattleGroundManager.RetinueType.Hero;
        }
        else if ((ti.targetRetinueRange | TargetSideEffect.TargetRange.Soldiers) == TargetSideEffect.TargetRange.Soldiers)
        {
            targetRetinueType = ServerBattleGroundManager.RetinueType.Soldier;
        }
        else
        {
            targetRetinueType = ServerBattleGroundManager.RetinueType.All;
        }

        ServerModuleRetinue retinue = null;
        if (targetPlayer != null)
        {
            retinue = targetPlayer.MyBattleGroundManager.GetRandomRetinue(targetRetinueType, -1);
        }
        else
        {
            retinue = MyServerGameManager.GetRandomAliveRetinueExcept(targetRetinueType, -1);
        }

        return retinue;
    }

    public bool CheckRetinueCanEquipMe(ServerModuleRetinue retinue, CardInfo_Equip equipInfo)
    {
        if (MyPlayer == retinue.ServerPlayer && retinue.CardInfo.RetinueInfo.HasSlotType(equipInfo.EquipInfo.SlotType) && !retinue.M_IsDead)
        {
            if (equipInfo.EquipInfo.SlotType == SlotTypes.Weapon && equipInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                if (retinue.CardInfo.RetinueInfo.IsSniper)
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
                if (retinue.IsAllEquipExceptMA)
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
        foreach (ServerCardBase card in MyHandManager.Cards)
        {
            if (!TriedCards.Contains(card.M_CardInstanceId))
            {
                if (card.Usable) return card;
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

    private ServerModuleRetinue SelectRetinueToEquipWeapon(List<ServerModuleRetinue> retinues, CardInfo_Equip cardInfo)
    {
        ServerModuleRetinue res = null;
        List<ServerModuleRetinue> retinues_NoWeapon = new List<ServerModuleRetinue>(); //优先给没有武器的装备

        List<ServerModuleRetinue> optionalRetinue = new List<ServerModuleRetinue>();
        foreach (ServerModuleRetinue retinue in retinues) //满足可以装备的前提
        {
            if (CheckRetinueCanEquipMe(retinue, cardInfo))
            {
                optionalRetinue.Add(retinue);
            }
        }

        foreach (ServerModuleRetinue retinue in optionalRetinue)
        {
            if (retinue.M_Weapon == null)
            {
                retinues_NoWeapon.Add(retinue);
            }
        }

        if (retinues_NoWeapon.Count != 0) //没有武器的里面，挑最强的
        {
            return GetRetinueByEvaluation(retinues_NoWeapon, EvaluationOption.Retinue, EvaluationDirection.Max);
        }
        else //都有武器情况下，给武器最差的装备
        {
            return GetRetinueByEvaluation(retinues_NoWeapon, EvaluationOption.Weapon, EvaluationDirection.Min);
        }
    }


    enum EvaluationOption
    {
        Retinue,
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

    delegate float GetRetinueByEvaluationDelegate(ServerModuleRetinue retinue);

    Dictionary<EvaluationOption, GetRetinueByEvaluationDelegate> EvaluationMethodDict = new Dictionary<EvaluationOption, GetRetinueByEvaluationDelegate>
    {
        {EvaluationOption.Retinue, CountRetinueValue},
        {EvaluationOption.Weapon, CountRetinueWeaponValue},
        {EvaluationOption.Shield, CountRetinueShieldValue},
        {EvaluationOption.Pack, CountRetinuePackValue},
        {EvaluationOption.MA, CountRetinueMAValue}
    };


    private ServerModuleRetinue GetRetinueByEvaluation(List<ServerModuleRetinue> optionalRetinue, EvaluationOption evaluationOption, EvaluationDirection evaluationDirection)
    {
        ServerModuleRetinue res = null;
        float resMard = 0;
        foreach (ServerModuleRetinue retinue in optionalRetinue)
        {
            float mark = EvaluationMethodDict[evaluationOption](retinue);

            if (evaluationDirection == EvaluationDirection.Max)
            {
                if (mark > resMard)
                {
                    res = retinue;
                    resMard = mark;
                }
            }
            else
            {
                if (mark < resMard)
                {
                    res = retinue;
                    resMard = mark;
                }
            }
        }

        return res;
    }


    private static float CountRetinueValue(ServerModuleRetinue retinue)
    {
        float mark = 0;

        mark += retinue.CardInfo.BaseInfo.BaseValue();
        mark += retinue.M_RetinueLeftLife;
        mark += retinue.M_RetinueShield * 3 + retinue.M_RetinueArmor;

        mark += CountRetinueWeaponValue(retinue);
        mark += CountRetinueShieldValue(retinue);
        mark += CountRetinuePackValue(retinue);
        mark += CountRetinueMAValue(retinue);

        return mark;
    }

    private static float CountRetinueWeaponValue(ServerModuleRetinue retinue)
    {
        float mark = 0;
        if (retinue.M_Weapon != null)
        {
            mark += retinue.M_Weapon.CardInfo.BaseInfo.BaseValue();
            mark += retinue.M_RetinueWeaponEnergy * retinue.M_RetinueAttack;
        }

        return mark;
    }

    private static float CountRetinueShieldValue(ServerModuleRetinue retinue)
    {
        float mark = 0;
        if (retinue.M_Shield != null)
        {
            mark += retinue.M_Shield.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountRetinuePackValue(ServerModuleRetinue retinue)
    {
        float mark = 0;
        if (retinue.M_Pack != null)
        {
            mark += retinue.M_Pack.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountRetinueMAValue(ServerModuleRetinue retinue)
    {
        float mark = 0;

        if (retinue.M_MA != null)
        {
            mark += retinue.M_MA.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    #endregion
}
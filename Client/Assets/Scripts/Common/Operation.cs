using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Operation
{
    public ClientPlayer BeginClientPlayer; //发起人
    public int BeginID; //发起物品ID
    public List<int> TargetIDs; //目标ID，支持多个
    public OperationType M_OperationType;

    public Operation(ClientPlayer beginClientPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType)
    {
        BeginClientPlayer = beginClientPlayer;
        BeginID = beginID;
        TargetIDs = targetIDs;
        M_OperationType = m_OperationType;
    }
}

public class OperationSummon : Operation
{
    public int CardID;

    public OperationSummon(ClientPlayer beginClientPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType, int cardID) : base(beginClientPlayer, beginID, targetIDs, m_OperationType)
    {
        CardID = cardID;
    }
}

public class OperationnAttack : Operation
{
    public List<int> AttackNumbers; //每个TargetID对应一个AttackNumber
    public List<int> ActualAttackNumbers; //实际伤害

    public OperationnAttack(ClientPlayer beginClientPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType, List<int> attackNumbers, List<int> actualAttackNumbers) : base(beginClientPlayer, beginID, targetIDs, m_OperationType)
    {
        AttackNumbers = attackNumbers;
        ActualAttackNumbers = actualAttackNumbers;
    }
}

public class OperationHeal : Operation
{
    public List<int> HealNumbers; //每个TargetID对应一个HealNumber
    public List<int> ActualHealNumbers; //实际治疗量

    public OperationHeal(ClientPlayer beginClientPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType, List<int> healNumbers, List<int> actualHealNumbers) : base(beginClientPlayer, beginID, targetIDs, m_OperationType)
    {
        HealNumbers = healNumbers;
        ActualHealNumbers = actualHealNumbers;
    }
}

public class OperationEquip : Operation
{
    public int CardID;
    public CardTypes M_CardType;
    public ModuleRetinue TargetModuleRetinue;

    public OperationEquip(ClientPlayer beginClientPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType, int cardID, CardTypes m_CardType, ModuleRetinue targetModuleRetinue) : base(beginClientPlayer, beginID, targetIDs, m_OperationType)
    {
        CardID = cardID;
        M_CardType = m_CardType;
        TargetModuleRetinue = targetModuleRetinue;
    }
}

//public class OperationUpgrade : Operation
//{
//    public CardTypes M_CardType;
//    public ModuleRetinue TargetModuleRetinue;

//    public OperationUpgrade(Player beginPlayer, int beginID, List<int> targetIDs, OperationType m_OperationType, CardTypes m_CardType, ModuleRetinue targetModuleRetinue) : base(beginPlayer, beginID, targetIDs, m_OperationType)
//    {
//        M_CardType = m_CardType;
//        TargetModuleRetinue = targetModuleRetinue;
//    }
//}

public enum OperationType
{
    Attack = 0,
    Heal = 1,
    Equip = 2,
    Summon = 3,
    Upgrade = 4
}
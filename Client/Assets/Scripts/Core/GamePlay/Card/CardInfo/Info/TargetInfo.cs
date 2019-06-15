﻿/// <summary>
/// 记录出牌时是否需要指定目标
/// </summary>
public struct TargetInfo
{
    public bool HasTargetMech;
    public bool HasTargetEquip;
    public bool HasTargetShip;

    public bool HasNoTarget
    {
        get { return !HasTargetMech && !HasTargetEquip && !HasTargetShip; }
    }

    public TargetRange targetMechRange;
    public TargetRange targetEquipRange;
    public TargetRange targetShipRange;

    public void Initialize(CardInfo_Base CardInfo)
    {
        FindTargetInSideEffectBundle(CardInfo.SideEffectBundle, CardInfo);
    }

    private void FindTargetInSideEffectBundle(SideEffectBundle seb, CardInfo_Base CardInfo)
    {
        foreach (SideEffectExecute see in seb.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnPlayCard, SideEffectExecute.TriggerRange.Self))
        {
            if (FindTarget(see)) break;
        }

        foreach (SideEffectExecute see in seb.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnMechSummon, SideEffectExecute.TriggerRange.Self))
        {
            if (FindTarget(see)) break;
        }
    }

    bool FindTarget(SideEffectExecute see)
    {
        foreach (SideEffectBase se in see.SideEffectBases)
        {
            if (se is TargetSideEffect tse && tse.IsNeedChoice)
            {
                if (tse is TargetSideEffectEquip && ((TargetSideEffectEquip) tse).IsNeedChoice)
                {
                    HasTargetEquip = true;
                    targetEquipRange = ((TargetSideEffectEquip) tse).TargetRange;
                    return true;
                }
                else
                {
                    TargetRange temp = tse.TargetRange;
                    if ((temp & TargetRange.Ships) == TargetRange.None)
                    {
                        HasTargetMech = true;
                        targetMechRange = tse.TargetRange;
                        return true;
                    }
                    else
                    {
                        HasTargetShip = true;
                        targetShipRange = tse.TargetRange;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
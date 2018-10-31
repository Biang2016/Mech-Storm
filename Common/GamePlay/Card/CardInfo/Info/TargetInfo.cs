using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 记录出牌时是否需要指定目标
/// </summary>
public struct TargetInfo
{
    public bool HasTargetRetinue;
    public bool HasTargetEquip;
    public bool HasTargetShip;

    public bool HasNoTarget
    {
        get { return !HasTargetRetinue && !HasTargetEquip && !HasTargetShip; }
    }

    public TargetSideEffect.TargetRange targetRetinueRange;
    public TargetSideEffect.TargetRange targetEquipRange;
    public TargetSideEffect.TargetRange targetShipRange;

    public void Initialize(CardInfo_Base CardInfo)
    {
        FindTargetInSideEffectBundle(CardInfo.SideEffectBundle, CardInfo);
        FindTargetInSideEffectBundle(CardInfo.SideEffectBundle_OnBattleGround, CardInfo);
    }

    private void FindTargetInSideEffectBundle(SideEffectBundle seb, CardInfo_Base CardInfo)
    {
        foreach (SideEffectExecute see in seb.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnPlayCard, SideEffectBundle.TriggerRange.Self))
        {
            if (FindTarget(see)) break;
        }

        foreach (SideEffectExecute see in seb.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnRetinueSummon, SideEffectBundle.TriggerRange.Self))
        {
            if (FindTarget(see)) break;
        }
    }

    bool FindTarget(SideEffectExecute see)
    {
        SideEffectBase se = see.SideEffectBase;
        if (se is TargetSideEffect tse && tse.IsNeedChoise)
        {
            if (tse is TargetSideEffectEquip && ((TargetSideEffectEquip) tse).IsNeedChoise)
            {
                HasTargetEquip = true;
                targetEquipRange = ((TargetSideEffectEquip) tse).M_TargetRange;
                return true;
            }
            else
            {
                TargetSideEffect.TargetRange temp = tse.M_TargetRange;
                if ((temp & TargetSideEffect.TargetRange.Ships) == TargetSideEffect.TargetRange.None)
                {
                    HasTargetRetinue = true;
                    targetRetinueRange = tse.M_TargetRange;
                    return true;
                }
                else
                {
                    HasTargetShip = true;
                    targetShipRange = tse.M_TargetRange;
                    return true;
                }
            }
        }

        return false;
    }
}
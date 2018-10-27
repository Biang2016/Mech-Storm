using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnPlayCard, SideEffectBundle.TriggerRange.Self))
        {
            SideEffectBase se = see.SideEffectBase;
            if (se is TargetSideEffect && ((TargetSideEffect) se).IsNeedChoise)
            {
                if (se is TargetSideEffectEquip && ((TargetSideEffectEquip) se).IsNeedChoise)
                {
                    HasTargetEquip = true;
                    targetEquipRange = ((TargetSideEffectEquip) se).M_TargetRange;
                    break;
                }
                else
                {
                    TargetSideEffect.TargetRange temp = ((TargetSideEffect) se).M_TargetRange;
                    if (temp != TargetSideEffect.TargetRange.Ships && temp != TargetSideEffect.TargetRange.SelfShip && temp != TargetSideEffect.TargetRange.EnemyShip && temp != TargetSideEffect.TargetRange.AllLife)
                    {
                        HasTargetRetinue = true;
                        targetRetinueRange = ((TargetSideEffect) se).M_TargetRange;
                        break;
                    }
                    else
                    {
                        HasTargetShip = true;
                        targetShipRange = ((TargetSideEffect) se).M_TargetRange;
                        break;
                    }
                }
            }
        }
    }
}
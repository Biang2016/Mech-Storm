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
            SideEffectBase se = see.SideEffectBase;
            if (se is TargetSideEffect equip && equip.IsNeedChoise)
            {
                if (equip is TargetSideEffectEquip && ((TargetSideEffectEquip) equip).IsNeedChoise)
                {
                    HasTargetEquip = true;
                    targetEquipRange = ((TargetSideEffectEquip) equip).M_TargetRange;
                    break;
                }
                else
                {
                    TargetSideEffect.TargetRange temp = equip.M_TargetRange;
                    if ((temp & TargetSideEffect.TargetRange.Ships) == TargetSideEffect.TargetRange.None)
                    {
                        HasTargetRetinue = true;
                        targetRetinueRange = equip.M_TargetRange;
                        break;
                    }
                    else
                    {
                        HasTargetShip = true;
                        targetShipRange = equip.M_TargetRange;
                        break;
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;

public abstract class TargetSideEffect : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_ConstInt("TargetSelect", (int) TargetSelect.All, typeof(TargetSelect));
        M_SideEffectParam.SetParam_ConstInt("TargetRange", (int) TargetRange.None, typeof(TargetRange));
        M_SideEffectParam.SetParam_ConstInt("ChoiceCount", 1);
    }

    public virtual List<TargetSelect> ValidTargetSelects
    {
        get
        {
            List<TargetSelect> res = new List<TargetSelect>();
            foreach (KeyValuePair<TargetSelect, List<TargetRange>> kv in TargetSelector.TargetSelectorPresets[TargetSelectorType])
            {
                res.Add(kv.Key);
            }

            return res;
        }
    }

    public bool IsNeedChoice
    {
        get
        {
            if (TargetSelectorType == TargetSelector.TargetSelectorTypes.EveryMechBased) return false;
            if (TargetSelect == TargetSelect.Multiple) return true;
            if (TargetSelect == TargetSelect.Single)
            {
                if (TargetRange == TargetRange.SelfShip || TargetRange == TargetRange.EnemyShip || TargetRange == TargetRange.Self || TargetRange == TargetRange.SelfDeck || TargetRange == TargetRange.EnemyDeck)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }

    public int ChoiceCount
    {
        get { return M_SideEffectParam.GetParam_ConstInt("ChoiceCount"); }
    }

    public abstract TargetSelector.TargetSelectorTypes TargetSelectorType { get; }

    public TargetRange TargetRange
    {
        get { return (TargetRange) M_SideEffectParam.GetParam_ConstInt("TargetRange"); }
    }

    public TargetSelect TargetSelect
    {
        get { return (TargetSelect) M_SideEffectParam.GetParam_ConstInt("TargetSelect"); }
    }

    public string GetDescOfTargetRange()
    {
        return TargetSelector.GetDescOfTargetSelector(TargetRange, TargetSelect, ChoiceCount);
    }
}
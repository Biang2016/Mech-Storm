using System.Collections.Generic;

public abstract class TargetSideEffect : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_Bool("IsNeedChoice", false);
        M_SideEffectParam.SetParam_ConstInt("ChoiceCount", 1);
        M_SideEffectParam.SetParam_ConstInt("TargetSelectorType", (int) TargetSelectorType, typeof(TargetSelector.TargetSelectorTypes));
        M_SideEffectParam.SetParam_ConstInt("TargetRange", (int) TargetRange.None, typeof(TargetRange));
        M_SideEffectParam.SetParam_ConstInt("TargetSelect", (int) TargetSelect.All, typeof(TargetSelect));
    }

    public bool IsNeedChoice
    {
        get { return M_SideEffectParam.GetParam_Bool("IsNeedChoice"); }
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
        return TargetSelector.GetDescOfTargetSelector(TargetRange, TargetSelect);
    }
}
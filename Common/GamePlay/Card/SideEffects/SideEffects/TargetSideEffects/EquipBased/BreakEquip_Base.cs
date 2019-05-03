using System.Collections.Generic;

public class BreakEquip_Base : TargetSideEffectEquip
{
    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
    }

    public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.EquipBased;
    public override List<TargetSelect> ValidTargetSelects => new List<TargetSelect> {TargetSelect.Single, TargetSelect.SingleRandom};
}
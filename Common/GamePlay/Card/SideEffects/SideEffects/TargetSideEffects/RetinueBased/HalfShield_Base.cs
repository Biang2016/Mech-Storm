public class HalfShield_Base : TargetSideEffect
{
    public override TargetSelector.TargetSelectorTypes TargetSelectorType => TargetSelector.TargetSelectorTypes.RetinueBased;

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], GetDescOfTargetRange());
    }
}
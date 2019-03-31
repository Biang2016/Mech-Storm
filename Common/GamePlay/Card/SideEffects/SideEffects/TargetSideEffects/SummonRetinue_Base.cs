public class SummonRetinue_Base : TargetSideEffect
{
    public int RetinueID;
    public int SummonRetinueID;

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(SummonRetinueID).BaseInfo;
        return HightlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(SummonRetinueID);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        RetinueID = reader.ReadSInt32();
        SummonRetinueID = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((SummonRetinue_Base) copy).RetinueID = RetinueID;
        ((SummonRetinue_Base) copy).SummonRetinueID = SummonRetinueID;
    }
}
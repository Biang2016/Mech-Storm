public class SummonRetinue_Base : TargetSideEffect
{
    
    
    public int SummonRetinueID;

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(SummonRetinueID).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], bi.CardNames[LanguageManager_Common.GetCurrentLanguage()]);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(SummonRetinueID);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        SummonRetinueID = reader.ReadSInt32();
    }
}
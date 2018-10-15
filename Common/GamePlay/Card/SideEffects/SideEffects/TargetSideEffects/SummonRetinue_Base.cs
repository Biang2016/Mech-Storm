public class SummonRetinue_Base : TargetSideEffect
{
    public int RetinueID;
    public int SummonRetinueID;

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(SummonRetinueID).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, isEnglish ? bi.CardName_en : bi.CardName);
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

    public override int CalculateDamage()
    {
        return 0;
    }

    public override int CalculateHeal()
    {
        return 0;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((SummonRetinue_Base) copy).RetinueID = RetinueID;
        ((SummonRetinue_Base) copy).SummonRetinueID = SummonRetinueID;
    }
}
public class SummonRetinue_Base : SideEffectBase
{
    public int RetinueID;
    public int SummonRetinueID;

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(SummonRetinueID).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, isEnglish ? bi.CardName_en : bi.CardName);
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(RetinueID);
        writer.WriteSInt32(SummonRetinueID);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
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
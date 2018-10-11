public class AddTempCardToDeck_Base : SideEffectBase, IEffectFactor
{
    public int Value;
    public int CardId;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(CardId).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, FinalValue, "[" + (isEnglish ? bi.CardName_en : bi.CardName) + "]");
    }

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(Value);
        writer.WriteSInt32(CardId);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        Value = reader.ReadSInt32();
        CardId = reader.ReadSInt32();
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddTempCardToDeck_Base) copy).Value = Value;
        ((AddTempCardToDeck_Base) copy).CardId = CardId;
        ((AddTempCardToDeck_Base) copy).Factor = Factor;
    }
}
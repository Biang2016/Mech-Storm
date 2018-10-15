public class DrawTypeCards_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public int Value;
    public int Factor = 1;
    public CardTypes DrawCardType = CardTypes.Energy;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, FinalValue, isEnglish ? BaseInfo.CardTypeNameDict_en[DrawCardType] : BaseInfo.CardTypeNameDict[DrawCardType], FinalValue <= 1 ? "" : "s");
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
        writer.WriteSInt32((int) DrawCardType);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value = reader.ReadSInt32();
        DrawCardType = (CardTypes) reader.ReadSInt32();
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DrawTypeCards_Base) copy).Value = Value;
        ((DrawTypeCards_Base) copy).Factor = Factor;
        ((DrawTypeCards_Base) copy).DrawCardType = DrawCardType;
    }
}
public class DrawTypeCards_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public int Value;
    private int factor = 1;

    public int GetFactor()
    {
        return factor;
    }

    public void SetFactor(int value)
    {
        factor = value;
    }

    public CardTypes DrawCardType = CardTypes.Energy;

    public int FinalValue
    {
        get { return Value * GetFactor(); }
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

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DrawTypeCards_Base) copy).Value = Value;
        ((DrawTypeCards_Base) copy).SetFactor(GetFactor());
        ((DrawTypeCards_Base) copy).DrawCardType = DrawCardType;
    }
}
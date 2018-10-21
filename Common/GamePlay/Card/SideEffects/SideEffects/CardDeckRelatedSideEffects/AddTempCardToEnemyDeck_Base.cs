public class AddTempCardToEnemyDeck_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public int Value;
    public int CardId;
    private int factor = 1;

    public int GetFactor()
    {
        return factor;
    }

    public void SetFactor(int value)
    {
        factor = value;
    }
    public int FinalValue
    {
        get { return Value * GetFactor(); }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        BaseInfo bi = AllCards.GetCard(CardId).BaseInfo;
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, FinalValue, "[" + (isEnglish ? bi.CardName_en : bi.CardName) + "]");
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
        writer.WriteSInt32(CardId);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value = reader.ReadSInt32();
        CardId = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddTempCardToEnemyDeck_Base) copy).Value = Value;
        ((AddTempCardToEnemyDeck_Base) copy).CardId = CardId;
        ((AddTempCardToEnemyDeck_Base) copy).SetFactor(GetFactor());
    }
}
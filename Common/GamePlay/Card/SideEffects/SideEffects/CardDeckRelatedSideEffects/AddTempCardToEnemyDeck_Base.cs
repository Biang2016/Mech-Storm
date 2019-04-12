using System.Collections.Generic;

public class AddTempCardToEnemyDeck_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public SideEffectValue Value = new SideEffectValue(0);
    private int factor = 1;

    public override List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

    public int CardId;

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
        get { return Value.Value * GetFactor(); }
    }

    public override string GenerateDesc()
    {
        BaseInfo bi = AllCards.GetCard(CardId).BaseInfo;
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], FinalValue, "[" + bi.CardNames[LanguageManager_Common.GetCurrentLanguage()] + "]");
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value.Value);
        writer.WriteSInt32(CardId);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value.Value = reader.ReadSInt32();
        CardId = reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddTempCardToEnemyDeck_Base) copy).Value = Value.Clone();
        ((AddTempCardToEnemyDeck_Base) copy).CardId = CardId;
        ((AddTempCardToEnemyDeck_Base) copy).SetFactor(GetFactor());
    }
}
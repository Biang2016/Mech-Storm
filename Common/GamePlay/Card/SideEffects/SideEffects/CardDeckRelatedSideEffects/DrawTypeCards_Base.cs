using System.Collections.Generic;

public class DrawTypeCards_Base : CardDeckRelatedSideEffects, IEffectFactor
{
    public SideEffectValue Value = new SideEffectValue(0);
    private int factor = 1;

    public List<SideEffectValue> Values
    {
        get { return new List<SideEffectValue> {Value}; }
    }

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
        get { return Value.Value * GetFactor(); }
    }

    public override string GenerateDesc()
    {
        return HighlightStringFormat(DescRaws[LanguageManager_Common.GetCurrentLanguage()], FinalValue, BaseInfo.CardTypeNameDict[LanguageManager_Common.GetCurrentLanguage()][DrawCardType], FinalValue <= 1 ? "" : "s");
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value.Value);
        writer.WriteSInt32((int) DrawCardType);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value.Value = reader.ReadSInt32();
        DrawCardType = (CardTypes) reader.ReadSInt32();
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DrawTypeCards_Base) copy).Value = Value.Clone();
        ((DrawTypeCards_Base) copy).SetFactor(GetFactor());
        ((DrawTypeCards_Base) copy).DrawCardType = DrawCardType;
    }
}
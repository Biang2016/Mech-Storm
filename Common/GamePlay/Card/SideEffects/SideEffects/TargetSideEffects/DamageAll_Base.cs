﻿public class DamageAll_Base : TargetSideEffect, IEffectFactor, IDamage
{
    public int Value;
    public int Factor = 1;

    public int FinalValue
    {
        get { return Value * Factor; }
    }

    public override string GenerateDesc(bool isEnglish)
    {
        return HightlightStringFormat(isEnglish ? DescRaw_en : DescRaw, GetChineseDescOfTargetRange(M_TargetRange, isEnglish, true, false), FinalValue);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Value = reader.ReadSInt32();
    }

    public int CalculateDamage()
    {
        return FinalValue;
    }

    public void SetEffetFactor(int factor)
    {
        Factor = factor;
    }


    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((DamageAll_Base) copy).Value = Value;
        ((DamageAll_Base) copy).Factor = Factor;
    }
}
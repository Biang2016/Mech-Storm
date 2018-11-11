using System.Collections.Generic;

/// <summary>
/// 卡牌属性倍率
/// </summary>
public interface IEffectFactor
{
    List<SideEffectValue> Values { get; }
    int GetFactor();
    void SetFactor(int value);
}

public class SideEffectValue
{
    public int Value;

    public SideEffectValue(int value)
    {
        Value = value;
    }

    public SideEffectValue Clone()
    {
        return new SideEffectValue(Value);
    }
}
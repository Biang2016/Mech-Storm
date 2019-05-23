using System.Collections.Generic;

/// <summary>
/// 给奖励和关卡难度调幅
/// </summary>
public interface IHardFactor
{
    List<HardFactorValue> Values { get; }
    int GetFactor();
    void SetFactor(int value);
}

public class HardFactorValue
{
    public int Value;

    public HardFactorValue(int value)
    {
        Value = value;
    }

    public HardFactorValue Clone()
    {
        return new HardFactorValue(Value);
    }
}
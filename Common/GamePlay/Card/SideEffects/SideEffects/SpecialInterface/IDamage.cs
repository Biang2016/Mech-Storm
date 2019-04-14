/// <summary>
/// 带伤害的效果
/// </summary>
public interface IDamage
{
    int CalculateDamage();
    IDamageType IDamageType { get; }
}

public enum IDamageType
{
    UnknownValue,
    Known
}
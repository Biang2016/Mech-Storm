using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// 带伤害的效果
/// </summary>
public interface IDamage
{
    int CalculateDamage();
    IDamageType IDamageType { get; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum IDamageType
{
    UnknownValue,
    Known
}
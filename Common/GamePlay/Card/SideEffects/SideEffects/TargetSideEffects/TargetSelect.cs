using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum TargetSelect
{
    Single,
    SingleRandom,
    Multiple,
    MultipleRandom,
    All,
}
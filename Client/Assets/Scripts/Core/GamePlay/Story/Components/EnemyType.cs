using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum EnemyType
{
    Soldier,
    Elite,
    Boss,
}
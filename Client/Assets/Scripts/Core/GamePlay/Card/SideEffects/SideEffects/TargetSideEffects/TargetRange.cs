using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum TargetRange
{
    AllLife = Mechs | Ships,
    SelfLife = SelfMechs | SelfShip,
    EnemyLife = EnemyMechs | EnemyShip,

    Mechs = SelfMechs | EnemyMechs,
    SelfMechs = SelfHeroes | SelfSoldiers,
    EnemyMechs = EnemyHeroes | EnemySoldiers,

    Heroes = SelfHeroes | EnemyHeroes,
    SelfHeroes = 1,
    EnemyHeroes = 2,

    Soldiers = SelfSoldiers | EnemySoldiers,
    SelfSoldiers = 4,
    EnemySoldiers = 8,

    Ships = SelfShip | EnemyShip,
    SelfShip = 16,
    EnemyShip = 32,

    Decks = SelfDeck | EnemyDeck,
    SelfDeck = 64,
    EnemyDeck = 128,

    Self = 256, //该物体自身，如出牌效果、战吼、亡语等
    None = 0,
}

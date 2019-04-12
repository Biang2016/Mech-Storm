using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class TargetSideEffect : SideEffectBase
{
    public bool IsNeedChoice;
    public TargetRange M_TargetRange; //限定范围

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        if (IsNeedChoice)
        {
            writer.WriteByte(0x01);
        }
        else writer.WriteByte(0x00);

        writer.WriteSInt32((int) M_TargetRange);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        IsNeedChoice = reader.ReadByte() == 0x01;
        M_TargetRange = (TargetRange) reader.ReadSInt32();
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetRange
    {
        AllLife = SelfLife | EnemyLife,
        SelfLife = SelfMechs | SelfShip,
        EnemyLife = EnemyMechs | EnemyShip,

        Mechs = SelfMechs | EnemyMechs,
        SelfMechs = SelfHeroes | SelfSoldiers,
        EnemyMechs = EnemyHeros | EnemySoldiers,

        Heroes = SelfHeroes | EnemyHeros,
        SelfHeroes = 1,
        EnemyHeros = 2,

        Soldiers = SelfSoldiers | EnemySoldiers,
        SelfSoldiers = 4,
        EnemySoldiers = 8,

        Ships = SelfShip | EnemyShip,
        SelfShip = 16,
        EnemyShip = 32,
        Self = 64, //该物体自身，如出牌效果、战吼、亡语等
        None = 0,
    }

    public string GetChineseDescOfTargetRange(TargetRange targetRange, bool isMulti, bool isRandom)
    {
        string textKey = "TargetRange_" + (isMulti ? "" : "Single_") + (isRandom ? "Random_" : "") + targetRange;
        return LanguageManager_Common.GetText(textKey);
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((TargetSideEffect) copy).IsNeedChoice = IsNeedChoice;
        ((TargetSideEffect) copy).M_TargetRange = M_TargetRange;
    }
}
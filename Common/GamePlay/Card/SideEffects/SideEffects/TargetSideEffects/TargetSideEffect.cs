using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class TargetSideEffect : SideEffectBase
{
    protected override void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_Bool("IsNeedChoice", false);
        M_SideEffectParam.SetParam_ConstInt("M_TargetRange", (int) TargetRange.None, typeof(TargetRange));
    }

    public bool IsNeedChoice
    {
        get { return M_SideEffectParam.GetParam_Bool("IsNeedChoice"); }
    }

    public TargetRange M_TargetRange
    {
        get { return (TargetRange) M_SideEffectParam.GetParam_ConstInt("M_TargetRange"); }
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

    public string GetDescOfTargetRange(TargetRange targetRange, bool isMulti, bool isRandom)
    {
        string textKey = "TargetRange_" + (isMulti ? "" : "Single_") + (isRandom ? "Random_" : "") + targetRange;
        return LanguageManager_Common.GetText(textKey);
    }
}
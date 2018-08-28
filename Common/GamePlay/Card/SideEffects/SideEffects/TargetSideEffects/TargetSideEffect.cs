using System;

public class TargetSideEffect : SideEffectBase
{
    public bool IsNeedChoise;
    public int TargetRetinueId;
    public TargetRange M_TargetRange; //限定范围

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        if (IsNeedChoise)
        {
            writer.WriteByte(0x01);
            writer.WriteSInt32(TargetRetinueId);
        }
        else writer.WriteByte(0x00);

        writer.WriteSInt32((int) M_TargetRange);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        IsNeedChoise = reader.ReadByte() == 0x01;
        if (IsNeedChoise) TargetRetinueId = reader.ReadSInt32();
        M_TargetRange = (TargetRange) reader.ReadSInt32();
    }

    [Flags]
    public enum TargetRange
    {
        BattleGrounds,
        SelfBattleGround,
        EnemyBattleGround,
        Heros,
        SelfHeros,
        EnemyHeros,
        Soldiers,
        SelfSoldiers,
        EnemySoldiers,
        Ships,
        SelfShip,
        EnemyShip,
        All,
        None,
    }

    public string GetChineseDescOfTargetRange(TargetRange targetRange)
    {
        switch (targetRange)
        {
            case TargetRange.BattleGrounds:
                return "机甲";
            case TargetRange.SelfBattleGround:
                return "我方机甲";
            case TargetRange.EnemyBattleGround:
                return "敌方机甲";
            case TargetRange.Heros:
                return "英雄";
            case TargetRange.SelfHeros:
                return "我方英雄";
            case TargetRange.EnemyHeros:
                return "敌方英雄";
            case TargetRange.Soldiers:
                return "士兵";
            case TargetRange.SelfSoldiers:
                return "我方士兵";
            case TargetRange.EnemySoldiers:
                return "敌方士兵";
            case TargetRange.SelfShip:
                return "我方飞船";
            case TargetRange.EnemyShip:
                return "敌方飞船";
            case TargetRange.Ships:
                return "飞船";
            case TargetRange.All:
                return "角色";
            default:
                return "";
        }
    }
}
using System;

public abstract class TargetSideEffect : SideEffectBase
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

    public string GetChineseDescOfTargetRange(TargetRange targetRange, bool isEnglish)
    {
        switch (targetRange)
        {
            case TargetRange.BattleGrounds:
                return isEnglish ? "Mech " : "机甲";
            case TargetRange.SelfBattleGround:
                return isEnglish ? "Your Mech " : "我方机甲";
            case TargetRange.EnemyBattleGround:
                return isEnglish ? "Enemy's Mech " : "敌方机甲";
            case TargetRange.Heros:
                return isEnglish ? "HeroMech " : "英雄";
            case TargetRange.SelfHeros:
                return isEnglish ? "Your HeroMech " : "我方英雄";
            case TargetRange.EnemyHeros:
                return isEnglish ? "Enemy's HeroMech " : "敌方英雄";
            case TargetRange.Soldiers:
                return isEnglish ? "Your SoldierMech" : "士兵";
            case TargetRange.SelfSoldiers:
                return isEnglish ? "Your SoldierMech " : "我方士兵";
            case TargetRange.EnemySoldiers:
                return isEnglish ? "Enemy's SoldierMech " : "敌方士兵";
            case TargetRange.SelfShip:
                return isEnglish ? "Your Spaceship " : "我方飞船";
            case TargetRange.EnemyShip:
                return isEnglish ? "Enemy's Spaceship " : "敌方飞船";
            case TargetRange.Ships:
                return isEnglish ? "Spaceship " : "飞船";
            case TargetRange.All:
                return isEnglish ? "Target " : "角色";
            default:
                return "";
        }
    }

    public abstract int CalculateDamage();
    public abstract int CalculateHeal();

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((TargetSideEffect) copy).IsNeedChoise = IsNeedChoise;
        ((TargetSideEffect) copy).TargetRetinueId = TargetRetinueId;
        ((TargetSideEffect) copy).M_TargetRange = M_TargetRange;
    }
}
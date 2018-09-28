using System;
using System.Collections.Generic;

public abstract class TargetSideEffect : SideEffectBase
{
    public bool IsNeedChoise;
    public TargetRange M_TargetRange; //限定范围

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        if (IsNeedChoise)
        {
            writer.WriteByte(0x01);
        }
        else writer.WriteByte(0x00);

        writer.WriteSInt32((int) M_TargetRange);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        IsNeedChoise = reader.ReadByte() == 0x01;
        M_TargetRange = (TargetRange) reader.ReadSInt32();
    }

    [Flags]
    public enum TargetRange
    {
        Mechs,
        SelfMechs,
        EnemyMechs,
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
        Self,
        None,
    }

    public HashSet<TargetRange> TR_Ships=new HashSet<TargetRange>{TargetRange.Ships,TargetRange.SelfShip,TargetRange.EnemyShip,TargetRange.All};
    public HashSet<TargetRange> TR_Retinues=new HashSet<TargetRange>{
        TargetRange.Mechs,
        TargetRange.SelfMechs,
        TargetRange.EnemyMechs,
        TargetRange.Heros,
        TargetRange.SelfHeros,
        TargetRange.EnemyHeros,
        TargetRange.Soldiers,
        TargetRange.SelfSoldiers,
        TargetRange.EnemySoldiers,
        TargetRange.All
    };

    public string GetChineseDescOfTargetRange(TargetRange targetRange, bool isEnglish, bool isMulti, bool isRandom)
    {
        if (!isMulti)
        {
            if (!isRandom)
            {
                switch (targetRange)
                {
                    case TargetRange.Mechs:
                        return isEnglish ? "A Mech " : "一个机甲";
                    case TargetRange.SelfMechs:
                        return isEnglish ? "One of Your Mechs " : "一个我方机甲";
                    case TargetRange.EnemyMechs:
                        return isEnglish ? "One of Enemy's Mechs " : "一个敌方机甲";
                    case TargetRange.Heros:
                        return isEnglish ? "One HeroMech " : "一个英雄";
                    case TargetRange.SelfHeros:
                        return isEnglish ? "One of Your HeroMechs " : "一个我方英雄";
                    case TargetRange.EnemyHeros:
                        return isEnglish ? "One of Enemy's HeroMechs " : "一个敌方英雄";
                    case TargetRange.Soldiers:
                        return isEnglish ? "One SoldierMech " : "一个士兵";
                    case TargetRange.SelfSoldiers:
                        return isEnglish ? "One of Your SoldierMechs " : "一个我方士兵";
                    case TargetRange.EnemySoldiers:
                        return isEnglish ? "One of Enemy's SoldierMechs " : "一个敌方士兵";
                    case TargetRange.SelfShip:
                        return isEnglish ? "Your Spaceship " : "我方战舰";
                    case TargetRange.EnemyShip:
                        return isEnglish ? "Enemy's Spaceship " : "敌方战舰";
                    case TargetRange.Ships:
                        return isEnglish ? "A Spaceship " : "一个战舰";
                    case TargetRange.All:
                        return isEnglish ? "A Target " : "一个角色";
                    case TargetRange.Self:
                        return isEnglish ? "Self " : "自身";
                    default:
                        return "";
                }
            }
            else
            {
                switch (targetRange)
                {
                    case TargetRange.Mechs:
                        return isEnglish ? "A Random Mech " : "一个随机机甲";
                    case TargetRange.SelfMechs:
                        return isEnglish ? "A Random Mech of Yours " : "一个随机我方机甲";
                    case TargetRange.EnemyMechs:
                        return isEnglish ? "A Random Mech of Enemy's " : "一个随机敌方机甲";
                    case TargetRange.Heros:
                        return isEnglish ? "A Random HeroMech " : "一个随机英雄";
                    case TargetRange.SelfHeros:
                        return isEnglish ? "A Random HeroMech of Yours " : "一个随机我方英雄";
                    case TargetRange.EnemyHeros:
                        return isEnglish ? "A Random HeroMech of Enemy's " : "一个随机敌方英雄";
                    case TargetRange.Soldiers:
                        return isEnglish ? "A Random SoldierMech " : "一个随机士兵";
                    case TargetRange.SelfSoldiers:
                        return isEnglish ? "A Random SoldierMech of Yours " : "一个随机我方士兵";
                    case TargetRange.EnemySoldiers:
                        return isEnglish ? "A Random SoldierMech of Enemy's " : "一个随机敌方士兵";
                    case TargetRange.SelfShip:
                        return isEnglish ? "Your Spaceship " : "我方战舰";
                    case TargetRange.EnemyShip:
                        return isEnglish ? "Enemy's Spaceship " : "敌方战舰";
                    case TargetRange.Ships:
                        return isEnglish ? "A Random Spaceship " : "一个随机战舰";
                    case TargetRange.All:
                        return isEnglish ? "A Random Target " : "一个随机角色";
                    case TargetRange.Self:
                        return isEnglish ? "Self " : "自身";
                    default:
                        return "";
                }
            }
        }
        else
        {
            switch (targetRange)
            {
                case TargetRange.Mechs:
                    return isEnglish ? "All Mechs " : "所有机甲";
                case TargetRange.SelfMechs:
                    return isEnglish ? "All Your Mechs " : "所有我方机甲";
                case TargetRange.EnemyMechs:
                    return isEnglish ? "All Enemy's Mechs " : "所有敌方机甲";
                case TargetRange.Heros:
                    return isEnglish ? "All HeroMech " : "所有英雄";
                case TargetRange.SelfHeros:
                    return isEnglish ? "All Your HeroMechs " : "所有我方英雄";
                case TargetRange.EnemyHeros:
                    return isEnglish ? "All Enemy's HeroMechs " : "所有敌方英雄";
                case TargetRange.Soldiers:
                    return isEnglish ? "All Your SoldierMechs " : "所有士兵";
                case TargetRange.SelfSoldiers:
                    return isEnglish ? "All Your SoldierMechs " : "所有我方士兵";
                case TargetRange.EnemySoldiers:
                    return isEnglish ? "All Enemy's SoldierMechs " : "所有敌方士兵";
                case TargetRange.SelfShip:
                    return isEnglish ? "Your Spaceship " : "我方战舰";
                case TargetRange.EnemyShip:
                    return isEnglish ? "Enemy's Spaceship " : "敌方战舰";
                case TargetRange.Ships:
                    return isEnglish ? "All Spaceships " : "所有战舰";
                case TargetRange.All:
                    return isEnglish ? "All Targets " : "所有角色";
                case TargetRange.Self:
                    return isEnglish ? "Self " : "自身";
                default:
                    return "";
            }
        }
    }

    public abstract int CalculateDamage();
    public abstract int CalculateHeal();

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((TargetSideEffect) copy).IsNeedChoise = IsNeedChoise;
        ((TargetSideEffect) copy).M_TargetRange = M_TargetRange;
    }
}
using System;
using System.Collections.Generic;

public class SideEffectBase : ICloneable
{
    public Player Player;

    public int SideEffectID;
    public string Name;
    public string DescRaw;

    public string HightlightColor;

    public SideEffectBase()
    {
    }

    public SideEffectBase(int sideEffectID, string name, string desc)
    {
        SideEffectID = sideEffectID;
        Name = name;
        DescRaw = desc;
    }

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    //序列化时无视player，也就是说效果是无关玩家的
    public virtual void Serialze(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(SideEffectID);
        writer.WriteString8(Name);
        writer.WriteString8(DescRaw);
    }

    public static SideEffectBase BaseDeserialze(DataStream reader)
    {
        string type = reader.ReadString8();
        SideEffectBase se = SideEffectManager.GetNewSideEffec(type);
        se.Deserialze(reader);
        return se;
    }

    protected virtual void Deserialze(DataStream reader)
    {
        SideEffectID = reader.ReadSInt32();
        Name = reader.ReadString8();
        DescRaw = reader.ReadString8();
    }

    public virtual string GenerateDesc()
    {
        return "";
    }

    public virtual void Excute(object Player)
    {
    }

    public static string HightlightStringFormat(string hightlightColor, string src, params object[] args)
    {
        string[] colorStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            colorStrings[i] = "<color=\"" + hightlightColor + "\">" + args[i].ToString() + "</color>";
        }

        return String.Format(src, colorStrings);
    }

    public enum TriggerTime
    {
        OnBeginRound,
        OnSelfBeginRound,
        OnEnemyBeginRound,

        OnDrawCard,
        OnSelfDrawCard,
        OnEnemyDrawCard,

        OnPlayCard,
        OnSelfPlayCard,
        OnEnemyPlayCard,

        OnRetinueSummon,
        OnSelfRetinueSummon,
        OnEnemyRetinueSummon,

        OnHeroSummon,
        OnSelfHeroSummon,
        OnEnemyHeroSummon,

        OnSoldierSummon,
        OnSelfSoldierSummon,
        OnEnemySoldierSummon,

        OnRetinueAttack,
        OnSelfRetinueAttack,
        OnEnemyRetinueAttack,
        OnThisRetinueAttack,

        OnHeroAttack,
        OnSelfHeroAttack,
        OnEnemyHeroAttack,

        OnSoldierAttack,
        OnSelfSoldierAttack,
        OnEnemySoldierAttack,

        OnRetinueInjured,
        OnSelfRetinueInjured,
        OnEnemyRetinueInjured,
        OnThisRetinueInjured,

        OnHeroInjured,
        OnSelfHeroInjured,
        OnEnemyHeroInjured,

        OnSoldierInjured,
        OnSelfSoldierInjured,
        OnEnemySoldierInjured,

        OnRetinueDie,
        OnSelfRetinueDie,
        OnEnemyRetinueDie,

        OnHeroDie,
        OnSelfHeroDie,
        OnEnemyHeroDie,

        OnSoldierDie,
        OnSelfSoldierDie,
        OnEnemySoldierDie,

        OnEndRound,
        OnSelfEndRound,
        OnEnemyEndRound,

        OnPlayThisCard,
        OnThisSummon,
        OnThisRetinueDie,
    }

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc = new SortedDictionary<TriggerTime, string>
    {
        {TriggerTime.OnBeginRound, "任一方回合开始时,"},
        {TriggerTime.OnSelfBeginRound, "我方回合开始时,"},
        {TriggerTime.OnEnemyBeginRound, "敌方回合开始时,"},

        {TriggerTime.OnDrawCard, "任一方抽牌时,"},
        {TriggerTime.OnSelfDrawCard, "我方抽牌时,"},
        {TriggerTime.OnEnemyDrawCard, "敌方抽牌时,"},

        {TriggerTime.OnPlayCard, "任一方出牌时,"},
        {TriggerTime.OnSelfPlayCard, "我方出牌时,"},
        {TriggerTime.OnEnemyPlayCard, "敌方出牌时,"},

        {TriggerTime.OnRetinueSummon, "任一方召唤机甲时,"},
        {TriggerTime.OnSelfRetinueSummon, "我方召唤机甲时,"},
        {TriggerTime.OnEnemyRetinueSummon, "敌方召唤机甲时,"},

        {TriggerTime.OnHeroSummon, "任一方召唤英雄时,"},
        {TriggerTime.OnSelfHeroSummon, "我方召唤英雄时,"},
        {TriggerTime.OnEnemyHeroSummon, "敌方召唤英雄时,"},

        {TriggerTime.OnSoldierSummon, "任一方召唤士兵时,"},
        {TriggerTime.OnSelfSoldierSummon, "我方召唤士兵时,"},
        {TriggerTime.OnEnemySoldierSummon, "敌方召唤士兵时,"},

        {TriggerTime.OnRetinueAttack, "当一个机甲进攻时,"},
        {TriggerTime.OnSelfRetinueAttack, "当我方机甲进攻时,"},
        {TriggerTime.OnEnemyRetinueAttack, "当敌方机甲进攻时,"},
        {TriggerTime.OnThisRetinueAttack, "当该机甲进攻时,"},

        {TriggerTime.OnHeroAttack, "当一个英雄进攻时,"},
        {TriggerTime.OnSelfHeroAttack, "当我方英雄进攻时,"},
        {TriggerTime.OnEnemyHeroAttack, "当敌方英雄进攻时,"},

        {TriggerTime.OnSoldierAttack, "当一个士兵进攻时,"},
        {TriggerTime.OnSelfSoldierAttack, "当我方士兵进攻时,"},
        {TriggerTime.OnEnemySoldierAttack, "当敌方士兵进攻时,"},

        {TriggerTime.OnRetinueInjured, "当一个机甲受损时,"},
        {TriggerTime.OnSelfRetinueInjured, "当我方机甲受损时,"},
        {TriggerTime.OnEnemyRetinueInjured, "当敌方机甲受损时,"},
        {TriggerTime.OnThisRetinueInjured, "当该机甲受损时,"},

        {TriggerTime.OnHeroInjured, "当一个英雄受损时,"},
        {TriggerTime.OnSelfHeroInjured, "当我方英雄受损时,"},
        {TriggerTime.OnEnemyHeroInjured, "当敌方英雄受损时,"},

        {TriggerTime.OnSoldierInjured, "当一个士兵受损时,"},
        {TriggerTime.OnSelfSoldierInjured, "当我方士兵受损时,"},
        {TriggerTime.OnEnemySoldierInjured, "当敌方士兵受损时,"},

        {TriggerTime.OnRetinueDie, "当一个机甲死亡时,"},
        {TriggerTime.OnSelfRetinueDie, "当我方机甲死亡时,"},
        {TriggerTime.OnEnemyRetinueDie, "当敌方机甲死亡时,"},

        {TriggerTime.OnHeroDie, "当一个英雄死亡时,"},
        {TriggerTime.OnSelfHeroDie, "当我方英雄死亡时,"},
        {TriggerTime.OnEnemyHeroDie, "当敌方英雄死亡时,"},

        {TriggerTime.OnSoldierDie, "当一个士兵死亡时,"},
        {TriggerTime.OnSelfSoldierDie, "当我方士兵死亡时,"},
        {TriggerTime.OnEnemySoldierDie, "当敌方士兵死亡时,"},

        {TriggerTime.OnEndRound, "任一方回合结束时,"},
        {TriggerTime.OnSelfEndRound, "我方回合结束时,"},
        {TriggerTime.OnEnemyEndRound, "敌方回合结束时,"},

        {TriggerTime.OnPlayThisCard, ""},
        {TriggerTime.OnThisSummon, "战吼:"},
        {TriggerTime.OnThisRetinueDie, "亡语:"},
    };
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// 一大捆SideEffectExecute，用于附在卡牌、随从、武器、战舰上
/// </summary>
public class SideEffectBundle
{
    public List<SideEffectExecute> SideEffectExecutes = new List<SideEffectExecute>();
    public SortedDictionary<TriggerTime, Dictionary<TriggerRange, List<SideEffectExecute>>> SideEffectExecutes_Dict = new SortedDictionary<TriggerTime, Dictionary<TriggerRange, List<SideEffectExecute>>>();

    public void AddSideEffectExecute(SideEffectExecute see)
    {
        SideEffectExecutes.Add(see);

        if (!SideEffectExecutes_Dict.ContainsKey(see.TriggerTime)) SideEffectExecutes_Dict.Add(see.TriggerTime, new Dictionary<TriggerRange, List<SideEffectExecute>>());
        Dictionary<TriggerRange, List<SideEffectExecute>> des = SideEffectExecutes_Dict[see.TriggerTime];
        if (!des.ContainsKey(see.TriggerRange)) des.Add(see.TriggerRange, new List<SideEffectExecute>());
        List<SideEffectExecute> sees = SideEffectExecutes_Dict[see.TriggerTime][see.TriggerRange];
        sees.Add(see);
    }

    public List<SideEffectExecute> GetSideEffectExecutes(TriggerTime triggerTime, TriggerRange triggerRange)
    {
        List<SideEffectExecute> res = new List<SideEffectExecute>();
        if (SideEffectExecutes_Dict.ContainsKey(triggerTime))
        {
            var temp = SideEffectExecutes_Dict[triggerTime];
            if (temp.ContainsKey(triggerRange))
            {
                res.AddRange(temp[triggerRange]);
            }
        }

        return res;
    }

    public string GetSideEffectsDesc(bool isEnglish)
    {
        string res = "";
        foreach (KeyValuePair<TriggerTime, Dictionary<TriggerRange, List<SideEffectExecute>>> kv in SideEffectExecutes_Dict)
        {
            foreach (KeyValuePair<TriggerRange, List<SideEffectExecute>> SEEs in kv.Value)
            {
                if (SEEs.Value.Count > 0)
                {
                    if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroSummon || kv.Key == TriggerTime.OnRetinueSummon || kv.Key == TriggerTime.OnSoldierSummon)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Battlecry: " : "战吼: ");
                    else if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroDie || kv.Key == TriggerTime.OnRetinueDie || kv.Key == TriggerTime.OnSoldierDie)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Die: " : "亡语: ");
                    else if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnEquipDie)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Broken: " : "亡语: ");
                    else if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnRetinueAttack)) res += isEnglish ? "When attacks, " : "进攻时, ";
                    else if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnRetinueKill)) res += isEnglish ? "When kills, " : "杀敌时, ";
                    else if (SEEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnPlayCard))
                    {
                        foreach (SideEffectExecute see in SEEs.Value)
                        {
                            if (see.SideEffectBase is Exile_Base)
                            {
                                res = BaseInfo.AddImportantColorToText(isEnglish ? "Disposable. " : "移除, ") + res;
                            }
                        }

                        res += "";
                    }
                    else
                    {
                        if (isEnglish)
                        {
                            res += string.Format(TriggerTimeDesc_en[kv.Key], BaseInfo.AddHightLightColorToText(TriggerRangeDesc_en[SEEs.Key]));
                        }
                        else
                        {
                            res += string.Format(TriggerTimeDesc[kv.Key], BaseInfo.AddHightLightColorToText(TriggerRangeDesc[SEEs.Key]));
                        }
                    }

                    foreach (SideEffectExecute see in SEEs.Value)
                    {
                        res += see.SideEffectBase.GenerateDesc(isEnglish);
                    }
                }
            }

            res = res.TrimEnd(",.;/n ".ToCharArray());
            res += "; ";
        }

        return res;
    }

    public SideEffectBundle Clone()
    {
        SideEffectBundle copy = new SideEffectBundle();
        foreach (SideEffectExecute see in SideEffectExecutes)
        {
            copy.AddSideEffectExecute(see.Clone());
        }

        return copy;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(SideEffectExecutes.Count);
        foreach (SideEffectExecute see in SideEffectExecutes)
        {
            see.Serialize(writer);
        }
    }

    public static SideEffectBundle Deserialze(DataStream reader)
    {
        SideEffectBundle res = new SideEffectBundle();
        int SideEffectCount = reader.ReadSInt32();
        for (int i = 0; i < SideEffectCount; i++)
        {
            SideEffectExecute see = SideEffectExecute.Deserialze(reader);
            res.AddSideEffectExecute(see);
        }

        return res;
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerTime
    {
        None = 0,

        OnTrigger = 1 << 0, //例如：某buff为【下一次造成的伤害翻倍】，需要在造成伤害的技能之前触发，且修改该技能的伤害值。

        OnBeginRound = 1 << 1,
        OnDrawCard = 1 << 2,
        OnPlayCard = 1 << 3,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueSummon = OnHeroSummon | OnSoldierSummon,
        OnHeroSummon = 1 << 4,
        OnSoldierSummon = 1 << 5,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueAttack = OnHeroAttack | OnSoldierAttack,
        OnHeroAttack = 1 << 6,
        OnSoldierAttack = 1 << 7,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueInjured = OnHeroInjured | OnSoldierInjured,
        OnHeroInjured = 1 << 8,
        OnSoldierInjured = 1 << 9,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueKill = OnHeroKill | OnSoldierKill,
        OnHeroKill = 1 << 10,
        OnSoldierKill = 1 << 11,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueMakeDamage = OnHeroMakeDamage | OnSoldierMakeDamage,
        OnHeroMakeDamage = 1 << 12,
        OnSoldierMakeDamage = 1 << 13,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueBeHealed = OnHeroBeHealed | OnSoldierBeHealed,
        OnHeroBeHealed = 1 << 14,
        OnSoldierBeHealed = 1 << 15,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnRetinueDie = OnHeroDie | OnSoldierDie,
        OnHeroDie = 1 << 16,
        OnSoldierDie = 1 << 17,

        OnEquipDie = 1 << 18,

        /// <summary>
        /// Don't Invoke，otherwise it'll trigger multiple times.
        /// </summary>
        OnMakeDamage = OnMakeSpellDamage | OnRetinueMakeDamage,
        OnMakeSpellDamage = 1 << 19,

        OnPlayerGetEnergy = 1 << 21,
        OnPlayerUseEnergy = 1 << 22,
        OnPlayerAddLife = 1 << 23,
        OnPlayerLostLife = 1 << 24,
        OnEndRound = 1 << 25,

        OnUseMetal = 1 << 26,
    }

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc = new SortedDictionary<TriggerTime, string>
    {
        {TriggerTime.OnTrigger, "{0}"},

        {TriggerTime.OnBeginRound, "{0}回合开始时, "},
        {TriggerTime.OnDrawCard, "{0}抽牌时, "},
        {TriggerTime.OnPlayCard, "{0}出牌时, "},

        {TriggerTime.OnRetinueSummon, "{0}召唤机甲时, "},
        {TriggerTime.OnHeroSummon, "{0}召唤英雄时, "},
        {TriggerTime.OnSoldierSummon, "{0}召唤士兵时, "},

        {TriggerTime.OnRetinueAttack, "当{0}机甲进攻时, "},
        {TriggerTime.OnHeroAttack, "当{0}英雄进攻时, "},
        {TriggerTime.OnSoldierAttack, "当{0}士兵进攻时, "},

        {TriggerTime.OnRetinueInjured, "当{0}机甲受损时, "},
        {TriggerTime.OnHeroInjured, "当{0}英雄受损时, "},
        {TriggerTime.OnSoldierInjured, "当{0}士兵受损时, "},

        {TriggerTime.OnRetinueKill, "当{0}机甲成功击杀, "},
        {TriggerTime.OnHeroKill, "当{0}英雄成功击杀, "},
        {TriggerTime.OnSoldierKill, "当{0}士兵成功击杀, "},

        {TriggerTime.OnRetinueMakeDamage, "当{0}机甲造成伤害时, "},
        {TriggerTime.OnHeroMakeDamage, "当{0}英雄造成伤害时, "},
        {TriggerTime.OnSoldierMakeDamage, "当{0}士兵造成伤害时, "},

        {TriggerTime.OnRetinueBeHealed, "当{0}机甲得到修复时, "},
        {TriggerTime.OnHeroBeHealed, "当{0}英雄得到修复时, "},
        {TriggerTime.OnSoldierBeHealed, "当{0}士兵得到修复时, "},

        {TriggerTime.OnRetinueDie, "当{0}机甲死亡时, "},
        {TriggerTime.OnHeroDie, "当{0}英雄死亡时, "},
        {TriggerTime.OnSoldierDie, "当{0}士兵死亡时, "},

        {TriggerTime.OnEquipDie, "{0}装备破坏时, "},

        {TriggerTime.OnMakeDamage, "{0}造成伤害时, "},
        {TriggerTime.OnMakeSpellDamage, "{0}造成法术伤害时, "},

        {TriggerTime.OnPlayerGetEnergy, "当{0}获得能量时, "},
        {TriggerTime.OnPlayerUseEnergy, "当{0}消耗能量时, "},
        {TriggerTime.OnPlayerAddLife, "当{0}获得生命时, "},
        {TriggerTime.OnPlayerLostLife, "当{0}生命减少时, "},
        {TriggerTime.OnEndRound, "{0}回合结束时, "},

        {TriggerTime.OnUseMetal, "{0}消耗金属时, "},
    };

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc_en = new SortedDictionary<TriggerTime, string>
    {
        {TriggerTime.OnTrigger, "{0}"},

        {TriggerTime.OnBeginRound, "When {0} turn starts, "},
        {TriggerTime.OnDrawCard, "Once {0} draws, "},
        {TriggerTime.OnPlayCard, "Once {0} plays a card, "},

        {TriggerTime.OnRetinueSummon, "When {0} Mech summoned, "},
        {TriggerTime.OnHeroSummon, "When {0} HeroMech summoned, "},
        {TriggerTime.OnSoldierSummon, "When {0} SoldierMech summoned, "},

        {TriggerTime.OnRetinueAttack, "When {0} Mech attacks, "},
        {TriggerTime.OnHeroAttack, "When {0} HeroMech attacks, "},
        {TriggerTime.OnSoldierAttack, "When {0} SoldierMech attacks, "},

        {TriggerTime.OnRetinueInjured, "When {0} Mech damaged, "},
        {TriggerTime.OnHeroInjured, "When {0} HeroMech damaged, "},
        {TriggerTime.OnSoldierInjured, "When {0} SoldierMech damaged, "},

        {TriggerTime.OnRetinueKill, "When {0} Mech kill enemy, "},
        {TriggerTime.OnHeroKill, "When {0} HeroMech kill enemy, "},
        {TriggerTime.OnSoldierKill, "When {0} SoldierMech kill enemy, "},

        {TriggerTime.OnRetinueMakeDamage, "When {0} Mech make damage, "},
        {TriggerTime.OnHeroMakeDamage, "When {0} HeroMech make damage, "},
        {TriggerTime.OnSoldierMakeDamage, "When {0} SoldierMech make damage, "},

        {TriggerTime.OnRetinueBeHealed, "When {0} Mech is healed, "},
        {TriggerTime.OnHeroBeHealed, "When {0} HeroMech is healed, "},
        {TriggerTime.OnSoldierBeHealed, "When {0} SoldierMech is healed, "},

        {TriggerTime.OnRetinueDie, "When {0} Mech died, "},
        {TriggerTime.OnHeroDie, "When {0} HeroMech died, "},
        {TriggerTime.OnSoldierDie, "When {0} SoldierMech died, "},

        {TriggerTime.OnEquipDie, "When {0} Mech's Equipment broken, "},

        {TriggerTime.OnMakeDamage, "When {0} deal damage, "},
        {TriggerTime.OnMakeSpellDamage, "When {0} deal spell damage, "},

        {TriggerTime.OnPlayerGetEnergy, "When {0} get energy, "},
        {TriggerTime.OnPlayerUseEnergy, "When {0} consume energy, "},
        {TriggerTime.OnPlayerAddLife, "When {0} get healed, "},
        {TriggerTime.OnPlayerLostLife, "When {0} lost life, "},
        {TriggerTime.OnEndRound, "When {0} turn ends, "},

        {TriggerTime.OnUseMetal, "When {0} consume metal, "},
    };

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerRange
    {
        None,

        SelfPlayer,
        EnemyPlayer,
        OnePlayer,
        One,
        SelfAnother,
        Another,
        Attached,
        Self,
    }

    public static SortedDictionary<TriggerRange, string> TriggerRangeDesc = new SortedDictionary<TriggerRange, string>
    {
        {TriggerRange.SelfPlayer, "我方"},
        {TriggerRange.EnemyPlayer, "敌方"},
        {TriggerRange.OnePlayer, "任一方"},
        {TriggerRange.One, "一个"},
        {TriggerRange.SelfAnother, "我方其他"},
        {TriggerRange.Another, "其他"},
        {TriggerRange.Attached, ""},
        {TriggerRange.Self, ""},
    };

    public static SortedDictionary<TriggerRange, string> TriggerRangeDesc_en = new SortedDictionary<TriggerRange, string>
    {
        {TriggerRange.SelfPlayer, "you "},
        {TriggerRange.EnemyPlayer, "enemy "},
        {TriggerRange.OnePlayer, "one player "},
        {TriggerRange.One, "one "},
        {TriggerRange.SelfAnother, "your another "},
        {TriggerRange.Another, "another "},
        {TriggerRange.Attached, ""},
        {TriggerRange.Self, "this"},
    };
}
using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Encapsulate side effect and its TriggerTime, TriggerRange, number of trigger times, and other attributes. 
/// </summary>
public class SideEffectExecute : IClone<SideEffectExecute>
{
    private static int idGenerator = 5000;

    public static int GenerateID()
    {
        return idGenerator++;
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SideEffectFrom
    {
        Unknown = 0,
        Buff = 1,
        SpellCard = 2,
        EnergyCard = 4,
        RetinueSideEffect = 8,
        EquipSideEffect = 16,
    }

    public SideEffectFrom M_SideEffectFrom;

    public int ID;
    public ExecuteSetting M_ExecuteSetting;
    public List<SideEffectBase> SideEffectBases;

    /// <summary>
    /// 对于不同的Type，有一些trigger参数的预设值，防止设置错误
    /// </summary>
    public enum ExecuteSettingTypes
    {
        PlayOutEffect,
        BattleCry,
        SelfRetinueDie,
        SelfEquipDie,
        WhenSelfRetinueAttack,
        WhenSelfRetinueKillOther,
        Others // 高级自定义
    }

    public ExecuteSettingTypes ExecuteSettingType {
        get
        {
            foreach (KeyValuePair<ExecuteSettingTypes, ExecuteSetting> kv in ExecuteSetting_Presets)
            {
                if (M_ExecuteSetting.IsEqual(kv.Value))
                {
                    return kv.Key;
                }
            }

            return ExecuteSettingTypes.Others;
        }
}

    public struct ExecuteSetting
    {
        public TriggerTime TriggerTime; //when to trigger
        public TriggerRange TriggerRange; //which range of events can trigger this effect
        public int TriggerTimes; //how many times we need to trigger it before it can realy trigger
        public int TriggerDelayTimes; //the max times we can trigger it.
        public TriggerTime RemoveTriggerTime; //when to remove this effect/decrease the remove time of this effect
        public TriggerRange RemoveTriggerRange; //which range of events can remove this effect
        public int RemoveTriggerTimes; //how many times of remove before we can remove the effect permenantly. (usually used in buffs)

        public ExecuteSetting(TriggerTime triggerTime, TriggerRange triggerRange, int triggerTimes, int triggerDelayTimes, TriggerTime removeTriggerTime, TriggerRange removeTriggerRange, int removeTriggerTimes)
        {
            TriggerTime = triggerTime;
            TriggerRange = triggerRange;
            TriggerTimes = triggerTimes;
            TriggerDelayTimes = triggerDelayTimes;
            RemoveTriggerTime = removeTriggerTime;
            RemoveTriggerRange = removeTriggerRange;
            RemoveTriggerTimes = removeTriggerTimes;
        }

        public static ExecuteSetting GenerateFromXMLNode(XmlNode node)
        {
            ExecuteSettingTypes est = (ExecuteSettingTypes) Enum.Parse(typeof(ExecuteSettingTypes), node.Attributes["ExecuteSettingTypes"].Value);
            if (est == ExecuteSettingTypes.Others)
            {
                TriggerTime triggerTime = (TriggerTime)Enum.Parse(typeof(TriggerTime), node.Attributes["triggerTime"].Value);
                TriggerRange triggerRange = (TriggerRange)Enum.Parse(typeof(TriggerRange), node.Attributes["triggerRange"].Value);
                int triggerDelayTimes = int.Parse(node.Attributes["triggerDelayTimes"].Value);
                int triggerTimes = int.Parse(node.Attributes["triggerTimes"].Value);
                TriggerTime removeTriggerTime = (TriggerTime)Enum.Parse(typeof(TriggerTime), node.Attributes["removeTriggerTime"].Value);
                TriggerRange removeTriggerRange = (TriggerRange)Enum.Parse(typeof(TriggerRange), node.Attributes["removeTriggerRange"].Value);
                int removeTriggerTimes = int.Parse(node.Attributes["removeTriggerTimes"].Value);
                ExecuteSetting newExecuteSetting = new ExecuteSetting(triggerTime,triggerRange,triggerTimes,triggerDelayTimes,removeTriggerTime,removeTriggerRange,removeTriggerTimes);
                return newExecuteSetting;
            }
            else
            {
                return ExecuteSetting_Presets[est];
            }
        }

        public bool IsEqual(ExecuteSetting target)
        {
            if (TriggerTime != target.TriggerTime) return false;
            if (TriggerRange != target.TriggerRange) return false;
            if (TriggerDelayTimes != target.TriggerDelayTimes) return false;
            if (TriggerTimes != target.TriggerTimes) return false;
            if (RemoveTriggerTime != target.RemoveTriggerTime) return false;
            if (RemoveTriggerRange != target.RemoveTriggerRange) return false;
            if (RemoveTriggerTimes != target.RemoveTriggerTimes) return false;
            return true;
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32((int) TriggerTime);
            writer.WriteSInt32((int) TriggerRange);
            writer.WriteSInt32(TriggerDelayTimes);
            writer.WriteSInt32(TriggerTimes);
            writer.WriteSInt32((int) RemoveTriggerTime);
            writer.WriteSInt32((int) RemoveTriggerRange);
            writer.WriteSInt32(RemoveTriggerTimes);
        }

        public static ExecuteSetting Deserialize(DataStream reader)
        {
            ExecuteSetting es = new ExecuteSetting();
            es.TriggerTime = (TriggerTime) reader.ReadSInt32();
            es.TriggerRange = (TriggerRange) reader.ReadSInt32();
            es.TriggerDelayTimes = reader.ReadSInt32();
            es.TriggerTimes = reader.ReadSInt32();
            es.RemoveTriggerTime = (TriggerTime) reader.ReadSInt32();
            es.RemoveTriggerRange = (TriggerRange) reader.ReadSInt32();
            es.RemoveTriggerTimes = reader.ReadSInt32();
            return es;
        }
    }

    /// <summary>
    /// 对于不同的Type，有一些trigger参数的预设值，防止设置错误
    /// </summary>
    public static Dictionary<ExecuteSettingTypes, ExecuteSetting> ExecuteSetting_Presets = new Dictionary<ExecuteSettingTypes, ExecuteSetting>
    {
        {
            ExecuteSettingTypes.PlayOutEffect, new ExecuteSetting(
                triggerTime: TriggerTime.OnPlayCard,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.BattleCry, new ExecuteSetting(
                triggerTime: TriggerTime.OnRetinueSummon,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 99999,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.SelfRetinueDie, new ExecuteSetting(
                triggerTime: TriggerTime.OnRetinueDie,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.SelfEquipDie, new ExecuteSetting(
                triggerTime: TriggerTime.OnEquipDie,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.WhenSelfRetinueAttack, new ExecuteSetting(
                triggerTime: TriggerTime.OnRetinueAttack,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.WhenSelfRetinueKillOther, new ExecuteSetting(
                triggerTime: TriggerTime.OnRetinueKill,
                triggerRange: TriggerRange.Self,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
        {
            ExecuteSettingTypes.Others, new ExecuteSetting(
                triggerTime: TriggerTime.None,
                triggerRange: TriggerRange.None,
                triggerDelayTimes: 0,
                triggerTimes: 1,
                removeTriggerTime: TriggerTime.None,
                removeTriggerRange: TriggerRange.None,
                removeTriggerTimes: 1)
        },
    };

    private SideEffectExecute()
    {
    }

    public SideEffectExecute(SideEffectFrom sideEffectFrom, List<SideEffectBase> sideEffectBases, ExecuteSetting executeSetting)
    {
        M_SideEffectFrom = sideEffectFrom;
        ID = GenerateID();
        SideEffectBases = sideEffectBases;
        M_ExecuteSetting = executeSetting;
    }

    public SideEffectExecute Clone()
    {
        return new SideEffectExecute(M_SideEffectFrom, CloneVariantUtils.List(SideEffectBases), M_ExecuteSetting);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) M_SideEffectFrom);
        writer.WriteSInt32(ID);
        writer.WriteSInt32(SideEffectBases.Count);
        foreach (SideEffectBase se in SideEffectBases)
        {
            se.Serialize(writer);
        }

        M_ExecuteSetting.Serialize(writer);
    }

    public static SideEffectExecute Deserialize(DataStream reader)
    {
        SideEffectExecute see = new SideEffectExecute();
        see.M_SideEffectFrom = (SideEffectFrom) reader.ReadSInt32();
        see.ID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            SideEffectBase se = SideEffectBase.BaseDeserialize(reader);
            see.SideEffectBases.Add(se);
        }

        see.M_ExecuteSetting = ExecuteSetting.Deserialize(reader);
        return see;
    }

    public string GenerateDesc()
    {
        string res = "";
        if (ExecuteSettingType == ExecuteSettingTypes.BattleCry) res += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("TriggerTime_BattleCry") + ": ");
        else if (ExecuteSettingType == ExecuteSettingTypes.SelfRetinueDie || ExecuteSettingType == ExecuteSettingTypes.SelfEquipDie) res += BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("TriggerTime_Die") + ": ");
        else if (ExecuteSettingType == ExecuteSettingTypes.WhenSelfRetinueAttack) res += LanguageManager_Common.GetText("TriggerTime_WhenAttack");
        else if (ExecuteSettingType == ExecuteSettingTypes.WhenSelfRetinueKillOther) res += LanguageManager_Common.GetText("TriggerTime_WhenKill");
        else if (ExecuteSettingType == ExecuteSettingTypes.PlayOutEffect)
        {
            foreach (SideEffectBase se in SideEffectBases)
            {
                if (se is Exile_Base)
                {
                    res = BaseInfo.AddImportantColorToText(LanguageManager_Common.GetText("TriggerTime_Disposable")) + res;
                }
            }
        }
        else
        {
            res += string.Format(TriggerTimeDesc[LanguageManager_Common.GetCurrentLanguage()][M_ExecuteSetting.TriggerTime], BaseInfo.AddHighLightColorToText(TriggerRangeDesc[LanguageManager_Common.GetCurrentLanguage()][M_ExecuteSetting.TriggerRange]));
        }

        foreach (SideEffectBase se in SideEffectBases)
        {
            res += se.GenerateDesc();
        }

        if (res.EndsWith("</color>"))
        {
            res = res.Remove(res.LastIndexOf("</color>"));
            res = res.TrimEnd("，。;,.;/n ".ToCharArray());
            res += "</color>";
        }

        res = res.TrimEnd("，。;,.;/n ".ToCharArray());

        return res;
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TriggerTime
    {
        None = 0,

        /// <summary>
        /// e.g. a certain buff (double next damage) need to be triggered before next damage skill effect is triggered and double the skill effect number.
        /// So we need a super trigger to monitor all common triggers.
        /// </summary>
        OnTrigger = 1 << 0,

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

    public static SortedDictionary<string, SortedDictionary<TriggerTime, string>> TriggerTimeDesc = new SortedDictionary<string, SortedDictionary<TriggerTime, string>>
    {
        {
            "zh", new SortedDictionary<TriggerTime, string>
            {
                {TriggerTime.None, ""},
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
            }
        },
        {
            "en", new SortedDictionary<TriggerTime, string>
            {
                {TriggerTime.None, ""},
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
            }
        }
    };

    /// <summary>
    /// TriggerRange is used together with TriggerTime
    /// If you use a TriggerTime.OnBeginRound and a TriggerRange.EnemyPlayer, then this sideeffect will be triggered in Enemy's Begin Round Phase.
    /// </summary>
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

    public static SortedDictionary<string, SortedDictionary<TriggerRange, string>> TriggerRangeDesc = new SortedDictionary<string, SortedDictionary<TriggerRange, string>>
    {
        {
            "zh", new SortedDictionary<TriggerRange, string>
            {
                {TriggerRange.None, ""},
                {TriggerRange.SelfPlayer, "我方"},
                {TriggerRange.EnemyPlayer, "敌方"},
                {TriggerRange.OnePlayer, "任一方"},
                {TriggerRange.One, "一个"},
                {TriggerRange.SelfAnother, "我方其他"},
                {TriggerRange.Another, "其他"},
                {TriggerRange.Attached, ""},
                {TriggerRange.Self, ""},
            }
        },
        {
            "en", new SortedDictionary<TriggerRange, string>
            {
                {TriggerRange.None, ""},
                {TriggerRange.SelfPlayer, "you "},
                {TriggerRange.EnemyPlayer, "enemy "},
                {TriggerRange.OnePlayer, "one player "},
                {TriggerRange.One, "one "},
                {TriggerRange.SelfAnother, "your another "},
                {TriggerRange.Another, "another "},
                {TriggerRange.Attached, ""},
                {TriggerRange.Self, "this"},
            }
        }
    };
}
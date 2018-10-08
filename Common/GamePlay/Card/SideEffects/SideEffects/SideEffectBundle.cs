using System.Collections.Generic;

public class SideEffectBundle
{
    public SortedDictionary<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>> SideEffects;

    public SideEffectBundle()
    {
        SideEffects = new SortedDictionary<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>>();
    }

    public struct SideEffectExecute
    {
        public TriggerTime TriggerTime;
        public TriggerRange TriggerRange;
        public SideEffectBase SideEffectBase;

        public SideEffectExecute(TriggerTime triggerTime, TriggerRange triggerRange, SideEffectBase sideEffectBase)
        {
            TriggerTime = triggerTime;
            TriggerRange = triggerRange;
            SideEffectBase = sideEffectBase;
        }
    }

    public List<SideEffectExecute> GetSideEffects()
    {
        List<SideEffectExecute> sideEffectExecutes = new List<SideEffectExecute>();
        foreach (KeyValuePair<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>> des in SideEffects)
        {
            foreach (KeyValuePair<TriggerRange, List<SideEffectBase>> ses in des.Value)
            {
                foreach (SideEffectBase se in ses.Value)
                {
                    sideEffectExecutes.Add(new SideEffectExecute(des.Key, ses.Key, se));
                }
            }
        }

        return sideEffectExecutes;
    }

    public List<SideEffectBase> GetSideEffects(TriggerTime triggerTime, TriggerRange triggerRange)
    {
        if (!SideEffects.ContainsKey(triggerTime)) return new List<SideEffectBase>();
        Dictionary<TriggerRange, List<SideEffectBase>> des = SideEffects[triggerTime];
        if (!des.ContainsKey(triggerRange)) return new List<SideEffectBase>();
        List<SideEffectBase> ses = SideEffects[triggerTime][triggerRange];
        return ses;
    }

    public void AddSideEffect(SideEffectBase se, TriggerTime triggerTime, TriggerRange triggerRange)
    {
        if (!SideEffects.ContainsKey(triggerTime)) SideEffects.Add(triggerTime, new Dictionary<TriggerRange, List<SideEffectBase>>());
        Dictionary<TriggerRange, List<SideEffectBase>> des = SideEffects[triggerTime];
        if (!des.ContainsKey(triggerRange)) des.Add(triggerRange, new List<SideEffectBase>());
        List<SideEffectBase> ses = SideEffects[triggerTime][triggerRange];
        ses.Add(se);
    }

    public void RemoveSideEffect(SideEffectBase se, TriggerTime triggerTime, TriggerRange triggerRange)
    {
        if (!SideEffects.ContainsKey(triggerTime)) return;
        Dictionary<TriggerRange, List<SideEffectBase>> des = SideEffects[triggerTime];
        if (des == null || !des.ContainsKey(triggerRange)) return;
        List<SideEffectBase> ses = SideEffects[triggerTime][triggerRange];
        if (ses == null || !ses.Contains(se)) return;
        ses.Remove(se);
    }

    public string GetSideEffectsDesc(bool isEnglish)
    {
        string res = "";
        foreach (KeyValuePair<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>> kv in SideEffects)
        {
            foreach (KeyValuePair<TriggerRange, List<SideEffectBase>> SEs in kv.Value)
            {
                if (SEs.Value.Count > 0)
                {
                    if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroSummon || kv.Key == TriggerTime.OnRetinueSummon || kv.Key == TriggerTime.OnSoldierSummon)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Battlecry: " : "战吼: ");
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroDie || kv.Key == TriggerTime.OnRetinueDie || kv.Key == TriggerTime.OnSoldierDie)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Die: " : "亡语: ");
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnEquipDie)) res += BaseInfo.AddImportantColorToText(isEnglish ? "Broken: " : "亡语: ");
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnRetinueAttack)) res += isEnglish ? "When attacks, " : "进攻时, ";
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnRetinueKill)) res += isEnglish ? "When kills, " : "杀敌时, ";
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnPlayCard)) res += "";
                    else
                    {
                        if (isEnglish)
                        {
                            res += string.Format(TriggerTimeDesc_en[kv.Key], BaseInfo.AddHightLightColorToText(TriggerRangeDesc_en[SEs.Key]));
                        }
                        else
                        {
                            res += string.Format(TriggerTimeDesc[kv.Key], BaseInfo.AddHightLightColorToText(TriggerRangeDesc[SEs.Key]));
                        }
                    }

                    foreach (SideEffectBase se in SEs.Value)
                    {
                        res += se.GenerateDesc(isEnglish);
                    }
                }
            }

            res = res.TrimEnd(",. ".ToCharArray());
            res += ";\n";
        }

        return res;
    }

    public SideEffectBundle Clone()
    {
        SideEffectBundle copy = new SideEffectBundle();
        foreach (KeyValuePair<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>> des in SideEffects)
        {
            foreach (KeyValuePair<TriggerRange, List<SideEffectBase>> ses in des.Value)
            {
                foreach (SideEffectBase se in ses.Value)
                {
                    copy.AddSideEffect(se.Clone(), des.Key, ses.Key);
                }
            }
        }

        return copy;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(SideEffects.Count);
        foreach (KeyValuePair<TriggerTime, Dictionary<TriggerRange, List<SideEffectBase>>> des in SideEffects)
        {
            writer.WriteSInt32((int) des.Key);
            writer.WriteSInt32(des.Value.Count);
            foreach (KeyValuePair<TriggerRange, List<SideEffectBase>> ses in des.Value)
            {
                writer.WriteSInt32((int) ses.Key);
                writer.WriteSInt32(ses.Value.Count);
                foreach (SideEffectBase se in ses.Value)
                {
                    se.Serialze(writer);
                }
            }
        }
    }

    public static SideEffectBundle Deserialze(DataStream reader)
    {
        SideEffectBundle res = new SideEffectBundle();
        int SideEffectCount = reader.ReadSInt32();
        for (int i = 0; i < SideEffectCount; i++)
        {
            TriggerTime tt = (TriggerTime) reader.ReadSInt32();
            Dictionary<TriggerRange, List<SideEffectBase>> des = new Dictionary<TriggerRange, List<SideEffectBase>>();
            int desCount = reader.ReadSInt32();

            for (int j = 0; j < desCount; j++)
            {
                TriggerRange tr = (TriggerRange) reader.ReadSInt32();
                List<SideEffectBase> ses = new List<SideEffectBase>();
                int sesCount = reader.ReadSInt32();
                for (int k = 0; k < sesCount; k++)
                {
                    ses.Add(SideEffectBase.BaseDeserialze(reader));
                }

                des.Add(tr, ses);
            }

            res.SideEffects.Add(tt, des);
        }

        return res;
    }

    public enum TriggerTime
    {
        OnBeginRound,
        OnDrawCard,
        OnPlayCard,

        OnRetinueSummon,
        OnHeroSummon,
        OnSoldierSummon,

        OnRetinueAttack,
        OnHeroAttack,
        OnSoldierAttack,

        OnRetinueInjured,
        OnHeroInjured,
        OnSoldierInjured,

        OnRetinueKill,
        OnHeroKill,
        OnSoldierKill,

        OnRetinueMakeDamage,
        OnHeroMakeDamage,
        OnSoldierMakeDamage,

        OnRetinueBeHealed,
        OnHeroBeHealed,
        OnSoldierBeHealed,

        OnRetinueDie,
        OnHeroDie,
        OnSoldierDie,

        OnEquipDie,

        OnPlayerGetEnergy,
        OnPlayerUseEnergy,
        OnPlayerAddLife,
        OnPlayerLostLife,
        OnEndRound,
    }

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc = new SortedDictionary<TriggerTime, string>
    {
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

        {TriggerTime.OnPlayerGetEnergy, "当{0}获得能量时, "},
        {TriggerTime.OnPlayerUseEnergy, "当{0}消耗能量时, "},
        {TriggerTime.OnPlayerAddLife, "当{0}获得生命时, "},
        {TriggerTime.OnPlayerLostLife, "当{0}生命减少时, "},
        {TriggerTime.OnEndRound, "{0}回合结束时, "},
    };

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc_en = new SortedDictionary<TriggerTime, string>
    {
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

        {TriggerTime.OnEquipDie, "When {0} Mech's Equipment broken"},

        {TriggerTime.OnPlayerGetEnergy, "When {0} get energy, "},
        {TriggerTime.OnPlayerUseEnergy, "When {0} consume energy, "},
        {TriggerTime.OnPlayerAddLife, "When {0} get healed, "},
        {TriggerTime.OnPlayerLostLife, "When {0} lost life, "},
        {TriggerTime.OnEndRound, "When {0} turn ends, "},
    };

    public enum TriggerRange
    {
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
using System;
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
                    if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroSummon || kv.Key == TriggerTime.OnRetinueSummon || kv.Key == TriggerTime.OnSoldierSummon)) res += isEnglish ? "Battlecry: " : "战吼:";
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnHeroDie || kv.Key == TriggerTime.OnRetinueDie || kv.Key == TriggerTime.OnSoldierDie)) res += isEnglish ? "Die: " : "亡语:";
                    else if (SEs.Key == TriggerRange.Self && (kv.Key == TriggerTime.OnPlayCard)) res += "";
                    else
                    {
                        if (isEnglish)
                        {
                            res += string.Format(TriggerTimeDesc_en[kv.Key], TriggerRangeDesc_en[SEs.Key]);
                        }
                        else
                        {
                            res += string.Format(TriggerTimeDesc[kv.Key], TriggerRangeDesc[SEs.Key]);
                        }
                    }

                    foreach (SideEffectBase se in SEs.Value)
                    {
                        res += se.GenerateDesc(isEnglish);
                    }
                }
            }

            res = res.TrimEnd(". ".ToCharArray());
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

        OnRetinueDie,
        OnHeroDie,
        OnSoldierDie,

        OnEndRound,
    }

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc = new SortedDictionary<TriggerTime, string>
    {
        {TriggerTime.OnBeginRound, "{0}回合开始时,"},
        {TriggerTime.OnDrawCard, "{0}抽牌时,"},
        {TriggerTime.OnPlayCard, "{0}出牌时,"},

        {TriggerTime.OnRetinueSummon, "{0}召唤机甲时,"},
        {TriggerTime.OnHeroSummon, "{0}召唤英雄时,"},
        {TriggerTime.OnSoldierSummon, "{0}召唤士兵时,"},

        {TriggerTime.OnRetinueAttack, "当{0}机甲进攻时,"},
        {TriggerTime.OnHeroAttack, "当{0}英雄进攻时,"},
        {TriggerTime.OnSoldierAttack, "当{0}士兵进攻时,"},

        {TriggerTime.OnRetinueInjured, "当{0}机甲受损时,"},
        {TriggerTime.OnHeroInjured, "当{0}英雄受损时,"},
        {TriggerTime.OnSoldierInjured, "当{0}士兵受损时,"},

        {TriggerTime.OnRetinueDie, "当{0}机甲死亡时,"},
        {TriggerTime.OnHeroDie, "当{0}英雄死亡时,"},
        {TriggerTime.OnSoldierDie, "当{0}士兵死亡时,"},

        {TriggerTime.OnEndRound, "{0}回合结束时,"},
    };

    public static SortedDictionary<TriggerTime, string> TriggerTimeDesc_en = new SortedDictionary<TriggerTime, string>
    {
        {TriggerTime.OnBeginRound, "When {0} turn starts, "},
        {TriggerTime.OnDrawCard, "Every time when {0} draws, "},
        {TriggerTime.OnPlayCard, "Every time when {0} plays a card, "},

        {TriggerTime.OnRetinueSummon, "When {0} summon a Mech, "},
        {TriggerTime.OnHeroSummon, "When {0} summon a HeroMech, "},
        {TriggerTime.OnSoldierSummon, "When {0} summon a SoldierMech, "},

        {TriggerTime.OnRetinueAttack, "When a Mech of {0} attacks, "},
        {TriggerTime.OnHeroAttack, "When a HeroMech of {0} attacks, "},
        {TriggerTime.OnSoldierAttack, "When a SoldierMech of {0} attacks, "},

        {TriggerTime.OnRetinueInjured, "When a Mech of {0} damaged, "},
        {TriggerTime.OnHeroInjured, "When a HeroMech of {0} damaged, "},
        {TriggerTime.OnSoldierInjured, "When a SoldierMech of {0} damaged, "},

        {TriggerTime.OnRetinueDie, "When a Mech of {0} died, "},
        {TriggerTime.OnHeroDie, "When a HeroMech of {0} died, "},
        {TriggerTime.OnSoldierDie, "When a SoldierMech of {0} died, "},

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
        {TriggerRange.Self, ""},
    };
}
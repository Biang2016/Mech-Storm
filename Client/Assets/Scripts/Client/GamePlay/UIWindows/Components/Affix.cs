﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Affix : PoolObject
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Image BoardImage;

    internal AffixType AffixType;

    [SerializeField] private Color NoneColor;

    [SerializeField] private Color DieColor;
    [SerializeField] private Color BattleCryColor;

    [SerializeField] private Color SniperColor;
    [SerializeField] private Color FrenzyColor;
    [SerializeField] private Color DefenceColor;
    [SerializeField] private Color SentryColor;
    [SerializeField] private Color ChargerColor;

    [SerializeField] private Color MAColor;
    [SerializeField] private Color AttackColor;
    [SerializeField] private Color ShieldColor;
    [SerializeField] private Color ArmorColor;
    [SerializeField] private Color DodgeColor;

    [SerializeField] private Color SwordColor;
    [SerializeField] private Color GunColor;
    [SerializeField] private Color SniperGunColor;

    private Dictionary<AffixType, Color> AffixColorDict;

    void Awake()
    {
        AffixColorDict = new Dictionary<AffixType, Color>
        {
            {AffixType.None, NoneColor},

            {AffixType.Die, DieColor},
            {AffixType.BattleCry, BattleCryColor},

            {AffixType.Sniper, SniperColor},
            {AffixType.Frenzy, FrenzyColor},
            {AffixType.Defence, DefenceColor},
            {AffixType.Sentry, SentryColor},
            {AffixType.Charger, ChargerColor},

            {AffixType.MA, MAColor},
            {AffixType.Attack, AttackColor},
            {AffixType.Shield, ShieldColor},
            {AffixType.Armor, ArmorColor},
            {AffixType.Dodge, DodgeColor},

            {AffixType.Sword, SwordColor},
            {AffixType.Gun, GunColor},
            {AffixType.SniperGun, SniperGunColor},

            {AffixType.Disposable, AttackColor},
            {AffixType.Immune, FrenzyColor},
            {AffixType.Inactivity, ShieldColor},
        };
    }

    public void Initialize(AffixType affixType)
    {
        string text = "";

        text = BaseInfo.AddHighLightColorToText(AffixNameDict[LanguageManager.Instance.GetCurrentLanguage()][affixType]) + ": " + AffixDescDict[LanguageManager.Instance.GetCurrentLanguage()][affixType];
        Text.enableAutoSizing = LanguageManager.Instance.IsEnglish;

        Text.text = text;
        BoardImage.color = AffixColorDict[affixType];
    }

    private static Dictionary<string, Dictionary<AffixType, string>> AffixNameDict = new Dictionary<string, Dictionary<AffixType, string>>
    {
        {
            "zh", new Dictionary<AffixType, string>
            {
                {AffixType.None, ""},

                {AffixType.Die, "亡语"},
                {AffixType.BattleCry, "战吼"},

                {AffixType.Sniper, "狙击"},
                {AffixType.Frenzy, "狂暴"},
                {AffixType.Defence, "嘲讽"},
                {AffixType.Sentry, "哨戒"},
                {AffixType.Charger, "冲锋"},

                {AffixType.MA, "MA"},
                {AffixType.Attack, "攻击力"},
                {AffixType.Shield, "护盾"},
                {AffixType.Armor, "护甲"},
                {AffixType.Dodge, "闪避"},

                {AffixType.Sword, "刀剑"},
                {AffixType.Gun, "枪"},
                {AffixType.SniperGun, "狙击枪"},

                {AffixType.Disposable, "移除"},
                {AffixType.Immune, "免疫"},
                {AffixType.Inactivity, "失能"},
            }
        },
        {
            "en", new Dictionary<AffixType, string>
            {
                {AffixType.None, ""},

                {AffixType.Die, "Die"},
                {AffixType.BattleCry, "BattleCry"},

                {AffixType.Sniper, "Sniper"},
                {AffixType.Frenzy, "Frenzy"},
                {AffixType.Defence, "Defence"},
                {AffixType.Sentry, "Sentry"},
                {AffixType.Charger, "Charger"},

                {AffixType.MA, "MA"},
                {AffixType.Attack, "Attack"},
                {AffixType.Shield, "Shield"},
                {AffixType.Armor, "Armor"},
                {AffixType.Dodge, "Dodge"},

                {AffixType.Sword, "Sword"},
                {AffixType.Gun, "Gun"},
                {AffixType.SniperGun, "SniperGun"},

                {AffixType.Disposable, "Disposable"},
                {AffixType.Immune, "Immune"},
                {AffixType.Inactivity, "Inactivity"},
            }
        }
    };

    private static Dictionary<string, Dictionary<AffixType, string>> AffixDescDict = new Dictionary<string, Dictionary<AffixType, string>>
    {
        {
            "zh", new Dictionary<AffixType, string>
            {
                {AffixType.None, ""},

                {AffixType.Die, "当机甲死亡或装备被摧毁时触发"},
                {AffixType.BattleCry, "当机甲或装备被召唤到场上时触发"},

                {AffixType.Sniper, "具有狙击属性的机甲只能装备狙击枪，狙击枪可以攻击任何对象(不受嘲讽限制)"},
                {AffixType.Frenzy, "每回合可以发起两次攻击"},
                {AffixType.Defence, "敌方近战和枪类武器必须优先攻击嘲讽目标，狙击枪不受此限制"},
                {AffixType.Sentry, "无法主动进攻，但是可以反击"},
                {AffixType.Charger, "召唤后可以立即进攻"},

                {AffixType.MA, "终极装备，仅当其余装备均佩戴的情况下才能装备"},
                {AffixType.Attack, "若未装备武器，则伤害等于攻击力。装备武器时叠加此攻击力"},
                {AffixType.Shield, "免疫小于护盾值的伤害，超出护盾值的伤害将使护盾受损"},
                {AffixType.Armor, "抵御等同于护甲值的伤害"},
                {AffixType.Dodge, "有一定概率闪避伤害"},

                {AffixType.Sword, "伤害=攻击力*能量，每次攻击后能量提升"},
                {AffixType.Gun, "打出所有子弹，每发子弹造成一定伤害，当对方无嘲讽机甲时可攻击战舰"},
                {AffixType.SniperGun, "狙击枪只能装备在具有狙击属性的机甲上，可攻击任意敌方角色"},

                {AffixType.Disposable, "使用一次后就移出战斗"},
                {AffixType.Immune, "免疫所有伤害，若干回合后失效"},
                {AffixType.Inactivity, "无法进行主动攻击"}
            }
        },
        {
            "en", new Dictionary<AffixType, string>
            {
                {AffixType.None, ""},

                {AffixType.Die, "When this mech is killed or equip is destroyed, something will be triggered. "},
                {AffixType.BattleCry, "When this mech or equip is summoned, something will be triggered."},

                {AffixType.Sniper, "Sniper mechs can only equip Snipergun. Sniper can shoot anything including defender, mechs and ship."},
                {AffixType.Frenzy, "Can attack twice per round."},
                {AffixType.Defence, "Defence those non-defence mechs and your ship from being attacked by swords and guns (except snipergun)."},
                {AffixType.Sentry, "Cannot attack but can counterattack."},
                {AffixType.Charger, "Can attack immediately after summoned."},

                {AffixType.MA, "Ultimate equip. Can be equiped only when other slots are equiped."},
                {AffixType.Attack, "Damage equals to attack value without weapon. Attack value can be added to its weapons'."},
                {AffixType.Shield, "Immune to all damage that lower than shield value. Be injured by overflow damage."},
                {AffixType.Armor, "Defence the part of damage equal to armor value."},
                {AffixType.Dodge, "Has probability(PR) to avoid attacks."},

                {AffixType.Sword, "Damage = attack * energy. Energy increases after attack."},
                {AffixType.Gun, "Bursts all bullets when attacks. Can shoot the ship when there's no defender."},
                {AffixType.SniperGun, "Only equipped on the Sniper mechs. Can attack any enemy and ship."},

                {AffixType.Disposable, "Cannot be recycled to the deck again."},
                {AffixType.Immune, "Immune to all damage, which lasts for some rounds."},
                {AffixType.Inactivity, "Cannot attack actively."}
            }
        }
    };
}

public enum AffixType
{
    None,

    Die,
    BattleCry,

    Sniper,
    Frenzy,
    Defence,
    Sentry,
    Charger,

    MA,
    Attack,
    Shield,
    Armor,
    Dodge,

    Sword,
    Gun,
    SniperGun,

    Disposable,
    Immune,
    Inactivity,
}
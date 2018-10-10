using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngineInternal.Input;

public class Affix : PoolObject
{
    [SerializeField] private Text Text;
    [SerializeField] private Image BoardImage;

    internal AffixType AffixType;

    [SerializeField] private Color NoneColor;
    [SerializeField] private Color DieColor;
    [SerializeField] private Color BattleCryColor;
    [SerializeField] private Color SniperColor;
    [SerializeField] private Color FrenzyColor;
    [SerializeField] private Color DefenceColor;
    [SerializeField] private Color SentryColor;

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
        };
    }

    public void Initialize(AffixType affixType)
    {
        string text = "";
        if (GameManager.Instance.isEnglish)
        {
            text = AffixDescDict_en[affixType];
        }
        else
        {
            text = AffixDescDict[affixType];
        }

        Text.text = text;
        BoardImage.color = AffixColorDict[affixType];
    }

    private static Dictionary<AffixType, string> AffixDescDict = new Dictionary<AffixType, string>
    {
        {AffixType.None, ""},
        {AffixType.Die, "亡语: 当机甲死亡或装备被摧毁时触发"},
        {AffixType.BattleCry, "战吼: 当机甲或装备被召唤到场上时触发"},
        {AffixType.Sniper, "狙击: 具有狙击属性的机甲只能装备狙击枪，狙击枪可以攻击任何对象(不受嘲讽限制)"},
        {AffixType.Frenzy, "狂暴: 每回合可以发起两次攻击"},
        {AffixType.Defence, "嘲讽: 敌方近战和枪类武器必须优先攻击嘲讽目标，狙击枪不受此限制"},
        {AffixType.Sentry, "哨戒: 无法主动进攻，但是可以反击"},
        {AffixType.Charger, "冲锋: 召唤后可以立即进攻"},
    };

    private static Dictionary<AffixType, string> AffixDescDict_en = new Dictionary<AffixType, string>
    {
        {AffixType.None, ""},
        {AffixType.Die, "Die: When this mech is killed or equip is destroyed, something will be triggered. "},
        {AffixType.BattleCry, "BattleCry: When this mech or equip is summoned, something will be triggered."},
        {AffixType.Sniper, "Sniper: Sniper mechs can only equip Snipergun. Sniper can shoot anything including defender, mechs and ship."},
        {AffixType.Frenzy, "Frenzy: Can attack twice per round."},
        {AffixType.Defence, "Defence: Defence those non-defence mechs and your ship from being attacked by swords and guns (except snipergun)."},
        {AffixType.Sentry, "Sentry: Cannot attack but can counterattack."},
        {AffixType.Charger, "Charger: Can attack immediately after summoned."},
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
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct Bonus
{
    public BonusType M_BonusType;
    public int Value;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BonusType
    {
        UnlockCard,
        AdjustDeck,
        LifeUpperLimit,
        EnergyUpperLimit,
        Budget,
    }

    public string GetDesc(bool isEnglish)
    {
        Dictionary<BonusType, string> dic = isEnglish ? BonusDescRaw_en : BonusDescRaw;
        return string.Format(dic[M_BonusType], Value > 0 ? ("+" + Value) : ("-" + Value));
    }

    static Dictionary<BonusType, string> BonusDescRaw = new Dictionary<BonusType, string>
    {
        {BonusType.UnlockCard, "解锁卡片"},
        {BonusType.AdjustDeck, "获得一次调整卡组的机会"},
        {BonusType.LifeUpperLimit, "生命上限{0}"},
        {BonusType.EnergyUpperLimit, "能量上限{0}"},
        {BonusType.Budget, "预算{0}"},
    };

    static Dictionary<BonusType, string> BonusDescRaw_en = new Dictionary<BonusType, string>
    {
        {BonusType.UnlockCard, "Unlock card"},
        {BonusType.AdjustDeck, "A change to adjust deck"},
        {BonusType.LifeUpperLimit, "Life {0}"},
        {BonusType.EnergyUpperLimit, "Energy {0}"},
        {BonusType.Budget, "Budget {0}"},
    };

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) M_BonusType);
        writer.WriteSInt32(Value);
    }

    public static Bonus Deserialize(DataStream reader)
    {
        Bonus newBonus = new Bonus();
        newBonus.M_BonusType = (BonusType) reader.ReadSInt32();
        newBonus.Value = reader.ReadSInt32();
        return newBonus;
    }
}
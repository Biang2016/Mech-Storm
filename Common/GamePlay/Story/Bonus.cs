using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct Bonus
{
    public BonusType M_BonusType;
    public int Value;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BonusType
    {
        UnlockCardByID,
        UnlockCardByLevelNum,
        AdjustDeck,
        LifeUpperLimit,
        EnergyUpperLimit,
        Budget,
    }

    public string GetDesc()
    {
        Dictionary<BonusType, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return string.Format(dic[M_BonusType], Value > 0 ? ("+" + Value) : ("-" + Value));
    }

    static Dictionary<string, Dictionary<BonusType, string>> BonusDescRaw = new Dictionary<string, Dictionary<BonusType, string>>
    {
        {
            "zh", new Dictionary<BonusType, string>
            {
                {BonusType.UnlockCardByID, "解锁卡片"},
                {BonusType.UnlockCardByLevelNum, "解锁卡片"},
                {BonusType.AdjustDeck, "获得一次调整卡组的机会"},
                {BonusType.LifeUpperLimit, "生命上限{0}"},
                {BonusType.EnergyUpperLimit, "能量上限{0}"},
                {BonusType.Budget, "预算{0}"},
            }
        },
        {
            "en", new Dictionary<BonusType, string>
            {
                {BonusType.UnlockCardByID, "Unlock card"},
                {BonusType.UnlockCardByLevelNum, "Unlock card"},
                {BonusType.AdjustDeck, "A chance to adjust deck"},
                {BonusType.LifeUpperLimit, "Life {0}"},
                {BonusType.EnergyUpperLimit, "Energy {0}"},
                {BonusType.Budget, "Budget {0}"},
            }
        }
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
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Bonus : IHardFactor, IClone<Bonus>
{
    public BonusType M_BonusType;
    public HardFactorValue BonusBaseValue;
    private int hardFactor;

    public Bonus(BonusType mBonusType, HardFactorValue bonusBaseValue, int hardFactor)
    {
        M_BonusType = mBonusType;
        BonusBaseValue = bonusBaseValue;
        this.hardFactor = hardFactor;
    }

    public List<HardFactorValue> Values
    {
        get { return new List<HardFactorValue> {BonusBaseValue}; }
    }

    public int GetFactor()
    {
        return hardFactor;
    }

    public void SetFactor(int value)
    {
        hardFactor = value;
    }

    public int BonusFinalValue
    {
        get
        {
            if (M_BonusType == BonusType.LifeUpperLimit || M_BonusType == BonusType.EnergyUpperLimit || M_BonusType == BonusType.Budget)
            {
                if (GetFactor() == 0)
                {
                    return 0;
                }

                int res = (int) (BonusBaseValue.Value * ((float) GetFactor() / 100));
                if (res == 0) res = 1;
                return res;
            }
            else
            {
                return BonusBaseValue.Value;
            }
        }
    }

    public string GetDesc()
    {
        Dictionary<BonusType, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return string.Format(dic[M_BonusType], BonusFinalValue > 0 ? ("+" + BonusFinalValue) : ("-" + BonusFinalValue));
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
        writer.WriteSInt32(BonusBaseValue.Value);
        writer.WriteSInt32(hardFactor);
    }

    public static Bonus Deserialize(DataStream reader)
    {
        BonusType type = (BonusType) reader.ReadSInt32();
        int value = reader.ReadSInt32();
        int hardFactor = reader.ReadSInt32();
        HardFactorValue bonusBaseValue = new HardFactorValue(value);
        Bonus newBonus = new Bonus(type, bonusBaseValue, hardFactor);
        return newBonus;
    }

    public Bonus Clone()
    {
        return new Bonus(M_BonusType, new HardFactorValue(BonusBaseValue.Value), GetFactor());
    }

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
}
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
        MagicUpperLimit,
        Budget,
    }

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
using System.Collections.Generic;
using System.Linq;

public class BonusGroup : Probability
{
    public bool IsAlways;
    public List<Bonus> Bonuses = new List<Bonus>();

    public int Probability { get; set; }

    public BonusGroup()
    {
    }

    public BonusGroup(bool isAlways, List<Bonus> bonuses, int probability)
    {
        IsAlways = isAlways;
        Bonuses = bonuses;
        Probability = probability;
    }

    public BonusGroup Clone()
    {
        return new BonusGroup(IsAlways, Bonuses.ToArray().ToList(), Probability);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsAlways ? 0x01 : 0x00));

        writer.WriteSInt32(Bonuses.Count);
        foreach (Bonus bonus in Bonuses)
        {
            bonus.Serialize(writer);
        }

        writer.WriteSInt32(Probability);
    }

    public static BonusGroup Deserialize(DataStream reader)
    {
        BonusGroup newBonusGroup = new BonusGroup();
        newBonusGroup.IsAlways = reader.ReadByte() == 0x01;

        int BonusesCount = reader.ReadSInt32();
        newBonusGroup.Bonuses = new List<Bonus>();
        for (int i = 0; i < BonusesCount; i++)
        {
            newBonusGroup.Bonuses.Add(Bonus.Deserialize(reader));
        }

        newBonusGroup.Probability = reader.ReadSInt32();

        return newBonusGroup;
    }
}

public interface Probability
{
    int Probability { get; set; }
}
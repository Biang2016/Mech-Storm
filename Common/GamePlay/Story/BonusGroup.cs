using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;

public class BonusGroup : Probability
{
    public bool IsAlways;
    public List<Bonus> Bonuses = new List<Bonus>();

    public int Probability { get; set; }
    public bool Singleton { get; set; }

    public Probability ProbabilityClone()
    {
        return Clone();
    }

    public BonusGroup()
    {
    }

    public BonusGroup(bool isAlways, List<Bonus> bonuses, int probability, bool singleton)
    {
        IsAlways = isAlways;
        Bonuses = bonuses;
        Probability = probability;
        Singleton = singleton;
    }

    public BonusGroup Clone()
    {
        return new BonusGroup(IsAlways, Bonuses.ToArray().ToList(), Probability, Singleton);
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
        writer.WriteByte((byte) (Singleton ? 0x01 : 0x00));
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
        newBonusGroup.Singleton = reader.ReadByte() == 0x01;

        return newBonusGroup;
    }
}

public interface Probability
{
    int Probability { get; set; }
    bool Singleton { get; set; }

    Probability ProbabilityClone();
}
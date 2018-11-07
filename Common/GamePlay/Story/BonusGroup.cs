using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BonusGroup
{
    public bool IsAlways;
    public List<Bonus> Bonuses = new List<Bonus>();

    public BonusGroup()
    {
    }

    public BonusGroup(bool isAlways, List<Bonus> bonuses)
    {
        IsAlways = isAlways;
        Bonuses = bonuses;
    }

    public BonusGroup Clone()
    {
        return new BonusGroup(IsAlways, Bonuses.ToArray().ToList());
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsAlways ? 0x01 : 0x00));

        writer.WriteSInt32(Bonuses.Count);
        foreach (Bonus bonus in Bonuses)
        {
            bonus.Serialize(writer);
        }
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

        return newBonusGroup;
    }
}
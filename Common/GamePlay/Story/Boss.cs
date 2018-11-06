using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

public struct Boss
{
    public string Name;
    public string BuildName;
    public int PicID;

    public List<BonusGroup> AlwaysBonusGroup;
    public List<BonusGroup> OptionalBonusGroup;

    public void Serialize(DataStream writer)
    {
        writer.WriteString8(Name);
        writer.WriteString8(BuildName);
        writer.WriteSInt32(PicID);

        writer.WriteSInt32(AlwaysBonusGroup.Count);
        foreach (BonusGroup bonus in AlwaysBonusGroup)
        {
            bonus.Serialize(writer);
        }

        writer.WriteSInt32(OptionalBonusGroup.Count);
        foreach (BonusGroup bonus in OptionalBonusGroup)
        {
            bonus.Serialize(writer);
        }
    }

    public static Boss Deserialize(DataStream reader)
    {
        Boss newBoss = new Boss();
        newBoss.Name = reader.ReadString8();
        newBoss.BuildName = reader.ReadString8();
        newBoss.PicID = reader.ReadSInt32();

        int alwaysBonusCount = reader.ReadSInt32();
        newBoss.AlwaysBonusGroup = new List<BonusGroup>();
        for (int i = 0; i < alwaysBonusCount; i++)
        {
            newBoss.AlwaysBonusGroup.Add(BonusGroup.Deserialize(reader));
        }

        int optionalBonusCount = reader.ReadSInt32();
        newBoss.OptionalBonusGroup = new List<BonusGroup>();
        for (int i = 0; i < optionalBonusCount; i++)
        {
            newBoss.OptionalBonusGroup.Add(BonusGroup.Deserialize(reader));
        }

        return newBoss;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Level
{
    public int LevelID;
    public List<Boss> Bosses = new List<Boss>();

    public Level()
    {
    }

    public Level(int levelID, List<Boss> bosses)
    {
        LevelID = levelID;
        Bosses = bosses;
    }

    public Level Clone()
    {
        return new Level(LevelID, Bosses.ToArray().ToList());
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(Bosses.Count);
        foreach (Boss boss in Bosses)
        {
            boss.Serialize(writer);
        }
    }

    public static Level Deserialize(DataStream reader)
    {
        Level newLevel = new Level();
        newLevel.LevelID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        newLevel.Bosses = new List<Boss>();
        for (int i = 0; i < count; i++)
        {
            newLevel.Bosses.Add(Boss.Deserialize(reader));
        }

        return newLevel;
    }

    public string DeserializeLog()
    {
        string log = "";
        log += " [LevelID]=" + LevelID;
        log += " [Bosses]=";

        foreach (Boss boss in Bosses)
        {
            log += boss.DeserializeLog();
        }

        return log;
    }
}
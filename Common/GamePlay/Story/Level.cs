using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Level
{
    public int LevelID;
    public int LevelNum;
    public SortedDictionary<int, Boss> Bosses = new SortedDictionary<int, Boss>(); //Key : BossPicID

    public Level()
    {
    }

    public Level(int levelID, int levelNum, SortedDictionary<int, Boss> bosses)
    {
        LevelID = levelID;
        LevelNum = levelNum;
        Bosses = bosses;
    }

    public Level Clone()
    {
        SortedDictionary<int, Boss> newBosses = new SortedDictionary<int, Boss>();
        foreach (KeyValuePair<int, Boss> keyValuePair in Bosses)
        {
            newBosses.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return new Level(LevelID, LevelNum, newBosses);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(LevelNum);
        writer.WriteSInt32(Bosses.Count);
        foreach (Boss boss in Bosses.Values)
        {
            boss.Serialize(writer);
        }
    }

    public static Level Deserialize(DataStream reader)
    {
        Level newLevel = new Level();
        newLevel.LevelID = reader.ReadSInt32();
        newLevel.LevelNum = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        newLevel.Bosses = new SortedDictionary<int, Boss>();
        for (int i = 0; i < count; i++)
        {
            Boss boss = Boss.Deserialize(reader);
            newLevel.Bosses.Add(boss.PicID, boss);
        }

        return newLevel;
    }
}
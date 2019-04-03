using System.Collections.Generic;

public class Chapter
{
    public int ChapterID;
    public int LevelNum;
    public bool IsBigBoss;
    public int BigBossFightTimes;
    public SortedDictionary<int, Boss> Bosses = new SortedDictionary<int, Boss>(); //Key : BossPicID

    public Chapter()
    {
    }

    public Chapter(int chapterID, int levelNum, bool isBigBoss, int bigBossFightTimes, SortedDictionary<int, Boss> bosses)
    {
        ChapterID = chapterID;
        LevelNum = levelNum;
        IsBigBoss = isBigBoss;
        BigBossFightTimes = bigBossFightTimes;
        Bosses = bosses;
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Boss> newBosses = new SortedDictionary<int, Boss>();
        foreach (KeyValuePair<int, Boss> keyValuePair in Bosses)
        {
            newBosses.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return new Chapter(ChapterID, LevelNum, IsBigBoss, BigBossFightTimes, newBosses);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(LevelNum);
        writer.WriteByte((byte) (IsBigBoss ? 0x01 : 0x00));
        writer.WriteSInt32(BigBossFightTimes);
        writer.WriteSInt32(Bosses.Count);
        foreach (Boss boss in Bosses.Values)
        {
            boss.Serialize(writer);
        }
    }

    public static Chapter Deserialize(DataStream reader)
    {
        Chapter newLevel = new Chapter();
        newLevel.ChapterID = reader.ReadSInt32();
        newLevel.LevelNum = reader.ReadSInt32();
        newLevel.IsBigBoss = reader.ReadByte() == 0x01;
        newLevel.BigBossFightTimes = reader.ReadSInt32();
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
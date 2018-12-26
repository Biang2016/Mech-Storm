using System;
using System.Collections.Generic;
using System.Linq;

public class Story
{
    public string StoryName;
    public List<Level> Levels = new List<Level>();

    public BuildInfo PlayerCurrentBuildInfo;
    public BuildInfo PlayerCurrentUnlockedBuildInfo;

    public SortedDictionary<int, BuildInfo> PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();

    public GamePlaySettings StoryGamePlaySettings;

    public SortedDictionary<int, int> LevelBeatBossPicIDs = new SortedDictionary<int, int>(); //记录玩家击败的Boss的图片ID，如果击败过，后续游戏就不再出现，以免带来重复感. Key : LevelID, Value: BossPicID
    public SortedDictionary<int, int> LevelNumFightTimes = new SortedDictionary<int, int>(); //每个等级对应需要过关次数,Key : LevelNum
    public SortedDictionary<int, List<int>> LevelNumBossRemain = new SortedDictionary<int, List<int>>(); //每个等级Boss库剩余(不含BigBoss)  Key : LevelNum Value : BossPicIDs
    public SortedDictionary<int, List<int>> LevelNumBigBossRemain = new SortedDictionary<int, List<int>>(); //每个等级BigBoss库剩余  Key : LevelNum Value : BossPicIDs
    public SortedDictionary<int, int> LevelBossCount = new SortedDictionary<int, int>(); //每关Boss按钮数量  Key : LevelID
    public SortedDictionary<int, List<int>> LevelUnlockBossInfo = new SortedDictionary<int, List<int>>(); //玩家能看到的关卡的boss信息 Key: LevelID, Value : BossPicIDs

    public Story()
    {
    }

    public Story(string storyName, List<Level> levels, BuildInfo playerCurrentBuildInfo, BuildInfo playerCurrentUnlockedBuildInfo, GamePlaySettings storyGamePlaySettings)
    {
        StoryName = storyName;
        Levels = levels;
        PlayerCurrentBuildInfo = playerCurrentBuildInfo;
        PlayerCurrentUnlockedBuildInfo = playerCurrentUnlockedBuildInfo;
        StoryGamePlaySettings = storyGamePlaySettings;
    }

    public Story Variant() //变换关卡
    {
        List<Level> newLevels = new List<Level>();
        Levels.ForEach(level => { newLevels.Add(level.Clone()); });
        Story newStory = new Story(StoryName, newLevels, PlayerCurrentBuildInfo.Clone(), PlayerCurrentUnlockedBuildInfo.Clone(), StoryGamePlaySettings.Clone());

        SortedDictionary<int, int> levelNumBossRemainChoiceCount = new SortedDictionary<int, int>();
        SortedDictionary<int, List<int>> levelNumBigBossRemainChoices = new SortedDictionary<int, List<int>>();
        foreach (Level level in newStory.Levels)
        {
            if (!newStory.LevelNumFightTimes.ContainsKey(level.LevelNum))
            {
                newStory.LevelNumFightTimes.Add(level.LevelNum, 1);
            }
            else
            {
                newStory.LevelNumFightTimes[level.LevelNum]++;
            }

            if (!newStory.LevelNumBossRemain.ContainsKey(level.LevelNum))
            {
                List<int> bossPicIDs = new List<int>();
                List<int> bigBossPicIDs = new List<int>();

                level.Bosses.Values.ToList().ForEach(boss =>
                {
                    if (boss.Name == "Boss")
                    {
                        bigBossPicIDs.Add(boss.PicID);
                    }
                    else if (boss.Name == "Soldier")
                    {
                        bossPicIDs.Add(boss.PicID);
                    }
                });
                newStory.LevelNumBossRemain.Add(level.LevelNum, bossPicIDs);
                newStory.LevelNumBigBossRemain.Add(level.LevelNum, bigBossPicIDs);

                levelNumBossRemainChoiceCount.Add(level.LevelNum, bossPicIDs.Count);
                levelNumBigBossRemainChoices.Add(level.LevelNum, bigBossPicIDs.ToArray().ToList());
            }
        }

        Random rd = new Random();

        int levelID = 0;
        for (int i = 0; i < newStory.LevelNumFightTimes.Count; i++)
        {
            int levelFightTimes = newStory.LevelNumFightTimes[i];

            for (int j = 0; j < levelFightTimes; j++)
            {
                Level level = newStory.Levels[levelID];
                int levelNum = level.LevelNum;

                if (j < levelFightTimes - level.BigBossFightTimes) //还没到bigBoss
                {
                    int minCount = levelNumBossRemainChoiceCount[levelNum];
                    int maxCount = levelNumBossRemainChoiceCount[levelNum] + 1;
                    int curLevelBossTryCount = rd.Next(Math.Min(minCount, 2), Math.Min(4, maxCount)); //每个level尽量选出2~3个boss
                    newStory.LevelBossCount.Add(levelID, curLevelBossTryCount);

                    levelNumBigBossRemainChoices[levelNum].ForEach(bossPicID => level.Bosses.Remove(bossPicID));
                    levelNumBossRemainChoiceCount[levelNum]--;
                }
                else
                {
                    int bossPicID = levelNumBigBossRemainChoices[levelNum][0];
                    Boss temp = level.Bosses[bossPicID];
                    level.Bosses.Clear();
                    level.Bosses.Add(bossPicID, temp);
                    levelNumBigBossRemainChoices[levelNum].Remove(bossPicID);
                    newStory.LevelBossCount.Add(levelID, 1);
                }

                levelID++;

                level.IsBigBoss = false;
                level.Bosses.Values.ToList().ForEach(boss =>
                {
                    if (boss.Name == "Boss")
                    {
                        level.IsBigBoss = true;
                    }
                });
            }
        }

        return newStory;
    }

    public void BeatBoss(int levelID, int beatBossPicID)
    {
        LevelBeatBossPicIDs.Add(levelID, Levels[levelID].Bosses[beatBossPicID].PicID);
        int levelNum = Levels[levelID].LevelNum;
        Boss boss = Levels[levelID].Bosses[beatBossPicID];
        if (boss.Name == "Boss")
        {
            LevelNumBigBossRemain[levelNum].Remove(beatBossPicID);
        }
        else
        {
            LevelNumBossRemain[levelNum].Remove(beatBossPicID);
        }
    }

    public void UnlockLevelBosses(int levelID)
    {
        Level level = Levels[levelID];
        if (levelID < Levels.Count)
        {
            int nextLevelBossCount = LevelBossCount[levelID];
            List<int> bosses = null;
            if (!level.IsBigBoss)
            {
                bosses = Utils.GetRandomFromList(LevelNumBossRemain[level.LevelNum], nextLevelBossCount);
            }
            else
            {
                bosses = Utils.GetRandomFromList(LevelNumBigBossRemain[level.LevelNum], nextLevelBossCount);
            }

            LevelUnlockBossInfo.Add(levelID, bosses);
        }
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteString8(StoryName);
        writer.WriteSInt32(Levels.Count);
        foreach (Level level in Levels)
        {
            level.Serialize(writer);
        }

        PlayerCurrentBuildInfo.Serialize(writer);
        PlayerCurrentUnlockedBuildInfo.Serialize(writer);

        writer.WriteSInt32(PlayerBuildInfos.Count);
        foreach (BuildInfo bi in PlayerBuildInfos.Values)
        {
            bi.Serialize(writer);
        }

        StoryGamePlaySettings.Serialize(writer);

        writer.WriteSInt32(LevelBeatBossPicIDs.Count);
        foreach (KeyValuePair<int, int> kv in LevelBeatBossPicIDs)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }

        writer.WriteSInt32(LevelNumFightTimes.Count);
        foreach (KeyValuePair<int, int> kv in LevelNumFightTimes)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }

        writer.WriteSInt32(LevelNumBossRemain.Count);
        foreach (KeyValuePair<int, List<int>> kv in LevelNumBossRemain)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (int bossPicID in kv.Value)
            {
                writer.WriteSInt32(bossPicID);
            }
        }

        writer.WriteSInt32(LevelNumBigBossRemain.Count);
        foreach (KeyValuePair<int, List<int>> kv in LevelNumBigBossRemain)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (int bossPicID in kv.Value)
            {
                writer.WriteSInt32(bossPicID);
            }
        }

        writer.WriteSInt32(LevelBossCount.Count);
        foreach (KeyValuePair<int, int> kv in LevelBossCount)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }

        writer.WriteSInt32(LevelUnlockBossInfo.Count);
        foreach (KeyValuePair<int, List<int>> kv in LevelUnlockBossInfo)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (int bossPicID in kv.Value)
            {
                writer.WriteSInt32(bossPicID);
            }
        }
    }

    public static Story Deserialize(DataStream reader)
    {
        Story newStory = new Story();
        newStory.StoryName = reader.ReadString8();
        int levelCount = reader.ReadSInt32();
        newStory.Levels = new List<Level>();
        for (int i = 0; i < levelCount; i++)
        {
            newStory.Levels.Add(Level.Deserialize(reader));
        }

        newStory.PlayerCurrentBuildInfo = BuildInfo.Deserialize(reader);
        newStory.PlayerCurrentUnlockedBuildInfo = BuildInfo.Deserialize(reader);

        int buildCount = reader.ReadSInt32();
        newStory.PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();
        for (int i = 0; i < buildCount; i++)
        {
            BuildInfo bi = BuildInfo.Deserialize(reader);
            newStory.PlayerBuildInfos.Add(bi.BuildID, bi);
        }

        newStory.StoryGamePlaySettings = GamePlaySettings.Deserialize(reader);

        int beatBossPicIDCount = reader.ReadSInt32();
        for (int i = 0; i < beatBossPicIDCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int bossPicID = reader.ReadSInt32();
            newStory.LevelBeatBossPicIDs.Add(levelID, bossPicID);
        }

        int levelFightTimCount = reader.ReadSInt32();
        for (int i = 0; i < levelFightTimCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int times = reader.ReadSInt32();
            newStory.LevelNumFightTimes.Add(levelID, times);
        }


        int levelBossRemainCount = reader.ReadSInt32();
        for (int i = 0; i < levelBossRemainCount; i++)
        {
            int levelNum = reader.ReadSInt32();
            int bossCount = reader.ReadSInt32();
            List<int> bossPicIDs = new List<int>();
            for (int j = 0; j < bossCount; j++)
            {
                bossPicIDs.Add(reader.ReadSInt32());
            }

            newStory.LevelNumBossRemain.Add(levelNum, bossPicIDs);
        }

        int levelBigBossRemainCount = reader.ReadSInt32();
        for (int i = 0; i < levelBigBossRemainCount; i++)
        {
            int levelNum = reader.ReadSInt32();
            int bossCount = reader.ReadSInt32();
            List<int> bossPicIDs = new List<int>();
            for (int j = 0; j < bossCount; j++)
            {
                bossPicIDs.Add(reader.ReadSInt32());
            }

            newStory.LevelNumBigBossRemain.Add(levelNum, bossPicIDs);
        }

        int levelBossCountCount = reader.ReadSInt32();
        for (int i = 0; i < levelBossCountCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int bossCount = reader.ReadSInt32();
            newStory.LevelBossCount.Add(levelID, bossCount);
        }

        int levelUnlockBossCount = reader.ReadSInt32();
        for (int i = 0; i < levelUnlockBossCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int bossCount = reader.ReadSInt32();
            List<int> bossPicIDs = new List<int>();
            for (int j = 0; j < bossCount; j++)
            {
                bossPicIDs.Add(reader.ReadSInt32());
            }

            newStory.LevelUnlockBossInfo.Add(levelID, bossPicIDs);
        }

        return newStory;
    }
}

public enum StoryLevelType
{
    Soldier,
    Boss,
    Shop
}
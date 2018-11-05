﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Story
{
    public string StoryName;
    public List<Level> Levels = new List<Level>();

    public BuildInfo PlayerCurrentBuildInfo;
    public BuildInfo PlayerCurrentUnlockedBuildInfo;

    public SortedDictionary<int, BuildInfo> PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();

    public GamePlaySettings StoryGamePlaySettings;

    public int PlayerCurrentLevel = 0;
    public SortedDictionary<int, int> PlayerBeatBossID = new SortedDictionary<int, int>();

    public Story()
    {
    }

    public Story(string storyName, List<Level> levels, BuildInfo playerCurrentBuildInfo, BuildInfo playerCurrentUnlockedBuildInfo, GamePlaySettings storyGamePlaySettings, int playerCurrentLevel, SortedDictionary<int, int> playerBeatBossID)
    {
        StoryName = storyName;
        Levels = levels;
        PlayerCurrentBuildInfo = playerCurrentBuildInfo;
        PlayerCurrentUnlockedBuildInfo = playerCurrentUnlockedBuildInfo;
        StoryGamePlaySettings = storyGamePlaySettings;

        PlayerCurrentLevel = playerCurrentLevel;
        PlayerBeatBossID = playerBeatBossID;
    }

    public Story Variant()//变换关卡
    {
        List<Level> newLevels = new List<Level>();
        foreach (Level level in Levels)
        {
            Level newLevel = level.Clone();
            HashSet<int> selectBoss = new HashSet<int>();
            Random rd = new Random(level.LevelID * DateTime.Now.Millisecond);
            if (level.Bosses.Count >= 3)//如果boss数量大于等于3个，则挑选2~3个boss加入
            {
                int bossCount = rd.Next(2, 4);
                for (int j = 0; j < bossCount; j++)
                {
                    int nextBossIndex = rd.Next(0, level.Bosses.Count);
                    while (selectBoss.Contains(nextBossIndex))
                    {
                        nextBossIndex = rd.Next(0, level.Bosses.Count);
                    }
                    selectBoss.Add(nextBossIndex);
                }
            }
            else
            {
                for (int i = 0; i < level.Bosses.Count; i++)
                {
                    selectBoss.Add(i);
                }
            }
            newLevel.Bosses.Clear();

            foreach (int bossIndex in selectBoss)
            {
                newLevel.Bosses.Add(level.Bosses[bossIndex]);
            }
            newLevels.Add(newLevel);
        }

        return new Story(StoryName, newLevels, PlayerCurrentBuildInfo.Clone(), PlayerCurrentUnlockedBuildInfo.Clone(), StoryGamePlaySettings.Clone(), PlayerCurrentLevel, PlayerBeatBossID);
    }

    public Story Clone()
    {
        List<Level> newLevels = new List<Level>();
        foreach (Level level in Levels)
        {
            newLevels.Add(level.Clone());
        }

        return new Story(StoryName, newLevels, PlayerCurrentBuildInfo.Clone(), PlayerCurrentUnlockedBuildInfo.Clone(), StoryGamePlaySettings.Clone(), PlayerCurrentLevel, PlayerBeatBossID);
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

        writer.WriteSInt32(PlayerCurrentLevel);

        writer.WriteSInt32(PlayerBeatBossID.Count);
        foreach (KeyValuePair<int, int> levelAndBoss in PlayerBeatBossID)
        {
            writer.WriteSInt32(levelAndBoss.Key);
            writer.WriteSInt32(levelAndBoss.Value);
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

        newStory.PlayerCurrentLevel = reader.ReadSInt32();
        int beatLevelCount = reader.ReadSInt32();
        for (int i = 0; i < beatLevelCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int bossID = reader.ReadSInt32();
            newStory.PlayerBeatBossID.Add(levelID, bossID);
        }

        return newStory;
    }
}
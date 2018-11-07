using System;
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
    public SortedDictionary<int, int> PlayerBeatBossIDs = new SortedDictionary<int, int>(); //记录玩家通关每一层击败的是哪个boss， Key: LevelID, Value: BossPicID

    public SortedDictionary<int, int> PlayerBeatBossPicIDs = new SortedDictionary<int, int>(); //记录玩家击败的Boss的图片ID，如果击败过，后续游戏就不再出现，以免带来重复感. Key : LevelID, Value: BossPicID

    public Story()
    {
    }

    public Story(string storyName, List<Level> levels, BuildInfo playerCurrentBuildInfo, BuildInfo playerCurrentUnlockedBuildInfo, GamePlaySettings storyGamePlaySettings, int playerCurrentLevel, SortedDictionary<int, int> playerBeatBossIDs, SortedDictionary<int, int> playerBeatBossPicIDs)
    {
        StoryName = storyName;
        Levels = levels;
        PlayerCurrentBuildInfo = playerCurrentBuildInfo;
        PlayerCurrentUnlockedBuildInfo = playerCurrentUnlockedBuildInfo;
        StoryGamePlaySettings = storyGamePlaySettings;

        PlayerCurrentLevel = playerCurrentLevel;
        PlayerBeatBossIDs = playerBeatBossIDs;
        PlayerBeatBossPicIDs = playerBeatBossPicIDs;
    }

    public Story Variant() //变换关卡
    {
        List<Level> newLevels = new List<Level>();
        Levels.ForEach(level => { newLevels.Add(level.Clone()); });
        return new Story(StoryName, newLevels, PlayerCurrentBuildInfo.Clone(), PlayerCurrentUnlockedBuildInfo.Clone(), StoryGamePlaySettings.Clone(), PlayerCurrentLevel, new SortedDictionary<int, int>(), new SortedDictionary<int, int>());
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

        writer.WriteSInt32(PlayerBeatBossIDs.Count);
        foreach (KeyValuePair<int, int> levelAndBoss in PlayerBeatBossIDs)
        {
            writer.WriteSInt32(levelAndBoss.Key);
            writer.WriteSInt32(levelAndBoss.Value);
        }

        writer.WriteSInt32(PlayerBeatBossPicIDs.Count);
        foreach (KeyValuePair<int, int> levelIDAndBossPicID in PlayerBeatBossPicIDs)
        {
            writer.WriteSInt32(levelIDAndBossPicID.Key);
            writer.WriteSInt32(levelIDAndBossPicID.Value);
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
            newStory.PlayerBeatBossIDs.Add(levelID, bossID);
        }

        int beatBossPicIDCount = reader.ReadSInt32();
        for (int i = 0; i < beatBossPicIDCount; i++)
        {
            int levelID = reader.ReadSInt32();
            int bossPicID = reader.ReadSInt32();
            newStory.PlayerBeatBossPicIDs.Add(levelID, bossPicID);
        }

        return newStory;
    }
}
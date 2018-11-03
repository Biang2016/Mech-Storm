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

    public Story Clone()
    {
        List<Level> newLevels = new List<Level>();
        foreach (Level level in Levels)
        {
            newLevels.Add(level.Clone());
        }

        return new Story(StoryName, newLevels, PlayerCurrentBuildInfo.Clone(), PlayerCurrentUnlockedBuildInfo.Clone(), StoryGamePlaySettings.Clone());
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

        return newStory;
    }

    public string DeserializeLog()
    {
        string log = "";
        log += " [StoryName]" + StoryName;
        log += " [Levels]";
        foreach (Level level in Levels)
        {
            log += " [level]=" + level.DeserializeLog();
        }

        log += " [PlayerCurrentBuildInfo]=" + PlayerCurrentBuildInfo.DeserializeLog();
        log += " [PlayerCurrentUnlockedBuildInfo]=" + PlayerCurrentUnlockedBuildInfo.DeserializeLog();

        log += " [PlayerBuildInfos]=";
        foreach (BuildInfo playerBuildInfo in PlayerBuildInfos.Values)
        {
            log += playerBuildInfo.DeserializeLog();
        }

        log += " [StoryGamePlaySettings]=" + StoryGamePlaySettings.DeserializeLog();
        return log;
    }
}
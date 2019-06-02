using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class Story : IClone<Story>, IVariant<Story>
{
    public string StoryName;
    public SortedDictionary<int, Chapter> Chapters = new SortedDictionary<int, Chapter>();

    public SortedDictionary<int, int> Base_CardLimitDict = new SortedDictionary<int, int>(); // 玩家牌库已解锁各牌的上限信息（以基本牌为key）
    public SortedDictionary<int, BuildInfo> PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();

    public GamePlaySettings StoryGamePlaySettings;

    private Story()
    {
    }

    public Story(string storyName, SortedDictionary<int, Chapter> chapters, SortedDictionary<int, BuildInfo> playerBuildInfos, GamePlaySettings storyGamePlaySettings)
    {
        StoryName = storyName;
        Chapters = chapters;
        PlayerBuildInfos = playerBuildInfos;

        foreach (KeyValuePair<int, BuildInfo> kv in playerBuildInfos)
        {
            Base_CardLimitDict = kv.Value.M_BuildCards.GetBaseCardLimitDict();
            break;
        }
        StoryGamePlaySettings = storyGamePlaySettings;
        RefreshLevelPointer();
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public void RefreshBaseCardLimitDict()
    {

    }

    public Story Variant() //变换关卡
    {
        SortedDictionary<int, BuildInfo> newPlayerBuildInfos = CloneVariantUtils.SortedDictionary(PlayerBuildInfos);
        foreach (KeyValuePair<int, BuildInfo> kv in newPlayerBuildInfos)
        {
            kv.Value.BuildID = kv.Key;
        }

        SortedDictionary<int, Chapter> newChapters = CloneVariantUtils.SortedDictionary(Chapters, CloneVariantUtils.OperationType.Variant);
        foreach (KeyValuePair<int, Chapter> kv in newChapters)
        {
            kv.Value.ChapterID = kv.Key;
        }

        Story newStory = new Story(
            StoryName,
            newChapters,
            newPlayerBuildInfos,
            StoryGamePlaySettings.Clone());
        return newStory;
    }

    public Story Clone()
    {
        SortedDictionary<int, Chapter> newChapters = CloneVariantUtils.SortedDictionary(Chapters);
        foreach (KeyValuePair<int, Chapter> kv in newChapters)
        {
            kv.Value.ChapterID = kv.Key;
        }

        Story newStory = new Story(
            StoryName,
            newChapters,
            PlayerBuildInfos, StoryGamePlaySettings.Clone());
        return newStory;
    }

    public void ExportToXML(XmlDocument doc)
    {
        XmlElement story_ele = doc.CreateElement("Story");
        doc.AppendChild(story_ele);
        foreach (KeyValuePair<int, BuildInfo> kv in PlayerBuildInfos)
        {
            kv.Value.ExportToXML(story_ele);
            break;
        }

        StoryGamePlaySettings.ExportToXML(story_ele);

        XmlElement chapters_ele = doc.CreateElement("Chapters");
        story_ele.AppendChild(chapters_ele);
        foreach (KeyValuePair<int, Chapter> kv in Chapters)
        {
            kv.Value.ExportToXML(chapters_ele);
        }
    }

    private void RefreshLevelPointer()
    {
        foreach (KeyValuePair<int, Chapter> kv in Chapters)
        {
            kv.Value.InfoRefresh = InfoRefresh;
            foreach (KeyValuePair<int, Level> _kv in kv.Value.Levels)
            {
                _kv.Value.InfoRefresh = InfoRefresh;
            }
        }
    }

    public delegate string InfoRefreshDelegate();

    public InfoRefreshDelegate InfoRefresh; // 信息更新委托

    public void BeatChapter(int chapterID)
    {
        //TODO

        InfoRefresh();
    }

    public void EditAllCardLimitDict(int cardID, int changeValue) // 更改某卡牌上限数量
    {
        //TODO 减少上限时要删掉对应的牌
        foreach (KeyValuePair<int, BuildInfo> kv in PlayerBuildInfos)
        {
            SortedDictionary<int, BuildInfo.BuildCards.CardSelectInfo> csis = kv.Value.M_BuildCards.CardSelectInfos;
            int remainChange = changeValue;
            if (csis[cardID].CardSelectUpperLimit + changeValue >= 0)
            {
                csis[cardID].CardSelectUpperLimit += changeValue;
            }
            else
            {
                remainChange += csis[cardID].CardSelectUpperLimit;
                csis[cardID].CardSelectUpperLimit = 0;
                List<int> series = AllCards.GetCardSeries(cardID);
                foreach (int i in series)
                {
                    if (csis[i].CardSelectUpperLimit + remainChange >= 0)
                    {
                        csis[i].CardSelectUpperLimit += remainChange;
                        break;
                    }
                    else
                    {
                        remainChange += csis[i].CardSelectUpperLimit;
                        csis[i].CardSelectUpperLimit = 0;
                    }
                }
            }
        }

        int baseCardID = AllCards.GetCardBaseCardID(cardID);
        Base_CardLimitDict[baseCardID] += changeValue;

        InfoRefresh();
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteString8(StoryName);
        writer.WriteSInt32(Chapters.Count);
        foreach (KeyValuePair<int, Chapter> kv in Chapters)
        {
            kv.Value.Serialize(writer);
        }

        writer.WriteSInt32(Base_CardLimitDict.Count);
        foreach (KeyValuePair<int, int> kv in Base_CardLimitDict)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }

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
        int chapterCount = reader.ReadSInt32();
        newStory.Chapters = new SortedDictionary<int, Chapter>();
        for (int i = 0; i < chapterCount; i++)
        {
            Chapter chapter = Chapter.Deserialize(reader);
            if (newStory.Chapters.ContainsKey(chapter.ChapterID))
            {
                Utils.DebugLog("Duplicate! chapter.ChapterID: " + chapter.ChapterID);
            }
            else
            {
                newStory.Chapters.Add(chapter.ChapterID, chapter);
            }
        }

        int cldCount = reader.ReadSInt32();
        newStory.Base_CardLimitDict = new SortedDictionary<int, int>();
        for (int i = 0; i < cldCount; i++)
        {
            int key = reader.ReadSInt32();
            int value = reader.ReadSInt32();
            if (newStory.Base_CardLimitDict.ContainsKey(key))
            {
                Utils.DebugLog("Duplicate! Base_CardLimitDict.key: " + key);
            }
            else
            {
                newStory.Base_CardLimitDict.Add(key, value);
            }
        }

        int buildCount = reader.ReadSInt32();
        newStory.PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();
        for (int i = 0; i < buildCount; i++)
        {
            BuildInfo bi = BuildInfo.Deserialize(reader);
            if (newStory.PlayerBuildInfos.ContainsKey(bi.BuildID))
            {
                Utils.DebugLog("Duplicate! bi.BuildID: " + bi.BuildID);
            }
            else
            {
                newStory.PlayerBuildInfos.Add(bi.BuildID, bi);
            }
        }

        newStory.StoryGamePlaySettings = GamePlaySettings.Deserialize(reader);

        return newStory;
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public void DeleteLevel(Level level)
    {
        SortedDictionary<int, List<int>> removeLevelChapterIDLevelID = new SortedDictionary<int, List<int>>();
        foreach (KeyValuePair<int, Chapter> kv in Chapters)
        {
            removeLevelChapterIDLevelID.Add(kv.Key, new List<int>());
            foreach (KeyValuePair<int, Level> _kv in kv.Value.Levels)
            {
                if (_kv.Value.LevelNames["en"].Equals(level.LevelNames["en"]))
                {
                    removeLevelChapterIDLevelID[kv.Key].Add(_kv.Key);
                }
            }
        }

        foreach (KeyValuePair<int, List<int>> kv in removeLevelChapterIDLevelID)
        {
            Chapter chapter = Chapters[kv.Key];
            foreach (int i in kv.Value)
            {
                chapter.Levels.Remove(i);
            }

            chapter.RefreshLevelMap();
        }
    }

  

    /// <summary>
    /// Can only be used in StoryEditor!!
    /// </summary>
    public void DeleteCard(int cardID)
    {
    }
}
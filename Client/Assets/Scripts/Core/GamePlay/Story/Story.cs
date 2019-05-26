using System;
using System.Collections.Generic;
using System.Linq;

public class Story : IClone<Story>, IVariant<Story>
{
    public string StoryName;
    public SortedDictionary<int, Chapter> Chapters = new SortedDictionary<int, Chapter>();
    public SortedDictionary<int, Level> StoryLevels = new SortedDictionary<int, Level>(); //方便查询的数据结构，不进行序列化

    public SortedDictionary<int, int> Base_CardLimitDict = new SortedDictionary<int, int>(); // 玩家牌库已解锁各牌的上限信息（以基本牌为key）
    public SortedDictionary<int, BuildInfo> PlayerBuildInfos = new SortedDictionary<int, BuildInfo>();

    public GamePlaySettings StoryGamePlaySettings;

    private int levelIDGenerator = 1000;

    public int GetLevelID()
    {
        return levelIDGenerator++;
    }

    private Story()
    {
    }

    public Story(string storyName, SortedDictionary<int, Chapter> chapters, SortedDictionary<int, int> base_CardLimitDict, SortedDictionary<int, BuildInfo> playerBuildInfos, GamePlaySettings storyGamePlaySettings)
    {
        levelIDGenerator = 1000;
        StoryName = storyName;
        Chapters = chapters;
        PlayerBuildInfos = playerBuildInfos;
        Base_CardLimitDict = base_CardLimitDict;
        StoryGamePlaySettings = storyGamePlaySettings;
        RefreshLevelPointer();
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
            CloneVariantUtils.SortedDictionary(Base_CardLimitDict),
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
            CloneVariantUtils.SortedDictionary(Base_CardLimitDict),
            PlayerBuildInfos, StoryGamePlaySettings.Clone());
        return newStory;
    }

    private void RefreshLevelPointer()
    {
        foreach (KeyValuePair<int, Chapter> kv in Chapters)
        {
            foreach (KeyValuePair<EnemyType, SortedDictionary<int, Enemy>> KV in kv.Value.ChapterAllEnemies)
            {
                foreach (KeyValuePair<int, Enemy> _kv in KV.Value)
                {
                    _kv.Value.M_Story = this;
                    _kv.Value.LevelID = GetLevelID();
                    StoryLevels.Add(_kv.Value.LevelID, _kv.Value);
                }
            }

            foreach (KeyValuePair<int, Shop> KV in kv.Value.ChapterAllShops)
            {
                KV.Value.M_Story = this;
                KV.Value.LevelID = GetLevelID();
                StoryLevels.Add(KV.Value.LevelID, KV.Value);
            }
        }
    }

    public delegate string StoryInfoRefreshDelegate();

    public StoryInfoRefreshDelegate StoryInfoRefresh; // 信息更新委托

    public void BeatEnemy(Enemy enemy)
    {
        //TODO

        StoryInfoRefresh();
    }

    public void VisitShop()
    {
    }

    public void BeatChapter(Chapter chapter)
    {
        //TODO

        StoryInfoRefresh();
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

        StoryInfoRefresh();
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
}
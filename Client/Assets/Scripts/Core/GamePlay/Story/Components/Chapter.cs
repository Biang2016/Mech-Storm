using System;
using System.Collections.Generic;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID; //章节层号
    public SortedDictionary<string, string> ChapterNames;
    public SortedDictionary<int, Level> Levels = new SortedDictionary<int, Level>();

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<string, string> chapterNames, SortedDictionary<int, Level> chapterAllLevels)
    {
        ChapterID = chapterID;
        ChapterNames = chapterNames;
        Levels = chapterAllLevels;
        //TODO 分配LevelID
    }

    public Story.InfoRefreshDelegate InfoRefresh; // 信息更新委托

    public void LevelVisited(int levelID)
    {
        //TODO

        InfoRefresh();
    }

    public Chapter Variant()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(Levels, CloneVariantUtils.OperationType.Variant);
        foreach (KeyValuePair<int, Level> kv in New_ChapterAllStoryPaces)
        {
            kv.Value.LevelID = kv.Key;
        }

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), New_ChapterAllStoryPaces);
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(Levels);
        foreach (KeyValuePair<int, Level> kv in New_ChapterAllStoryPaces)
        {
            kv.Value.LevelID = kv.Key;
        }
        //TODO 各pace之间的连接关系Clone

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), New_ChapterAllStoryPaces);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(ChapterNames.Count);
        foreach (KeyValuePair<string, string> kv in ChapterNames)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }

        writer.WriteSInt32(Levels.Count);
        foreach (KeyValuePair<int, Level> KV in Levels)
        {
            writer.WriteSInt32(KV.Key);
            KV.Value.Serialize(writer);
        }
    }

    public static Chapter Deserialize(DataStream reader)
    {
        int chapterID = reader.ReadSInt32();
        SortedDictionary<string, string> chapterNames = new SortedDictionary<string, string>();
        int nameCount = reader.ReadSInt32();
        for (int i = 0; i < nameCount; i++)
        {
            string lang = reader.ReadString8();
            string chapterName = reader.ReadString8();
            chapterNames.Add(lang, chapterName);
        }

        int count = reader.ReadSInt32();
        SortedDictionary<int, Level> chapterAllStoryPaces = new SortedDictionary<int, Level>();
        for (int i = 0; i < count; i++)
        {
            int storyPaceID = reader.ReadSInt32();
            Level storyPace = Level.BaseDeserialize(reader);
            chapterAllStoryPaces.Add(storyPaceID, storyPace);
        }

        return new Chapter(chapterID, chapterNames, chapterAllStoryPaces);
    }
}
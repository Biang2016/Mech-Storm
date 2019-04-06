using System;
using System.Collections.Generic;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID;
    public SortedDictionary<int, StoryPace> ChapterAllStoryPaces = new SortedDictionary<int, StoryPace>();
    public SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> ChapterAllEnemies = new SortedDictionary<EnemyType, SortedDictionary<int, Enemy>>();
    public SortedDictionary<int, Shop> ChapterAllShops = new SortedDictionary<int, Shop>();

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<int, StoryPace> chapterAllStoryPaces)
    {
        ChapterID = chapterID;
        ChapterAllStoryPaces = chapterAllStoryPaces;
        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            ChapterAllEnemies.Add(enemyType, new SortedDictionary<int, Enemy>());
        }

        foreach (KeyValuePair<int, StoryPace> kv in ChapterAllStoryPaces)
        {
            switch (kv.Value.StoryPaceType)
            {
                case StoryPaceType.Enemy:
                {
                    Enemy enemy = (Enemy) kv.Value;
                    ChapterAllEnemies[enemy.EnemyType].Add(enemy.StoryPaceID, enemy);
                    break;
                }
                case StoryPaceType.Shop:
                {
                    Shop shop = (Shop) kv.Value;
                    ChapterAllShops.Add(shop.StoryPaceID, shop);
                    break;
                }
            }
        }
    }

    public Chapter Variant()
    {
        SortedDictionary<int, StoryPace> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(ChapterAllStoryPaces, CloneVariantUtils.OperationType.Variant);

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, New_ChapterAllStoryPaces);
    }

    public Chapter Clone()
    {
        SortedDictionary<int, StoryPace> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(ChapterAllStoryPaces);

        //TODO 各pace之间的连接关系Clone

        return new Chapter(ChapterID, New_ChapterAllStoryPaces);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(ChapterAllStoryPaces.Count);
        foreach (KeyValuePair<int, StoryPace> KV in ChapterAllStoryPaces)
        {
            writer.WriteSInt32(KV.Key);
            KV.Value.Serialize(writer);
        }
    }

    public static Chapter Deserialize(DataStream reader)
    {
        int chapterID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        SortedDictionary<int, StoryPace> chapterAllStoryPaces = new SortedDictionary<int, StoryPace>();
        for (int i = 0; i < count; i++)
        {
            int storyPaceID = reader.ReadSInt32();
            StoryPace storyPace = StoryPace.BaseDeserialize(reader);
            chapterAllStoryPaces.Add(storyPaceID, storyPace);
        }

        return new Chapter(chapterID, chapterAllStoryPaces);
    }
}
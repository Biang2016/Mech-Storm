using System;
using System.Collections.Generic;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID;
    public SortedDictionary<int, Level> ChapterAllLevels = new SortedDictionary<int, Level>();
    public SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> ChapterAllEnemies = new SortedDictionary<EnemyType, SortedDictionary<int, Enemy>>();
    public SortedDictionary<int, Shop> ChapterAllShops = new SortedDictionary<int, Shop>();

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<int, Level> chapterAllLevels)
    {
        ChapterID = chapterID;
        ChapterAllLevels = chapterAllLevels;
        foreach (EnemyType enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            ChapterAllEnemies.Add(enemyType, new SortedDictionary<int, Enemy>());
        }

        foreach (KeyValuePair<int, Level> kv in ChapterAllLevels)
        {
            switch (kv.Value.LevelType)
            {
                case LevelType.Enemy:
                {
                    Enemy enemy = (Enemy) kv.Value;
                    ChapterAllEnemies[enemy.EnemyType].Add(enemy.LevelID, enemy);
                    break;
                }
                case LevelType.Shop:
                {
                    Shop shop = (Shop) kv.Value;
                    ChapterAllShops.Add(shop.LevelID, shop);
                    break;
                }
            }
        }
    }

    public Chapter Variant()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(ChapterAllLevels, CloneVariantUtils.OperationType.Variant);

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, New_ChapterAllStoryPaces);
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(ChapterAllLevels);

        //TODO 各pace之间的连接关系Clone

        return new Chapter(ChapterID, New_ChapterAllStoryPaces);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(ChapterAllLevels.Count);
        foreach (KeyValuePair<int, Level> KV in ChapterAllLevels)
        {
            writer.WriteSInt32(KV.Key);
            KV.Value.Serialize(writer);
        }
    }

    public static Chapter Deserialize(DataStream reader)
    {
        int chapterID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        SortedDictionary<int, Level> chapterAllStoryPaces = new SortedDictionary<int, Level>();
        for (int i = 0; i < count; i++)
        {
            int storyPaceID = reader.ReadSInt32();
            Level storyPace = Level.BaseDeserialize(reader);
            chapterAllStoryPaces.Add(storyPaceID, storyPace);
        }

        return new Chapter(chapterID, chapterAllStoryPaces);
    }
}
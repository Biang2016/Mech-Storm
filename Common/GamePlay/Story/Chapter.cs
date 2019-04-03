using System.Collections.Generic;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID;
    public SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> ChapterAllEnemies = new SortedDictionary<EnemyType, SortedDictionary<int, Enemy>>();
    public SortedDictionary<int, Shop> ChapterAllShops = new SortedDictionary<int, Shop>();

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> chapterAllEnemies, SortedDictionary<int, Shop> chapterAllShops)
    {
        ChapterID = chapterID;
        ChapterAllEnemies = chapterAllEnemies;
        ChapterAllShops = chapterAllShops;
    }

    public Chapter Variant()
    {
        SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> New_ChapterAllEnemies = CloneVariantUtils.SortedDictionary(ChapterAllEnemies, CloneVariantUtils.OperationType.Variant);
        SortedDictionary<int, Shop> New_ChapterAllShops = CloneVariantUtils.SortedDictionary(ChapterAllShops, CloneVariantUtils.OperationType.Variant);

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, New_ChapterAllEnemies, New_ChapterAllShops);
    }

    public Chapter Clone()
    {
        SortedDictionary<EnemyType, SortedDictionary<int, Enemy>> New_ChapterAllEnemies = CloneVariantUtils.SortedDictionary(ChapterAllEnemies);
        SortedDictionary<int, Shop> New_ChapterAllShops = CloneVariantUtils.SortedDictionary(ChapterAllShops);

        //TODO 各pace之间的连接关系Clone

        return new Chapter(ChapterID, New_ChapterAllEnemies, New_ChapterAllShops);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(ChapterAllEnemies.Count);
        foreach (KeyValuePair<EnemyType, SortedDictionary<int, Enemy>> KV in ChapterAllEnemies)
        {
            writer.WriteSInt32((int) KV.Key);
            writer.WriteSInt32(KV.Value.Count);
            foreach (KeyValuePair<int, Enemy> kv in KV.Value)
            {
                writer.WriteSInt32(kv.Key);
                kv.Value.Serialize(writer);
            }
        }

        writer.WriteSInt32(ChapterAllShops.Count);
        foreach (KeyValuePair<int, Shop> kv in ChapterAllShops)
        {
            writer.WriteSInt32(kv.Key);
            kv.Value.Serialize(writer);
        }
    }

    public static Chapter Deserialize(DataStream reader)
    {
        Chapter newLevel = new Chapter();
        newLevel.ChapterID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            EnemyType enemyType = (EnemyType) reader.ReadSInt32();
            newLevel.ChapterAllEnemies.Add(enemyType, new SortedDictionary<int, Enemy>());
            int _count = reader.ReadSInt32();
            for (int j = 0; j < _count; j++)
            {
                int picID = reader.ReadSInt32();
                Enemy enemy = Enemy.Deserialize(reader);
                newLevel.ChapterAllEnemies[enemyType].Add(picID, enemy);
            }
        }

        count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            Shop shop = Shop.Deserialize(reader);
            newLevel.ChapterAllShops.Add(shop.ShopID, shop);
        }

        return newLevel;
    }
}
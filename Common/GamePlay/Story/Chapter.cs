using System.Collections.Generic;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID;
    public SortedDictionary<int, Enemy> ChapterAllEnemies = new SortedDictionary<int, Enemy>();
    public SortedDictionary<int, Shop> ChapterAllShops = new SortedDictionary<int, Shop>();

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<int, Enemy> chapterAllEnemies, SortedDictionary<int, Shop> chapterAllShops)
    {
        ChapterID = chapterID;
        ChapterAllEnemies = chapterAllEnemies;
        ChapterAllShops = chapterAllShops;
    }

    public Chapter Variant()
    {
        SortedDictionary<int, Enemy> New_ChapterAllEnemies = CloneVariantUtils.SortedDictionary(ChapterAllEnemies, CloneVariantUtils.OperationType.Variant);
        SortedDictionary<int, Shop> New_ChapterAllShops = CloneVariantUtils.SortedDictionary(ChapterAllShops, CloneVariantUtils.OperationType.Variant);

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, New_ChapterAllEnemies, New_ChapterAllShops);
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Enemy> New_ChapterAllEnemies = CloneVariantUtils.SortedDictionary(ChapterAllEnemies);
        SortedDictionary<int, Shop> New_ChapterAllShops = CloneVariantUtils.SortedDictionary(ChapterAllShops);

        //TODO 各pace之间的连接关系Clone

        return new Chapter(ChapterID, New_ChapterAllEnemies, New_ChapterAllShops);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(ChapterAllEnemies.Count);
        foreach (KeyValuePair<int, Enemy> kv in ChapterAllEnemies)
        {
            writer.WriteSInt32(kv.Key);
            kv.Value.Serialize(writer);
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
            Enemy enemy = Enemy.Deserialize(reader);
            newLevel.ChapterAllEnemies.Add(enemy.EnemyPicID,enemy);
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
using System.Collections.Generic;
using System.Xml;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID; //章节层号
    public SortedDictionary<string, string> ChapterNames;
    public SortedDictionary<int, Level> Levels = new SortedDictionary<int, Level>(); // key: 六边形阵图的nodeLocation编号
    public SortedDictionary<int, bool> LevelBeatedDictionary = new SortedDictionary<int, bool>(); // key: 六边形阵图的nodeLocation编号, value: 是否已经过关

    public int ChapterMapRoundCount;
    public const int SystemMaxMapRoundCount = 6;
    public const int SystemMinMapRoundCount = 2;

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<string, string> chapterNames, SortedDictionary<int, Level> chapterAllLevels, int chapterMapRoundCount, SortedDictionary<int, bool> levelBeatedDictionary = null)
    {
        ChapterID = chapterID;
        ChapterNames = chapterNames;
        Levels = chapterAllLevels;

        ChapterMapRoundCount = chapterMapRoundCount;
        RefreshLevelMap();

        if (levelBeatedDictionary == null)
        {
            foreach (KeyValuePair<int, Level> kv in Levels)
            {
                LevelBeatedDictionary.Add(kv.Key, false);
            }
        }
        else
        {
            LevelBeatedDictionary = levelBeatedDictionary;
        }
    }

    public Story.InfoRefreshDelegate InfoRefresh; // 信息更新委托

    public void LevelVisited(int levelID)
    {
        //TODO

        InfoRefresh();
    }

    public void RefreshLevelMap()
    {
        //TODO 分配LevelID  生成关卡关系等
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(Levels);
        foreach (KeyValuePair<int, Level> kv in New_ChapterAllStoryPaces)
        {
            kv.Value.LevelID = kv.Key;
        }
        //TODO 各level之间的连接关系Clone

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), New_ChapterAllStoryPaces, ChapterMapRoundCount);
    }

    public Chapter Variant()
    {
        SortedDictionary<int, Level> New_ChapterAllStoryPaces = CloneVariantUtils.SortedDictionary(Levels, CloneVariantUtils.OperationType.Variant);
        foreach (KeyValuePair<int, Level> kv in New_ChapterAllStoryPaces)
        {
            kv.Value.LevelID = kv.Key;
        }

        //TODO 各pace之间的连接关系Variant，以及入口关卡设置

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), New_ChapterAllStoryPaces, ChapterMapRoundCount);
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement chapter_ele = doc.CreateElement("Chapter");
        parent_ele.AppendChild(chapter_ele);

        chapter_ele.SetAttribute("chapterID", ChapterID.ToString());
        chapter_ele.SetAttribute("name_en", ChapterNames["en"]);
        chapter_ele.SetAttribute("name_zh", ChapterNames["zh"]);

        XmlElement enemies_ele = doc.CreateElement("EnemyInfos");
        chapter_ele.AppendChild(enemies_ele);

        XmlElement shops_ele = doc.CreateElement("ShopInfos");
        chapter_ele.AppendChild(shops_ele);

        foreach (KeyValuePair<int, Level> kv in Levels)
        {
            if (kv.Value.LevelType == LevelType.Enemy)
            {
                XmlElement enemy_ele = doc.CreateElement("EnemyInfo");
                enemies_ele.AppendChild(enemy_ele);
                enemy_ele.SetAttribute("id", kv.Key.ToString());
                enemy_ele.SetAttribute("name", ((Enemy) kv.Value).LevelNames["en"]);
            }
            else if (kv.Value.LevelType == LevelType.Shop)
            {
                XmlElement shop_ele = doc.CreateElement("ShopInfo");
                shops_ele.AppendChild(shop_ele);
                shop_ele.SetAttribute("id", kv.Key.ToString());
                shop_ele.SetAttribute("name", ((Shop) kv.Value).LevelNames["en"]);
            }
        }

        chapter_ele.SetAttribute("chapterMapRoundCount", ChapterMapRoundCount.ToString());
    }

    public static Chapter GetChapterFromXML(XmlNode node_ChapterInfo)
    {
        int chapterID = int.Parse(node_ChapterInfo.Attributes["chapterID"].Value);
        string Name_zh = node_ChapterInfo.Attributes["name_zh"].Value;
        string Name_en = node_ChapterInfo.Attributes["name_en"].Value;
        SortedDictionary<string, string> names = new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}};

        SortedDictionary<int, Level> allLevels = new SortedDictionary<int, Level>();

        XmlNode node_EnemyInfos = node_ChapterInfo.ChildNodes[0];
        XmlNode node_ShopInfos = node_ChapterInfo.ChildNodes[1];

        for (int i = 0; i < node_EnemyInfos.ChildNodes.Count; i++)
        {
            XmlNode enemyInfo = node_EnemyInfos.ChildNodes[i];
            Enemy enemy = (Enemy) AllLevels.GetLevel(LevelType.Enemy, enemyInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            int id = int.Parse(enemyInfo.Attributes["id"].Value);
            allLevels.Add(id, enemy);
        }

        for (int i = 0; i < node_ShopInfos.ChildNodes.Count; i++)
        {
            XmlNode shopInfo = node_ShopInfos.ChildNodes[i];
            Shop shop = (Shop) AllLevels.GetLevel(LevelType.Shop, shopInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            int id = int.Parse(shopInfo.Attributes["id"].Value);
            allLevels.Add(id, shop);
        }

        int chapterMapRoundCount = int.Parse(node_ChapterInfo.Attributes["chapterMapRoundCount"].Value);

        Chapter chapter = new Chapter(chapterID, names, allLevels, chapterMapRoundCount);
        return chapter;
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
        foreach (KeyValuePair<int, Level> kv in Levels)
        {
            writer.WriteSInt32(kv.Key);
            kv.Value.Serialize(writer);
        }

        writer.WriteSInt32(ChapterMapRoundCount);

        writer.WriteSInt32(LevelBeatedDictionary.Count);
        foreach (KeyValuePair<int, bool> kv in LevelBeatedDictionary)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteByte((byte) (kv.Value ? 0x01 : 0x00));
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
        SortedDictionary<int, Level> chapterAllLevels = new SortedDictionary<int, Level>();
        for (int i = 0; i < count; i++)
        {
            int levelID = reader.ReadSInt32();
            Level level = Level.BaseDeserialize(reader);
            chapterAllLevels.Add(levelID, level);
        }

        int chapterMapRoundCount = reader.ReadSInt32();

        count = reader.ReadSInt32();
        SortedDictionary<int, bool> levelBeatedDictionary = new SortedDictionary<int, bool>();
        for (int i = 0; i < count; i++)
        {
            int levelID = reader.ReadSInt32();
            bool isBeated = reader.ReadByte() == 0x01;
            levelBeatedDictionary.Add(levelID, isBeated);
        }

        return new Chapter(chapterID, chapterNames, chapterAllLevels, chapterMapRoundCount, levelBeatedDictionary);
    }
}
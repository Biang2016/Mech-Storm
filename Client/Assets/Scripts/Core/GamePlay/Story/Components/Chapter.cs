using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NSubstitute.Routing;

public class Chapter : IClone<Chapter>, IVariant<Chapter>
{
    public int ChapterID; //章节层号
    public SortedDictionary<string, string> ChapterNames;
    public SortedDictionary<int, Level> Levels = new SortedDictionary<int, Level>(); // key: 六边形阵图的nodeLocation编号
    public SortedDictionary<int, HashSet<int>> AllRoutes = new SortedDictionary<int, HashSet<int>>(); // key: node location， value目标node location 进行持久化，代表所有可能的Route
    public SortedDictionary<int, HashSet<int>> Routes = new SortedDictionary<int, HashSet<int>>(); // key: node location， value目标node location 每次Variant都不相同，不进行持久化
    public SortedDictionary<int, bool> LevelBeatedDictionary = new SortedDictionary<int, bool>(); // key: 六边形阵图的nodeLocation编号, value: 是否已经过关

    private int chapterMapRoundCount;

    //Can only be modified in Story Editor
    public int ChapterMapRoundCount
    {
        get { return chapterMapRoundCount; }
        set
        {
            chapterMapRoundCount = value;
            Routes.Clear();
            AllRoutes.Clear();
        }
    }

    public const int SystemMaxMapRoundCount = 6;
    public const int SystemMinMapRoundCount = 2;

    private Chapter()
    {
    }

    public Chapter(int chapterID, SortedDictionary<string, string> chapterNames, SortedDictionary<int, Level> chapterAllLevels, int chapterMapRoundCount, SortedDictionary<int, HashSet<int>> allRoutes, SortedDictionary<int, bool> levelBeatedDictionary = null, SortedDictionary<int, HashSet<int>> routes = null)
    {
        ChapterID = chapterID;
        ChapterNames = chapterNames;
        Levels = chapterAllLevels;

        ChapterMapRoundCount = chapterMapRoundCount;

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

        AllRoutes = allRoutes;
        if (routes == null)
        {
            Routes = new SortedDictionary<int, HashSet<int>>();
        }
        else
        {
            Routes = routes;
        }
    }

    public Chapter Clone()
    {
        SortedDictionary<int, Level> levels = CloneVariantUtils.SortedDictionary(Levels);
        foreach (KeyValuePair<int, Level> kv in levels)
        {
            kv.Value.LevelID = kv.Key;
        }

        SortedDictionary<int, HashSet<int>> allRoutes = new SortedDictionary<int, HashSet<int>>();
        foreach (KeyValuePair<int, HashSet<int>> kv in AllRoutes)
        {
            HashSet<int> set = new HashSet<int>();
            foreach (int i in kv.Value)
            {
                set.Add(i);
            }

            allRoutes.Add(kv.Key, set);
        }

        SortedDictionary<int, HashSet<int>> routes = new SortedDictionary<int, HashSet<int>>();
        foreach (KeyValuePair<int, HashSet<int>> kv in Routes)
        {
            HashSet<int> set = new HashSet<int>();
            foreach (int i in kv.Value)
            {
                set.Add(i);
            }

            routes.Add(kv.Key, set);
        }

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), levels, ChapterMapRoundCount, allRoutes, routes: routes);
    }

    public Chapter Variant()
    {
        SortedDictionary<int, Level> levels = RandomizeLevelPosition();
        SortedDictionary<int, HashSet<int>> routes = GenerateMazeRoutesByAllRoutes();
        SortedDictionary<int, HashSet<int>> allRoutes = new SortedDictionary<int, HashSet<int>>();
        foreach (KeyValuePair<int, HashSet<int>> kv in AllRoutes)
        {
            HashSet<int> set = new HashSet<int>();
            foreach (int i in kv.Value)
            {
                set.Add(i);
            }

            allRoutes.Add(kv.Key, set);
        }

        return new Chapter(ChapterID, CloneVariantUtils.SortedDictionary(ChapterNames), levels, ChapterMapRoundCount, allRoutes, routes: routes);
    }

    private SortedDictionary<int, HashSet<int>> GenerateMazeRoutesByAllRoutes() // 根据AllRoutes中提供的图信息，随机生成具有连通性的迷宫，写入Routes中
    {
        int startPoint = 0;
        SortedDictionary<int, HashSet<int>> newRoutes = new SortedDictionary<int, HashSet<int>>();
        if (AllRoutes.Count != 0)
        {
            // 生成树算法
            int nodeCount = AllRoutes.Count;
            HashSet<int> curNodes = new HashSet<int> {startPoint};
            HashSet<int> adNodes = new HashSet<int>();
            while (curNodes.Count != nodeCount)
            {
                foreach (int i in curNodes)
                {
                    foreach (int ad_node in AllRoutes[i])
                    {
                        if (!curNodes.Contains(ad_node))
                        {
                            adNodes.Add(ad_node);
                        }
                    }
                }

                if (adNodes.Count > 0)
                {
                    int addNode = Utils.GetRandomFromList(adNodes.ToList(), 1)[0];
                    HashSet<int> addNode_adNodes = new HashSet<int>();
                    foreach (int i in AllRoutes[addNode]) // 该新加入到集合内的顶点，是哪几个顶点的邻接顶点
                    {
                        if (curNodes.Contains(i))
                        {
                            addNode_adNodes.Add(i);
                        }
                    }

                    if (addNode_adNodes.Count > 0) // 随机选择一个邻接顶点，记录边信息
                    {
                        int addNode_adNode = Utils.GetRandomFromList(addNode_adNodes.ToList(), 1)[0];
                        if (!newRoutes.ContainsKey(addNode_adNode))
                        {
                            newRoutes.Add(addNode_adNode, new HashSet<int>());
                        }

                        newRoutes[addNode_adNode].Add(addNode);
                        if (!newRoutes.ContainsKey(addNode))
                        {
                            newRoutes.Add(addNode, new HashSet<int>());
                        }

                        newRoutes[addNode].Add(addNode_adNode);
                    }

                    adNodes.Remove(addNode);
                    curNodes.Add(addNode);
                }
            }
        }

        return newRoutes;
    }

    private SortedDictionary<int, Level> RandomizeLevelPosition() // 将各关卡依据等级信息，随机布置到章节地图中
    {
        //todo
        foreach (KeyValuePair<int, Level> kv in Levels)
        {
            kv.Value.LevelID = kv.Key;
        }

        return Levels;
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement chapter_ele = doc.CreateElement("Chapter");
        parent_ele.AppendChild(chapter_ele);

        chapter_ele.SetAttribute("chapterID", ChapterID.ToString());
        chapter_ele.SetAttribute("name_en", ChapterNames["en"]);
        chapter_ele.SetAttribute("name_zh", ChapterNames["zh"]);

        XmlElement routes_ele = doc.CreateElement("RouteInfos");
        chapter_ele.AppendChild(routes_ele);

        XmlElement enemies_ele = doc.CreateElement("EnemyInfos");
        chapter_ele.AppendChild(enemies_ele);

        XmlElement shops_ele = doc.CreateElement("ShopInfos");
        chapter_ele.AppendChild(shops_ele);

        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<int, HashSet<int>> kv in AllRoutes)
        {
            string endIndicesStr = string.Join(",", kv.Value.ToList());
            sb.Append(string.Format("({0}:{1});", kv.Key, endIndicesStr));
        }

        string routeInfoStr = sb.ToString().TrimEnd(";".ToArray());
        routes_ele.SetAttribute("routeInfo", routeInfoStr);

        foreach (KeyValuePair<int, Level> kv in Levels)
        {
            if (kv.Value.LevelType == LevelTypes.Enemy)
            {
                XmlElement enemy_ele = doc.CreateElement("EnemyInfo");
                enemies_ele.AppendChild(enemy_ele);
                enemy_ele.SetAttribute("id", kv.Key.ToString());
                enemy_ele.SetAttribute("name", ((Enemy) kv.Value).LevelNames["en"]);
            }
            else if (kv.Value.LevelType == LevelTypes.Shop)
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

        XmlNode node_RouteInfos = node_ChapterInfo.ChildNodes[0];
        XmlNode node_EnemyInfos = node_ChapterInfo.ChildNodes[1];
        XmlNode node_ShopInfos = node_ChapterInfo.ChildNodes[2];

        SortedDictionary<int, HashSet<int>> allRoutes = new SortedDictionary<int, HashSet<int>>();
        string routesInfoStr = node_RouteInfos.Attributes["routeInfo"].Value;
        string[] nodeHashSetStr = routesInfoStr.Split(';');
        foreach (string s in nodeHashSetStr)
        {
            if (string.IsNullOrEmpty(s)) continue;
            string raw = s.Trim("()".ToCharArray());
            string key = raw.Split(':')[0];

            string hashSetStr = raw.Split(':')[1];
            string[] hashSetStrings = hashSetStr.Split(',');

            HashSet<int> endNodeHashSet = new HashSet<int>();
            foreach (string hashSetString in hashSetStrings)
            {
                endNodeHashSet.Add(int.Parse(hashSetString));
            }

            allRoutes.Add(int.Parse(key), endNodeHashSet);
        }

        for (int i = 0; i < node_EnemyInfos.ChildNodes.Count; i++)
        {
            XmlNode enemyInfo = node_EnemyInfos.ChildNodes[i];
            Enemy enemy = (Enemy) AllLevels.GetLevel(LevelTypes.Enemy, enemyInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            int id = int.Parse(enemyInfo.Attributes["id"].Value);
            allLevels.Add(id, enemy);
        }

        for (int i = 0; i < node_ShopInfos.ChildNodes.Count; i++)
        {
            XmlNode shopInfo = node_ShopInfos.ChildNodes[i];
            Shop shop = (Shop) AllLevels.GetLevel(LevelTypes.Shop, shopInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            int id = int.Parse(shopInfo.Attributes["id"].Value);
            allLevels.Add(id, shop);
        }

        int chapterMapRoundCount = int.Parse(node_ChapterInfo.Attributes["chapterMapRoundCount"].Value);

        Chapter chapter = new Chapter(chapterID, names, allLevels, chapterMapRoundCount, allRoutes);
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

        writer.WriteSInt32(AllRoutes.Count);
        foreach (KeyValuePair<int, HashSet<int>> kv in AllRoutes)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (int i in kv.Value)
            {
                writer.WriteSInt32(i);
            }
        }

        writer.WriteSInt32(Routes.Count);
        foreach (KeyValuePair<int, HashSet<int>> kv in Routes)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value.Count);
            foreach (int i in kv.Value)
            {
                writer.WriteSInt32(i);
            }
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

        count = reader.ReadSInt32();
        SortedDictionary<int, HashSet<int>> allRoutes = new SortedDictionary<int, HashSet<int>>();
        for (int i = 0; i < count; i++)
        {
            int key = reader.ReadSInt32();
            allRoutes.Add(key, new HashSet<int>());
            int valueCount = reader.ReadSInt32();
            for (int j = 0; j < valueCount; j++)
            {
                int value = reader.ReadSInt32();
                allRoutes[key].Add(value);
            }
        }

        count = reader.ReadSInt32();
        SortedDictionary<int, HashSet<int>> routes = new SortedDictionary<int, HashSet<int>>();
        for (int i = 0; i < count; i++)
        {
            int key = reader.ReadSInt32();
            routes.Add(key, new HashSet<int>());
            int valueCount = reader.ReadSInt32();
            for (int j = 0; j < valueCount; j++)
            {
                int value = reader.ReadSInt32();
                routes[key].Add(value);
            }
        }

        return new Chapter(chapterID, chapterNames, chapterAllLevels, chapterMapRoundCount, allRoutes, levelBeatedDictionary, routes: routes);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AllLevels
{
    public static SortedDictionary<LevelType, string> LevelDirectoryDict = new SortedDictionary<LevelType, string>
    {
        {LevelType.Enemy, LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/Enemies/"},
        {LevelType.Shop, LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/Shops/"},
    };

    public static SortedDictionary<LevelType, string> LevelDefaultXMLDict = new SortedDictionary<LevelType, string>
    {
        {LevelType.Enemy, LevelDirectoryDict[LevelType.Enemy] + "DefaultEnemies.xml"},
        {LevelType.Shop, LevelDirectoryDict[LevelType.Shop] + "DefaultShops.xml"},
    };

    public static SortedDictionary<LevelType, SortedDictionary<string, Level>> LevelDict = new SortedDictionary<LevelType, SortedDictionary<string, Level>>
    {
        {LevelType.Enemy, new SortedDictionary<string, Level>()},
        {LevelType.Shop, new SortedDictionary<string, Level>()},
    };

    public static Level GetLevel(LevelType levelType, string levelName, CloneVariantUtils.OperationType operationType)
    {
        if (LevelDict[levelType].ContainsKey(levelName))
        {
            if (operationType == CloneVariantUtils.OperationType.Clone)
            {
                return LevelDict[levelType][levelName].Clone();
            }
            else
            {
                return LevelDict[levelType][levelName].Variant();
            }
        }

        return null;
    }

    public static void Reset()
    {
        foreach (KeyValuePair<LevelType, SortedDictionary<string, Level>> kv in LevelDict)
        {
            kv.Value.Clear();
        }
    }

    private static bool NeedReload = false;

    public static void AddAllLevels()
    {
        Reset();
        foreach (KeyValuePair<LevelType, SortedDictionary<string, Level>> kv in LevelDict)
        {
            LevelType levelType = kv.Key;
            foreach (string path in Directory.GetFiles(LevelDirectoryDict[levelType], "*.xml"))
            {
                string text;
                using (StreamReader sr = new StreamReader(path))
                {
                    text = sr.ReadToEnd();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                XmlElement allLevels = doc.DocumentElement;
                for (int i = 0; i < allLevels.ChildNodes.Count; i++)
                {
                    XmlNode levelInfo = allLevels.ChildNodes.Item(i);
                    Level level = GetLevelFromXML(levelInfo, out bool needRefresh);
                    NeedReload |= needRefresh;
                    kv.Value.Add(level.LevelNames["en"], level);
                }
            }
        }

        //If any problem, refresh XML and reload
        if (NeedReload)
        {
            NeedReload = false;
            RefreshAllLevelXML();
            ReloadLevelXML();
        }
    }

    public static Level GetLevelFromXML(XmlNode node_levelInfo, out bool needRefresh)
    {
        needRefresh = false;
        string Name_zh = node_levelInfo.Attributes["name_zh"].Value;
        string Name_en = node_levelInfo.Attributes["name_en"].Value;
        SortedDictionary<string, string> names = new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}};
        int picID = int.Parse(node_levelInfo.Attributes["picID"].Value);
        LevelType levelType = (LevelType) Enum.Parse(typeof(LevelType), node_levelInfo.Attributes["levelType"].Value);
        LevelThemeCategory levelThemeType = (LevelThemeCategory) Enum.Parse(typeof(LevelThemeCategory), node_levelInfo.Attributes["levelThemeCategory"].Value);
        switch (levelType)
        {
            case LevelType.Enemy:
            {
                XmlNode node_EnemyInfo = node_levelInfo.FirstChild;
                EnemyType enemyType = (EnemyType) Enum.Parse(typeof(EnemyType), node_EnemyInfo.Attributes["enemyType"].Value);

                BuildInfo bi = BuildInfo.GetBuildInfoFromXML(node_EnemyInfo.FirstChild, out bool _needRefresh_build);
                needRefresh |= _needRefresh_build;

                XmlNode node_BonusGroupInfos = node_EnemyInfo.ChildNodes.Item(1);
                List<BonusGroup> BonusGroups = new List<BonusGroup>();
                for (int i = 0; i < node_BonusGroupInfos.ChildNodes.Count; i++)
                {
                    XmlNode bonusGroupInfo = node_BonusGroupInfos.ChildNodes.Item(i);
                    BonusGroup bg = BonusGroup.GenerateBonusGroupFromXML(bonusGroupInfo, out bool _needRefresh_bonus);
                    needRefresh |= _needRefresh_bonus;
                    BonusGroups.Add(bg);
                }

                Enemy enemy = new Enemy(levelThemeType, picID, names, bi, enemyType, 100, BonusGroups);
                return enemy;
            }
            case LevelType.Shop:
            {
                XmlNode node_ShopInfo = node_levelInfo.FirstChild;
                List<ShopItem> shopItems = new List<ShopItem>();
                for (int i = 0; i < node_ShopInfo.ChildNodes.Count; i++)
                {
                    XmlNode node_ShopItem = node_ShopInfo.ChildNodes.Item(i);
                    ShopItem si = ShopItem.GenerateShopItemFromXML(node_ShopItem, out bool _needRefresh_shop);
                    needRefresh |= _needRefresh_shop;
                    if (si != null) shopItems.Add(si);
                }

                Shop shop = new Shop(levelThemeType, picID, names, shopItems);
                return shop;
            }
        }

        return null;
    }

    public static void ReloadLevelXML()
    {
        AddAllLevels();
    }

    public static void RefreshAllLevelXML()
    {
        List<Level> allLevels = new List<Level>();
        foreach (KeyValuePair<LevelType, SortedDictionary<string, Level>> kv in LevelDict)
        {
            foreach (KeyValuePair<string, Level> _kv in kv.Value)
            {
                allLevels.Add(_kv.Value);
            }
        }

        foreach (Level l in allLevels)
        {
            RefreshLevelXML(l);
        }
    }

    public static void RefreshLevelXML(Level level)
    {
        level = level.Clone();
        SortedDictionary<string, Level> dict = LevelDict[level.LevelType];
        if (dict.ContainsKey(level.LevelNames["en"]))
        {
            dict[level.LevelNames["en"]] = level;
        }
        else
        {
            dict.Add(level.LevelNames["en"], level);
        }

        string text;
        using (StreamReader sr = new StreamReader(LevelDefaultXMLDict[level.LevelType]))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allLevels = doc.DocumentElement;
        level.ExportToXML(allLevels);

        using (StreamWriter sw = new StreamWriter(LevelDefaultXMLDict[level.LevelType]))
        {
            doc.Save(sw);
        }
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public static void DeleteLevel(LevelType levelType, string levelName_en)
    {
        string text;
        using (StreamReader sr = new StreamReader(LevelDefaultXMLDict[levelType]))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allLevel = doc.DocumentElement;
        SortedDictionary<string, XmlElement> levelNodesDict = new SortedDictionary<string, XmlElement>();
        foreach (XmlElement node in allLevel.ChildNodes)
        {
            string name = node.Attributes["name_en"].Value;
            if (name != levelName_en)
            {
                levelNodesDict.Add(name, node);
            }
        }

        allLevel.RemoveAll();
        foreach (KeyValuePair<string, XmlElement> kv in levelNodesDict)
        {
            allLevel.AppendChild(kv.Value);
        }

        using (StreamWriter sw = new StreamWriter(LevelDefaultXMLDict[levelType]))
        {
            doc.Save(sw);
        }

        ReloadLevelXML();

        // 从Story中移除该Level
        SortedDictionary<string, SortedDictionary<int, List<int>>> removeList = new SortedDictionary<string, SortedDictionary<int, List<int>>>();

        foreach (KeyValuePair<string, Story> kv in AllStories.StoryDict)
        {
            removeList.Add(kv.Key, new SortedDictionary<int, List<int>>());
            foreach (KeyValuePair<int, Chapter> _kv in kv.Value.Chapters)
            {
                removeList[kv.Key].Add(_kv.Key, new List<int>());
                foreach (KeyValuePair<int, Level> KV in _kv.Value.Levels)
                {
                    if (KV.Value.LevelNames["en"].Equals(levelName_en))
                    {
                        removeList[kv.Key][_kv.Key].Add(KV.Key);
                    }
                }
            }
        }

        foreach (KeyValuePair<string, SortedDictionary<int, List<int>>> kv in removeList)
        {
            Story story = AllStories.StoryDict[kv.Key];
            foreach (KeyValuePair<int, List<int>> _kv in kv.Value)
            {
                Chapter chapter = story.Chapters[_kv.Key];
                foreach (int i in _kv.Value)
                {
                    chapter.Levels.Remove(i);
                }
            }

            AllStories.RefreshStoryXML(story);
            AllStories.ReloadStoryXML();
        }
    }
}
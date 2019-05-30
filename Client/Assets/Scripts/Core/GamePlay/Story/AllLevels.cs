using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AllLevels
{
    private static string EnemyDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/Enemies/";
    private static string ShopDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/Shops/";

    public static Dictionary<string, Enemy> EnemyDict = new Dictionary<string, Enemy>();
    public static Dictionary<string, Shop> ShopDict = new Dictionary<string, Shop>();

    public static void AddAllLevels()
    {
        Reset();
        AddAllEnemies();
        AddAllShops();
    }

    public static Enemy GetEnemy(string enemyName, CloneVariantUtils.OperationType operationType)
    {
        if (EnemyDict.ContainsKey(enemyName))
        {
            if (operationType == CloneVariantUtils.OperationType.Clone)
            {
                return (Enemy) EnemyDict[enemyName].Clone();
            }
            else
            {
                return (Enemy) EnemyDict[enemyName].Variant();
            }
        }

        return null;
    }

    public static Shop GetShop(string shopName)
    {
        if (ShopDict.ContainsKey(shopName))
        {
            return (Shop) ShopDict[shopName].Clone();
        }

        return null;
    }

    private static void AddAllEnemies()
    {
        foreach (string path in Directory.GetFiles(EnemyDirectory, "*.xml"))
        {
            string text;
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement allEnemies = doc.DocumentElement;
            for (int i = 0; i < allEnemies.ChildNodes.Count; i++)
            {
                XmlNode levelInfo = allEnemies.ChildNodes.Item(i);
                Enemy enemy = (Enemy) GetLevelFromXML(levelInfo);
                EnemyDict.Add(enemy.LevelNames["en"], enemy);
            }
        }
    }

    private static void AddAllShops()
    {
        foreach (string path in Directory.GetFiles(ShopDirectory, "*.xml"))
        {
            string text;
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement allShops = doc.DocumentElement;
            for (int i = 0; i < allShops.ChildNodes.Count; i++)
            {
                XmlNode levelInfo = allShops.ChildNodes.Item(i);
                Shop shop = (Shop) GetLevelFromXML(levelInfo);
                ShopDict.Add(shop.LevelNames["en"], shop);
            }
        }
    }

    public static Level GetLevelFromXML(XmlNode node_levelInfo)
    {
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

                BuildInfo bi = AllBuilds.GetBuildInfoFromXML(node_EnemyInfo.FirstChild);

                XmlNode node_BonusGroupInfos = node_EnemyInfo.ChildNodes.Item(1);
                List<BonusGroup> AlwaysBonusGroup = new List<BonusGroup>();
                List<BonusGroup> OptionalBonusGroup = new List<BonusGroup>();
                for (int i = 0; i < node_BonusGroupInfos.ChildNodes.Count; i++)
                {
                    XmlNode bonusGroupInfo = node_BonusGroupInfos.ChildNodes.Item(i);
                    BonusGroup bg = GetBonusGroupFromXML(bonusGroupInfo);

                    if (bg.IsAlways)
                    {
                        AlwaysBonusGroup.Add(bg);
                    }
                    else
                    {
                        OptionalBonusGroup.Add(bg);
                    }
                }

                Enemy enemy = new Enemy(levelThemeType, picID, names, bi, enemyType, 100, AlwaysBonusGroup, OptionalBonusGroup);
                return enemy;
            }
            case LevelType.Shop:
            {
                Shop shop = new Shop(levelThemeType, picID, names);
                return shop;
            }
        }

        return null;
    }

    public static BonusGroup GetBonusGroupFromXML(XmlNode bonusGroupInfo)
    {
        bool isAlways = bonusGroupInfo.Attributes["type"].Value == "Always";
        List<Bonus> bonuses = new List<Bonus>();
        int probability = 0;
        bool singleton = false;
        if (isAlways)
        {
            probability = 0;
            singleton = true;
        }
        else
        {
            probability = int.Parse(bonusGroupInfo.Attributes["probability"].Value);
            singleton = bonusGroupInfo.Attributes["singleton"].Value == "True";
        }

        BonusGroup bg = new BonusGroup(isAlways, bonuses, probability, singleton);
        for (int i = 0; i < bonusGroupInfo.ChildNodes.Count; i++)
        {
            XmlNode bonusInfo = bonusGroupInfo.ChildNodes.Item(i);
            Bonus.BonusType bonusType = (Bonus.BonusType) Enum.Parse(typeof(Bonus.BonusType), bonusInfo.Attributes["name"].Value);
            HardFactorValue bonusBaseValue = new HardFactorValue(int.Parse(bonusInfo.Attributes["value"].Value));
            Bonus bonus = new Bonus(bonusType, bonusBaseValue, 100);
            bg.Bonuses.Add(bonus);
        }

        return bg;
    }

    public static void Reset()
    {
        EnemyDict.Clear();
        ShopDict.Clear();
    }
}
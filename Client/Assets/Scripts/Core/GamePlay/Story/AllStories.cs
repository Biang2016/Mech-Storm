using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AllStories
{
    private static string StoriesDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/";

    public static void AddAllStories()
    {
        Reset();
        foreach (string path in Directory.GetFiles(StoriesDirectory, "*.xml"))
        {
            FileInfo fi = new FileInfo(path);
            string pureName = fi.Name.Substring(0, fi.Name.LastIndexOf("."));

            string text;
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement story = doc.DocumentElement;

            //初始卡组
            string buildName = story.ChildNodes.Item(0).Attributes["BuildName"].Value;
            int playerDefaultBuildId = BuildStoryDatabase.Instance.BuildGroupDict[BuildGroups.StoryBuilds].GetBuildInfo(buildName).BuildID;

            //初始解锁卡片
            XmlNode gpsNode = story.ChildNodes.Item(1);
            int DrawCardPerRound = int.Parse(gpsNode.Attributes["DrawCardPerRound"].Value);

            int DefaultCoin = int.Parse(gpsNode.Attributes["DefaultCoin"].Value);
            int DefaultLife = int.Parse(gpsNode.Attributes["DefaultLife"].Value);
            int DefaultLifeMax = int.Parse(gpsNode.Attributes["DefaultLifeMax"].Value);
            int DefaultLifeMin = int.Parse(gpsNode.Attributes["DefaultLifeMin"].Value);
            int DefaultEnergy = int.Parse(gpsNode.Attributes["DefaultEnergy"].Value);
            int DefaultEnergyMax = int.Parse(gpsNode.Attributes["DefaultEnergyMax"].Value);

            int DefaultDrawCardNum = int.Parse(gpsNode.Attributes["DefaultDrawCardNum"].Value);
            int MinDrawCardNum = int.Parse(gpsNode.Attributes["MinDrawCardNum"].Value);
            int MaxDrawCardNum = int.Parse(gpsNode.Attributes["MaxDrawCardNum"].Value);

            GamePlaySettings gps = new GamePlaySettings();
            gps.DrawCardPerRound = DrawCardPerRound;

            gps.DefaultCoin = DefaultCoin;
            gps.DefaultLife = DefaultLife;
            gps.DefaultLifeMax = DefaultLifeMax;
            gps.DefaultLifeMin = DefaultLifeMin;
            gps.DefaultEnergy = DefaultEnergy;
            gps.DefaultEnergyMax = DefaultEnergyMax;

            gps.DefaultDrawCardNum = DefaultDrawCardNum;
            gps.MinDrawCardNum = MinDrawCardNum;
            gps.MaxDrawCardNum = MaxDrawCardNum;

            SortedDictionary<int, Chapter> Chapters = new SortedDictionary<int, Chapter>();
            for (int i = 2; i < story.ChildNodes.Count; i++)
            {
                Chapter chapter = GetChapter(story.ChildNodes[i]);
                Chapters.Add(chapter.ChapterID, chapter);
            }

            BuildInfo PlayerCurrentBuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(playerDefaultBuildId).Clone();
            SortedDictionary<int, BuildInfo> playerBuildInfos = new SortedDictionary<int, BuildInfo>();
            playerBuildInfos.Add(PlayerCurrentBuildInfo.BuildID, PlayerCurrentBuildInfo);
            Story newStory = new Story(pureName, Chapters, PlayerCurrentBuildInfo.M_BuildCards.GetBaseCardLimitDict(), playerBuildInfos, gps);
            BuildStoryDatabase.Instance.StoryStartDict.Add(pureName, newStory);
        }
    }

    private static Chapter GetChapter(XmlNode chapterInfo)
    {
        int chapterID = int.Parse(chapterInfo.Attributes["chapterID"].Value);
        string Name_zh = chapterInfo.Attributes["name_zh"].Value;
        string Name_en = chapterInfo.Attributes["name_en"].Value;
        SortedDictionary<string, string> names = new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}};

        SortedDictionary<int, Level> allLevels = new SortedDictionary<int, Level>();
        int levelID = 0;
        for (int i = 0; i < chapterInfo.ChildNodes.Count; i++)
        {
            XmlNode group = chapterInfo.ChildNodes[i];
            for (int j = 0; j < group.ChildNodes.Count; j++)
            {
                Level level = GetLevel(group.ChildNodes[j]);
                allLevels.Add(levelID++, level);
            }
        }

        Chapter chapter = new Chapter(chapterID, names, allLevels);
        return chapter;
    }

    private static Level GetLevel(XmlNode levelInfo)
    {
        string Name_zh = levelInfo.Attributes["name_zh"].Value;
        string Name_en = levelInfo.Attributes["name_en"].Value;
        SortedDictionary<string, string> names = new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}};
        int picID = int.Parse(levelInfo.Attributes["picID"].Value);
        LevelType levelType = (LevelType) Enum.Parse(typeof(LevelType), levelInfo.Attributes["levelType"].Value);
        LevelThemeCategory levelThemeType = (LevelThemeCategory) Enum.Parse(typeof(LevelThemeCategory), levelInfo.Attributes["levelThemeCategory"].Value);
        switch (levelType)
        {
            case LevelType.Enemy:
            {
                XmlNode enemyInfo = levelInfo.FirstChild;
                EnemyType enemyType = (EnemyType) Enum.Parse(typeof(EnemyType), enemyInfo.Attributes["enemyType"].Value);
                string buildName = enemyInfo.Attributes["buildName"].Value;
                BuildInfo bi = BuildStoryDatabase.Instance.BuildGroupDict[BuildGroups.EnemyBuilds].GetBuildInfo(buildName);

                List<BonusGroup> AlwaysBonusGroup = new List<BonusGroup>();
                List<BonusGroup> OptionalBonusGroup = new List<BonusGroup>();
                for (int l = 0; l < enemyInfo.ChildNodes.Count; l++)
                {
                    XmlNode bonusGroupInfo = enemyInfo.ChildNodes.Item(l);
                    BonusGroup bg = GetBonusGroup(bonusGroupInfo);

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

    private static BonusGroup GetBonusGroup(XmlNode bonusGroupInfo)
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
        for (int l = 0; l < bonusGroupInfo.ChildNodes.Count; l++)
        {
            XmlNode bonusInfo = bonusGroupInfo.ChildNodes.Item(l);
            Bonus.BonusType bonusType = (Bonus.BonusType) Enum.Parse(typeof(Bonus.BonusType), bonusInfo.Attributes["name"].Value);
            HardFactorValue bonusBaseValue = new HardFactorValue(int.Parse(bonusInfo.Attributes["value"].Value));
            Bonus bonus = new Bonus(bonusType, bonusBaseValue, 100);
            bg.Bonuses.Add(bonus);
        }

        return bg;
    }

    public static void Reset()
    {
        BuildStoryDatabase.Instance.StoryStartDict.Clear();
    }
}
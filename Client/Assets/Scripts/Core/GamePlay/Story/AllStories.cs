using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AllStories
{
    public static string StoriesDirectory => Utils.GetConfigFolderPath() + "/Stories/";

    public static void AddAllStories()
    {
        foreach (string path in Directory.GetFiles(StoriesDirectory))
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
            int playerDefaultBuildId = BuildStoryDatabase.Instance.BuildGroupDict["StoryBuilds"].GetBuildInfo(buildName).BuildID;

            //初始解锁卡片
            XmlNode gpsNode = story.ChildNodes.Item(2);
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
            for (int i = 3; i < story.ChildNodes.Count; i++)
            {
                XmlNode chapterInfo = story.ChildNodes.Item(i);

                XmlNode chapterCommonBonusInfo = chapterInfo.FirstChild;
                List<BonusGroup> chapterCommonBonusGroups_Optional = new List<BonusGroup>();
                List<BonusGroup> chapterCommonBonusGroups_Always = new List<BonusGroup>();
                for (int j = 0; j < chapterCommonBonusInfo.ChildNodes.Count; j++)
                {
                    XmlNode commonBonusGroupInfo = chapterCommonBonusInfo.ChildNodes.Item(j);
                    BonusGroup bg = GetBonusGroup(commonBonusGroupInfo);
                    if (bg.IsAlways)
                    {
                        chapterCommonBonusGroups_Always.Add(bg);
                    }
                    else
                    {
                        chapterCommonBonusGroups_Optional.Add(bg);
                    }
                }

                int chapterID = int.Parse(chapterInfo.Attributes["ChapterID"].Value);
                SortedDictionary<int, Level> allLevels = new SortedDictionary<int, Level>();
                Chapter chapter = new Chapter(chapterID, allLevels);

                XmlNode enemyInfos = chapterInfo.ChildNodes.Item(1);
                for (int k = 0; k < enemyInfos.ChildNodes.Count; k++)
                {
                    XmlNode enemyInfo = enemyInfos.ChildNodes.Item(k);
                    string Name_zh = enemyInfo.Attributes["name_zh"].Value;
                    string Name_en = enemyInfo.Attributes["name_en"].Value;
                    int EnemyPicID = int.Parse(enemyInfo.Attributes["picID"].Value);

                    for (int m = 0; m < enemyInfo.ChildNodes.Count; m++)
                    {
                        XmlNode enemyTypeInfo = enemyInfo.ChildNodes.Item(m);
                        EnemyType enemyType = (EnemyType) Enum.Parse(typeof(EnemyType), enemyTypeInfo.Attributes["type"].Value);
                        string enemyBuildName = enemyTypeInfo.Attributes["BuildName"].Value;
                        BuildInfo enemyBuildInfo = BuildStoryDatabase.Instance.BuildGroupDict["EnemyBuilds"].GetBuildInfo(enemyBuildName).Clone();
                        List<BonusGroup> AlwaysBonusGroup = new List<BonusGroup>();
                        List<BonusGroup> OptionalBonusGroup = new List<BonusGroup>();
                        Enemy Enemy = new Enemy(EnemyPicID, new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}}, enemyBuildInfo, enemyType, 100, AlwaysBonusGroup, OptionalBonusGroup);

                        chapterCommonBonusGroups_Always.ForEach(bonusGroup => { Enemy.AlwaysBonusGroup.Add(bonusGroup.Clone()); });
                        chapterCommonBonusGroups_Optional.ForEach(bonusGroup => { Enemy.OptionalBonusGroup.Add(bonusGroup.Clone()); });
                        chapter.ChapterAllEnemies[enemyType].Add(Enemy.LevelID, Enemy);
                        for (int l = 0; l < enemyTypeInfo.ChildNodes.Count; l++)
                        {
                            XmlNode bonusGroupInfo = enemyTypeInfo.ChildNodes.Item(l);
                            BonusGroup bg = GetBonusGroup(bonusGroupInfo);

                            if (bg.IsAlways)
                            {
                                Enemy.AlwaysBonusGroup.Add(bg);
                            }
                            else
                            {
                                Enemy.OptionalBonusGroup.Add(bg);
                            }
                        }
                    }
                }

                Chapters.Add(chapterID, chapter);
            }

            BuildInfo PlayerCurrentBuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(playerDefaultBuildId).Clone();
            SortedDictionary<int, BuildInfo> playerBuildInfos = new SortedDictionary<int, BuildInfo>();
            playerBuildInfos.Add(PlayerCurrentBuildInfo.BuildID, PlayerCurrentBuildInfo);
            Story newStory = new Story(pureName, Chapters, PlayerCurrentBuildInfo.M_BuildCards.GetBaseCardLimitDict(), playerBuildInfos, gps);
            BuildStoryDatabase.Instance.StoryStartDict.Add(pureName, newStory);
        }
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
}
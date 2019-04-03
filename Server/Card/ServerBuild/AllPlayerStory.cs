using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

internal class AllPlayerStory
{
    public static string StoriesDirectory
    {
        get { return ServerConsole.ServerRoot + "Config/PlayerStories/"; }
    }

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
            int playerDefaultBuildId = Database.Instance.SpecialBuildsDict["StoryAdmin"].GetBuildInfo(buildName).BuildID;

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
                SortedDictionary<int, Enemy> chapterEnemies = new SortedDictionary<int, Enemy>();
                SortedDictionary<int, Shop> chapterShops = new SortedDictionary<int, Shop>();
                Chapter chapter = new Chapter(chapterID, chapterEnemies, chapterShops);

                XmlNode enemyInfos = chapterInfo.ChildNodes.Item(1);
                for (int k = 0; k < enemyInfos.ChildNodes.Count; k++)
                {
                    XmlNode enemyInfo = enemyInfos.ChildNodes.Item(k);
                    string Name = enemyInfo.Attributes["name"].Value;
                    int EnemyPicID = int.Parse(enemyInfo.Attributes["picID"].Value);

                    for (int m = 0; m < enemyInfo.ChildNodes.Count; m++)
                    {
                        XmlNode enemyTypeInfo = enemyInfo.ChildNodes.Item(m);

                    }

                    EnemyType enemyType = EnemyType.None;
                    if (!string.IsNullOrEmpty(BuildName_Soldier)) enemyType |= EnemyType.Soldier;
                    if (!string.IsNullOrEmpty(BuildName_Elite)) enemyType |= EnemyType.Elite;
                    if (!string.IsNullOrEmpty(BuildName_Boss)) enemyType |= EnemyType.Boss;
                    List<BonusGroup> AlwaysBonusGroup = new List<BonusGroup>();
                    List<BonusGroup> OptionalBonusGroup = new List<BonusGroup>();
                    Enemy Enemy = new Enemy(Name, BuildName_Soldier, BuildName_Elite, BuildName_Boss, EnemyPicID, enemyType, 100, AlwaysBonusGroup, OptionalBonusGroup);

                    chapterCommonBonusGroups_Always.ForEach(bonusGroup => { Enemy.AlwaysBonusGroup.Add(bonusGroup.Clone()); });
                    chapterCommonBonusGroups_Optional.ForEach(bonusGroup => { Enemy.OptionalBonusGroup.Add(bonusGroup.Clone()); });

                    chapter.ChapterAllEnemies.Add(Enemy.EnemyPicID, Enemy);

                    for (int l = 0; l < enemyInfo.ChildNodes.Count; l++)
                    {
                        XmlNode bonusGroupInfo = enemyInfo.ChildNodes.Item(l);
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

                Chapters.Add(chapter);
                chapterID++;
            }

            BuildInfo PlayerCurrentBuildInfo = Database.Instance.GetBuildInfoByID(playerDefaultBuildId).Clone();
            SortedDictionary<int, BuildInfo> playerbuildInfos = new SortedDictionary<int, BuildInfo>();
            playerbuildInfos.Add(PlayerCurrentBuildInfo.BuildID, PlayerCurrentBuildInfo);
            Story newStory = new Story(pureName, Chapters, PlayerCurrentBuildInfo.M_BuildCards.GetBaseCardLimitDict(), playerbuildInfos, gps);
            Database.Instance.StoryStartDict.Add(pureName, newStory);
        }
    }

    private static BonusGroup GetBonusGroup(XmlNode bonusGroupInfo)
    {
        BonusGroup bg = new BonusGroup();
        bg.IsAlways = bonusGroupInfo.Attributes["type"].Value == "Always";
        bg.Bonuses = new List<Bonus>();
        for (int l = 0; l < bonusGroupInfo.ChildNodes.Count; l++)
        {
            XmlNode bonusInfo = bonusGroupInfo.ChildNodes.Item(l);
            Bonus bonus = new Bonus();
            bonus.M_BonusType = (Bonus.BonusType) Enum.Parse(typeof(Bonus.BonusType), bonusInfo.Attributes["name"].Value);
            bonus.Value = int.Parse(bonusInfo.Attributes["value"].Value);
            bg.Bonuses.Add(bonus);
        }

        if (bg.IsAlways)
        {
            bg.Probability = 0;
            bg.Singleton = true;
        }
        else
        {
            bg.Probability = int.Parse(bonusGroupInfo.Attributes["probability"].Value);
            bg.Singleton = bonusGroupInfo.Attributes["singleton"].Value == "True";
        }

        return bg;
    }
}
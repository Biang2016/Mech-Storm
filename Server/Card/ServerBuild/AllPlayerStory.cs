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

            List<Level> Levels = new List<Level>();
            int levelID = 0;
            for (int i = 3; i < story.ChildNodes.Count; i++)
            {
                XmlNode levelInfo = story.ChildNodes.Item(i);

                int levelFightTimes = int.Parse(levelInfo.Attributes["LevelFightTimes"].Value); //该Level必须击败几个Boss（必须通过几个LevelID才能到下一个Level）
                int bigBossFightTimes = int.Parse(levelInfo.Attributes["BigBossFightTimes"].Value); //该Level必须击败几个BigBoss
                if (levelFightTimes > levelInfo.ChildNodes.Count - 1)
                {
                    throw new Exception("levelFightTimes < levelInfo.ChildNodes.Count - 1"); //配置错误
                }

                XmlNode levelCommonBonusInfo = levelInfo.FirstChild;
                List<BonusGroup> levelCommonBonusGroups_Optional = new List<BonusGroup>();
                List<BonusGroup> levelCommonBonusGroups_Always = new List<BonusGroup>();
                for (int j = 0; j < levelCommonBonusInfo.ChildNodes.Count; j++)
                {
                    XmlNode commonBonusGroupInfo = levelCommonBonusInfo.ChildNodes.Item(j);
                    BonusGroup bg = GetBonusGroup(commonBonusGroupInfo);
                    if (bg.IsAlways)
                    {
                        levelCommonBonusGroups_Always.Add(bg);
                    }
                    else
                    {
                        levelCommonBonusGroups_Optional.Add(bg);
                    }
                }

                for (int j = 0; j < levelFightTimes; j++)
                {
                    Level level = new Level();
                    level.LevelID = levelID;
                    level.LevelNum = int.Parse(levelInfo.Attributes["Level"].Value);
                    level.BigBossFightTimes = bigBossFightTimes;
                    level.Bosses = new SortedDictionary<int, Boss>();

                    for (int k = 1; k < levelInfo.ChildNodes.Count; k++)
                    {
                        XmlNode bossInfo = levelInfo.ChildNodes.Item(k);
                        Boss Boss = new Boss();
                        Boss.Name = bossInfo.Attributes["name"].Value;
                        Boss.BuildName = bossInfo.Attributes["BuildName"].Value;
                        Boss.PicID = int.Parse(bossInfo.Attributes["picID"].Value);
                        Boss.AlwaysBonusGroup = new List<BonusGroup>();
                        Boss.OptionalBonusGroup = new List<BonusGroup>();

                        levelCommonBonusGroups_Always.ForEach(bonusGroup => { Boss.AlwaysBonusGroup.Add(bonusGroup.Clone()); });
                        levelCommonBonusGroups_Optional.ForEach(bonusGroup => { Boss.OptionalBonusGroup.Add(bonusGroup.Clone()); });

                        level.Bosses.Add(Boss.PicID, Boss);

                        for (int l = 0; l < bossInfo.ChildNodes.Count; l++)
                        {
                            XmlNode bonusGroupInfo = bossInfo.ChildNodes.Item(l);
                            BonusGroup bg = GetBonusGroup(bonusGroupInfo);

                            if (bg.IsAlways)
                            {
                                Boss.AlwaysBonusGroup.Add(bg);
                            }
                            else
                            {
                                Boss.OptionalBonusGroup.Add(bg);
                            }
                        }
                    }

                    Levels.Add(level);
                    levelID++;
                }
            }

            BuildInfo PlayerCurrentBuildInfo = Database.Instance.GetBuildInfoByID(playerDefaultBuildId).Clone();
            SortedDictionary<int, BuildInfo> playerbuildInfos = new SortedDictionary<int, BuildInfo>();
            playerbuildInfos.Add(PlayerCurrentBuildInfo.BuildID, PlayerCurrentBuildInfo);
            Story newStory = new Story(pureName, Levels, BuildInfo.CloneCardCountDict(PlayerCurrentBuildInfo.CardCountDict), playerbuildInfos, gps);
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
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

internal class AllPlayerStory
{
    public static string StoriesDirectory = "../../Config/PlayerStories/";

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
            string unlock_BuildName = story.ChildNodes.Item(1).Attributes["BuildName"].Value;
            int playerDefaultUnlockedBuildID = Database.Instance.SpecialBuildsDict["StoryAdmin"].GetBuildInfo(unlock_BuildName).BuildID;

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
                Level level = new Level();
                XmlNode levelInfo = story.ChildNodes.Item(i);

                level.LevelID = levelID;

                level.Bosses = new List<Boss>();

                for (int j = 0; j < levelInfo.ChildNodes.Count; j++)
                {
                    XmlNode bossInfo = levelInfo.ChildNodes.Item(j);
                    Boss Boss = new Boss();
                    Boss.Name = bossInfo.Attributes["name"].Value;
                    Boss.BuildName = bossInfo.Attributes["BuildName"].Value;
                    Boss.PicID = int.Parse(bossInfo.Attributes["picID"].Value);
                    Boss.AlwaysBonusGroup = new List<BonusGroup>();
                    Boss.OptionalBonusGroup = new List<BonusGroup>();
                    level.Bosses.Add(Boss);

                    for (int k = 0; k < bossInfo.ChildNodes.Count; k++)
                    {
                        XmlNode bonusGroupInfo = bossInfo.ChildNodes.Item(k);
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

            BuildInfo PlayerCurrentBuildInfo = Database.Instance.GetBuildInfoByID(playerDefaultBuildId).Clone();
            BuildInfo PlayerCurrentUnlockedBuildInfo = Database.Instance.GetBuildInfoByID(playerDefaultUnlockedBuildID).Clone();
            Story newStory = new Story(pureName, Levels, PlayerCurrentBuildInfo, PlayerCurrentUnlockedBuildInfo, gps, 0, new SortedDictionary<int, int>());
            Database.Instance.StoryStartDict.Add(pureName, newStory);
        }
    }
}
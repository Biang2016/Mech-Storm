using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            List<Boss> Bosses = new List<Boss>();
            for (int i = 3; i < story.ChildNodes.Count; i++)
            {
                XmlNode bossInfo = story.ChildNodes.Item(i);
                Boss Boss = new Boss();
                Boss.Name = bossInfo.Attributes["name"].Value;
                Boss.BuildName = bossInfo.Attributes["BuildName"].Value;
                Bosses.Add(Boss);
            }

            Story newStory = new Story(Bosses.ToArray(), playerDefaultBuildId, playerDefaultUnlockedBuildID, gps);
            Database.Instance.StoryStartDict.Add(pureName, newStory);
        }
    }
}

public struct Boss
{
    public string Name;
    public string BuildName;
}

public class Story
{
    public List<Boss> Bosses = new List<Boss>();
    public List<Boss> BeatBosses = new List<Boss>();
    public List<Boss> RemainingBosses = new List<Boss>();

    public BuildInfo PlayerCurrentBuildInfo;
    public BuildInfo PlayerCurrentUnlockedBuildInfo;

    public List<BuildInfo> PlayerBuildInfos = new List<BuildInfo>();

    public GamePlaySettings StoryGamePlaySettings = new GamePlaySettings();

    public Story(Boss[] bossArr, int playerCurrentBuildID, int playerDefaultUnlockedBuildID, GamePlaySettings gamePlaySettings)
    {
        Bosses = bossArr.ToList();
        RemainingBosses = bossArr.ToList();

        PlayerCurrentBuildInfo = Database.Instance.GetBuildInfoByID(playerCurrentBuildID).Clone();
        PlayerCurrentBuildInfo.BuildID = Database.Instance.GenerateBuildID();
        Database.Instance.BuildInfoDict.Add(PlayerCurrentBuildInfo.BuildID, PlayerCurrentBuildInfo);

        PlayerCurrentUnlockedBuildInfo = Database.Instance.GetBuildInfoByID(playerDefaultUnlockedBuildID).Clone();
        PlayerCurrentUnlockedBuildInfo.BuildID = Database.Instance.GenerateBuildID();
        Database.Instance.BuildInfoDict.Add(PlayerCurrentUnlockedBuildInfo.BuildID, PlayerCurrentUnlockedBuildInfo);

        StoryGamePlaySettings = gamePlaySettings.Clone();
    }

    public Story Clone()
    {
        return new Story(Bosses.ToArray(), PlayerCurrentBuildInfo.BuildID, PlayerCurrentUnlockedBuildInfo.BuildID, StoryGamePlaySettings.Clone());
    }

    public void BeatBoss(Boss boss)
    {
        Bosses.Remove(boss);
        BeatBosses.Add(boss);
        RemainingBosses.Remove(boss);
    }

    public void UnlockCard(int cardID)
    {
        if (!PlayerCurrentUnlockedBuildInfo.CardIDs.Contains(cardID))
        {
            PlayerCurrentUnlockedBuildInfo.CardIDs.Add(cardID);
        }
    }
}
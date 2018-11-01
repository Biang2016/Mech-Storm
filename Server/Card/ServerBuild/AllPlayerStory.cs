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

            int PlayerDefaultCoin = int.Parse(gpsNode.Attributes["PlayerDefaultCoin"].Value);
            int PlayerDefaultLife = int.Parse(gpsNode.Attributes["PlayerDefaultLife"].Value);
            int PlayerDefaultLifeMax = int.Parse(gpsNode.Attributes["PlayerDefaultLifeMax"].Value);
            int PlayerDefaultLifeMin = int.Parse(gpsNode.Attributes["PlayerDefaultLifeMin"].Value);
            int PlayerDefaultEnergy = int.Parse(gpsNode.Attributes["PlayerDefaultEnergy"].Value);
            int PlayerDefaultEnergyMax = int.Parse(gpsNode.Attributes["PlayerDefaultEnergyMax"].Value);

            GamePlaySettings gps = new GamePlaySettings();
            gps.DrawCardPerRound = DrawCardPerRound;

            gps.DefaultCoin = PlayerDefaultCoin;
            gps.DefaultLife = PlayerDefaultLife;
            gps.DefaultLifeMax = PlayerDefaultLifeMax;
            gps.DefaultLifeMin = PlayerDefaultLifeMin;
            gps.DefaultEnergy = PlayerDefaultEnergy;
            gps.DefaultEnergyMax = PlayerDefaultEnergyMax;

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
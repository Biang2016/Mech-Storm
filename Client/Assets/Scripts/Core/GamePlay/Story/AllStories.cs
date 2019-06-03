using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AllStories
{
    public static string StoriesDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Stories/";

    public static Dictionary<string, Story> StoryDict = new Dictionary<string, Story>();

    private static bool NeedReload = false;

    public static void AddAllStories()
    {
        Reset();
        foreach (string path in Directory.GetFiles(StoriesDirectory, "*.xml", SearchOption.TopDirectoryOnly))
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

            BuildInfo buildInfo = BuildInfo.GetBuildInfoFromXML(story.ChildNodes.Item(0), out bool needRefresh);
            NeedReload |= needRefresh;
            GamePlaySettings gps = GamePlaySettings.GetGamePlaySettingsFromXML(story.ChildNodes.Item(1));

            SortedDictionary<int, Chapter> Chapters = new SortedDictionary<int, Chapter>();
            XmlNode node_Chapters = story.ChildNodes.Item(2);
            for (int i = 0; i < node_Chapters.ChildNodes.Count; i++)
            {
                Chapter chapter = GetChapterFromXML(node_Chapters.ChildNodes[i]);
                Chapters.Add(chapter.ChapterID, chapter);
            }

            SortedDictionary<int, BuildInfo> playerBuildInfos = new SortedDictionary<int, BuildInfo>();
            playerBuildInfos.Add(buildInfo.BuildID, buildInfo);
            Story newStory = new Story(pureName, Chapters, playerBuildInfos, gps);
            StoryDict.Add(newStory.StoryName, newStory);
        }

        //If any problem, refresh XML and reload
        if (NeedReload)
        {
            NeedReload = false;
            RefreshAllStoryXML();
            ReloadStoryXML();
        }
    }

    public static Story GetStory(string storyName, CloneVariantUtils.OperationType operationType)
    {
        if (StoryDict.ContainsKey(storyName))
        {
            if (operationType == CloneVariantUtils.OperationType.Clone)
            {
                return StoryDict[storyName].Clone();
            }
            else
            {
                return StoryDict[storyName].Variant();
            }
        }

        return null;
    }

    public static void ReloadStoryXML()
    {
        AddAllStories();
    }

    public static void RefreshAllStoryXML()
    {
        List<Story> allStories = new List<Story>();
        foreach (KeyValuePair<string, Story> kv in StoryDict)
        {
            allStories.Add(kv.Value);
        }

        foreach (Story s in allStories)
        {
            RefreshStoryXML(s);
        }
    }

    public static void RefreshStoryXML(Story story)
    {
        FileInfo fi = new FileInfo(StoriesDirectory + story.StoryName + ".xml");

        if (StoryDict.ContainsKey(story.StoryName))
        {
            StoryDict[story.StoryName] = story;
        }

        XmlDocument doc = new XmlDocument();
        story.ExportToXML(doc);
        XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmldecl, root);

        using (StreamWriter sw = new StreamWriter(fi.FullName))
        {
            doc.Save(sw);
        }
    }

    private static Chapter GetChapterFromXML(XmlNode node_ChapterInfo)
    {
        int chapterID = int.Parse(node_ChapterInfo.Attributes["chapterID"].Value);
        string Name_zh = node_ChapterInfo.Attributes["name_zh"].Value;
        string Name_en = node_ChapterInfo.Attributes["name_en"].Value;
        SortedDictionary<string, string> names = new SortedDictionary<string, string> {{"zh", Name_zh}, {"en", Name_en}};

        SortedDictionary<int, Level> allLevels = new SortedDictionary<int, Level>();

        int levelID = 0;
        XmlNode node_EnemyInfos = node_ChapterInfo.ChildNodes[0];
        XmlNode node_ShopInfos = node_ChapterInfo.ChildNodes[1];

        for (int i = 0; i < node_EnemyInfos.ChildNodes.Count; i++)
        {
            XmlNode enemyInfo = node_EnemyInfos.ChildNodes[i];
            Enemy enemy = (Enemy) AllLevels.GetLevel(LevelType.Enemy, enemyInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            allLevels.Add(levelID++, enemy);
        }

        for (int i = 0; i < node_ShopInfos.ChildNodes.Count; i++)
        {
            XmlNode shopInfo = node_ShopInfos.ChildNodes[i];
            Shop shop = (Shop) AllLevels.GetLevel(LevelType.Enemy, shopInfo.Attributes["name"].Value, CloneVariantUtils.OperationType.Clone);
            allLevels.Add(levelID++, shop);
        }

        int chapterMapRoundCount = int.Parse(node_ChapterInfo.Attributes["chapterMapRoundCount"].Value);

        Chapter chapter = new Chapter(chapterID, names, allLevels, chapterMapRoundCount);
        return chapter;
    }

    public static void Reset()
    {
        StoryDict.Clear();
    }
}
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class Level : IClone<Level>, IVariant<Level>
{
    public LevelTypes LevelType;
    public int LevelPicID;
    public SortedDictionary<string, string> LevelNames;
    public int DifficultyLevel;

    public int LevelID;

    public Level(LevelTypes levelType, int levelPicId, SortedDictionary<string, string> levelNames, int difficultyLevel)
    {
        LevelType = levelType;
        LevelPicID = levelPicId;
        LevelNames = levelNames;
        DifficultyLevel = difficultyLevel;
    }

    public Story.InfoRefreshDelegate InfoRefresh; // 信息更新委托

    public abstract Level Clone();
    public abstract Level Variant();
    public abstract bool DeleteCard(int cardID);

    public void ExportToXML(XmlElement allLevel_ele)
    {
        XmlDocument doc = allLevel_ele.OwnerDocument;
        XmlElement old_node = null;
        foreach (XmlElement level_node in allLevel_ele.ChildNodes)
        {
            if (level_node.Attributes["name_en"].Value.Equals(LevelNames["en"]))
            {
                old_node = level_node;
            }
        }

        if (old_node != null)
        {
            allLevel_ele.RemoveChild(old_node);
        }

        XmlElement level_ele = doc.CreateElement("LevelInfo");
        allLevel_ele.AppendChild(level_ele);
        level_ele.SetAttribute("name_en", LevelNames["en"]);
        level_ele.SetAttribute("name_zh", LevelNames["zh"]);
        level_ele.SetAttribute("picID", LevelPicID.ToString());
        level_ele.SetAttribute("levelType", LevelType.ToString());
        level_ele.SetAttribute("difficultyLevel", DifficultyLevel.ToString());

        ChildrenExportToXML(level_ele);
    }

    private static int NewLevelID = 0;

    private static int GenerateNewLevelID()
    {
        return NewLevelID++;
    }

    public static Level BaseGenerateEmptyLevel(LevelTypes levelType)
    {
        Level newLevel = null;
        int levelIDPostFix = GenerateNewLevelID();
        while (AllLevels.LevelDict[levelType].ContainsKey("New" + levelType + "_" + levelIDPostFix))
        {
            levelIDPostFix = GenerateNewLevelID();
        }

        newLevel = AllLevels.LevelDict[levelType]["New" + levelType]?.Clone();

        if (newLevel != null)
        {
            newLevel.LevelNames["zh"] = "新关卡_" + levelIDPostFix;
            newLevel.LevelNames["en"] = "New" + levelType + "_" + levelIDPostFix;
            AllLevels.RefreshLevelXML(newLevel);
            AllLevels.ReloadLevelXML();
        }

        return newLevel;
    }

    protected abstract void ChildrenExportToXML(XmlElement level_ele);

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) LevelType);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(LevelPicID);
        writer.WriteSInt32(LevelNames.Count);
        foreach (KeyValuePair<string, string> kv in LevelNames)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }

        writer.WriteSInt32(DifficultyLevel);
    }

    public static Level BaseDeserialize(DataStream reader)
    {
        LevelTypes levelType = (LevelTypes) reader.ReadSInt32();
        int levelID = reader.ReadSInt32();
        int levelPicID = reader.ReadSInt32();
        int levelNameCount = reader.ReadSInt32();
        SortedDictionary<string, string> LevelNames = new SortedDictionary<string, string>();
        for (int i = 0; i < levelNameCount; i++)
        {
            string ls = reader.ReadString8();
            string value = reader.ReadString8();
            LevelNames[ls] = value;
        }

        int difficultyLevel = reader.ReadSInt32();

        Level res = null;
        switch (levelType)
        {
            case LevelTypes.Enemy:
            {
                BuildInfo BuildInfo = BuildInfo.Deserialize(reader);
                EnemyType EnemyType = (EnemyType) (reader.ReadSInt32());
                int bonusCount = reader.ReadSInt32();
                List<BonusGroup> BonusGroups = new List<BonusGroup>();
                for (int i = 0; i < bonusCount; i++)
                {
                    BonusGroups.Add(BonusGroup.Deserialize(reader));
                }

                CardPriority cp = CardPriority.Deserialize(reader);

                int cardComboCount = reader.ReadSInt32();
                List<CardCombo> cardComboList = new List<CardCombo>();
                for (int i = 0; i < cardComboCount; i++)
                {
                    cardComboList.Add(CardCombo.Deserialize(reader));
                }

                res = new Enemy(levelPicID, LevelNames, difficultyLevel, BuildInfo, EnemyType, BonusGroups, cardComboList, cp);
                break;
            }

            case LevelTypes.Shop:
            {
                int count = reader.ReadSInt32();
                List<ShopItem> shopItems = new List<ShopItem>();
                for (int i = 0; i < count; i++)
                {
                    ShopItem si = ShopItem.Deserialize(reader);
                    shopItems.Add(si);
                }

                int shopItemCardCount = reader.ReadSInt32();
                int shopItemOthersCount = reader.ReadSInt32();

                res = new Shop(levelPicID, LevelNames, difficultyLevel, shopItems, shopItemCardCount, shopItemOthersCount);
                break;
            }
        }

        res.LevelID = levelID;
        return res;
    }

    public static string GetLevelTypeDesc(LevelTypes levelType)
    {
        return LanguageManager_Common.GetText("LevelType_" + levelType);
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum LevelTypes
{
    Enemy = 0,
    Shop = 1,
    Rest = 2,
    Start = 3,
    Treasure = 4,
}
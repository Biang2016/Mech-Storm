using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class Level : IClone<Level>, IVariant<Level>
{
    public LevelType LevelType;
    public LevelThemeCategory LevelThemeCategory;
    public int LevelPicID;
    public SortedDictionary<string, string> LevelNames;

    public int LevelID;

    public Level(LevelType levelType, LevelThemeCategory levelThemeCategory, int levelPicId, SortedDictionary<string, string> levelNames)
    {
        LevelThemeCategory = levelThemeCategory;
        LevelType = levelType;
        LevelPicID = levelPicId;
        LevelNames = levelNames;
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
        level_ele.SetAttribute("levelThemeCategory", LevelThemeCategory.ToString());
        level_ele.SetAttribute("levelType", LevelType.ToString());

        ChildrenExportToXML(level_ele);
    }

    protected abstract void ChildrenExportToXML(XmlElement level_ele);

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) LevelType);
        writer.WriteSInt32((int) LevelThemeCategory);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(LevelPicID);
        writer.WriteSInt32(LevelNames.Count);
        foreach (KeyValuePair<string, string> kv in LevelNames)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }
    }

    public static Level BaseDeserialize(DataStream reader)
    {
        LevelType levelType = (LevelType) reader.ReadSInt32();
        LevelThemeCategory levelThemeCategory = (LevelThemeCategory) reader.ReadSInt32();
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

        Level res = null;
        switch (levelType)
        {
            case LevelType.Enemy:
                BuildInfo BuildInfo = BuildInfo.Deserialize(reader);
                EnemyType EnemyType = (EnemyType) (reader.ReadSInt32());
                int hardFactor = reader.ReadSInt32();

                int alwaysBonusCount = reader.ReadSInt32();
                List<BonusGroup> AlwaysBonusGroup = new List<BonusGroup>();
                for (int i = 0; i < alwaysBonusCount; i++)
                {
                    AlwaysBonusGroup.Add(BonusGroup.Deserialize(reader));
                }

                int optionalBonusCount = reader.ReadSInt32();
                List<BonusGroup> OptionalBonusGroup = new List<BonusGroup>();
                for (int i = 0; i < optionalBonusCount; i++)
                {
                    OptionalBonusGroup.Add(BonusGroup.Deserialize(reader));
                }

                return new Enemy(levelThemeCategory, levelPicID, LevelNames, BuildInfo, EnemyType, hardFactor, AlwaysBonusGroup, OptionalBonusGroup);

                break;
            case LevelType.Shop:
                int count = reader.ReadSInt32();
                List<ShopItem> shopItems = new List<ShopItem>();
                for (int i = 0; i < count; i++)
                {
                    ShopItem si = ShopItem.Deserialize(reader);
                    shopItems.Add(si);
                }

                res = new Shop(levelThemeCategory, levelPicID, LevelNames, shopItems);
                break;
        }

        res.LevelID = levelID;
        return res;
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum LevelType
{
    Enemy = 0,
    Shop = 1,
    Rest = 2,
    Start = 3,
    Treasure = 4,
}

/// <summary>
/// 关卡主题分类
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum LevelThemeCategory
{
    Soldiers = 0,
    Spells = 1,
    Equips = 2,
    Energy = 3,
    LifeMetalEnergyBudget = 4,
    Heros = 5
}
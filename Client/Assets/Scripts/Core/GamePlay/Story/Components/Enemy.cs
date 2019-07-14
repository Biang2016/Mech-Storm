using System.Collections.Generic;
using System.Xml;

public class Enemy : Level
{
    public BuildInfo BuildInfo;
    public EnemyType EnemyType;
    public int Level;
    public List<BonusGroup> BonusGroups;

    public Enemy(LevelThemeCategory levelThemeCategory, int levelPicID, SortedDictionary<string, string> levelNames, BuildInfo buildInfo, EnemyType enemyType, int level, List<BonusGroup> bonusGroups)
        : base(LevelTypes.Enemy, levelThemeCategory, levelPicID, levelNames)
    {
        BuildInfo = buildInfo;
        EnemyType = enemyType;
        Level = level;
        BonusGroups = bonusGroups;
    }

    public override Level Clone()
    {
        return new Enemy(LevelThemeCategory, LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), BuildInfo.Clone(), EnemyType, Level, CloneVariantUtils.List(BonusGroups));
    }

    public override Level Variant()
    {
        //TODO
        return Clone();
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public override bool DeleteCard(int cardID)
    {
        if (BuildInfo.M_BuildCards.CardSelectInfos.ContainsKey(cardID))
        {
            BuildInfo.M_BuildCards.CardSelectInfos.Remove(cardID);
            return true;
        }

        return false;
    }

    protected override void ChildrenExportToXML(XmlElement level_ele)
    {
        XmlDocument doc = level_ele.OwnerDocument;
        XmlElement enemy_ele = doc.CreateElement("EnemyInfo");
        level_ele.AppendChild(enemy_ele);

        enemy_ele.SetAttribute("enemyType", EnemyType.ToString());
        enemy_ele.SetAttribute("level", Level.ToString());
        BuildInfo.ExportToXML(enemy_ele);

        XmlElement bonusGroupInfos_ele = doc.CreateElement("BonusGroupInfos");
        enemy_ele.AppendChild(bonusGroupInfos_ele);

        foreach (BonusGroup bg in BonusGroups)
        {
            bg.ExportToXML(bonusGroupInfos_ele);
        }
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
        writer.WriteSInt32((int) EnemyType);
        writer.WriteSInt32(Level);

        writer.WriteSInt32(BonusGroups.Count);
        foreach (BonusGroup bonus in BonusGroups)
        {
            bonus.Serialize(writer);
        }
    }

    public static string GetEnemyTypeDesc(EnemyType enemyType)
    {
        return LanguageManager_Common.GetText("EnemyType_" + enemyType);
    }
}
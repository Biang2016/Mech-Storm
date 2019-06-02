using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;

public class Enemy : Level
{
    public BuildInfo BuildInfo;
    public EnemyType EnemyType;
    private int hardFactor;
    public List<BonusGroup> AlwaysBonusGroup;
    public List<BonusGroup> OptionalBonusGroup;

    public Enemy(LevelThemeCategory levelThemeCategory, int levelPicID, SortedDictionary<string, string> levelNames, BuildInfo buildInfo, EnemyType enemyType, int hardFactor, List<BonusGroup> alwaysBonusGroup, List<BonusGroup> optionalBonusGroup)
        : base(LevelType.Enemy, levelThemeCategory, levelPicID, levelNames)
    {
        BuildInfo = buildInfo;
        EnemyType = enemyType;
        this.hardFactor = hardFactor;
        AlwaysBonusGroup = alwaysBonusGroup;
        OptionalBonusGroup = optionalBonusGroup;
    }

    public override Level Clone()
    {
        return new Enemy(LevelThemeCategory, LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), BuildInfo, EnemyType, hardFactor, CloneVariantUtils.List(AlwaysBonusGroup), CloneVariantUtils.List(OptionalBonusGroup));
    }

    public override Level Variant()
    {
        //TODO
        return null;
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
        BuildInfo.ExportToXML(enemy_ele);

        XmlElement bonusGroupInfos_ele = doc.CreateElement("BonusGroupInfos");
        enemy_ele.AppendChild(bonusGroupInfos_ele);

        foreach (BonusGroup bg in AlwaysBonusGroup)
        {
            bg.ExportToXML(bonusGroupInfos_ele);
        }

        foreach (BonusGroup bg in OptionalBonusGroup)
        {
            bg.ExportToXML(bonusGroupInfos_ele);
        }
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
        writer.WriteSInt32((int) EnemyType);
        writer.WriteSInt32(hardFactor);

        writer.WriteSInt32(AlwaysBonusGroup.Count);
        foreach (BonusGroup bonus in AlwaysBonusGroup)
        {
            bonus.Serialize(writer);
        }

        writer.WriteSInt32(OptionalBonusGroup.Count);
        foreach (BonusGroup bonus in OptionalBonusGroup)
        {
            bonus.Serialize(writer);
        }
    }

    public static Enemy Deserialize(DataStream reader) // 除Level类外 不可直接调用
    {
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

        return new Enemy(LevelThemeCategory.Energy, 0, null, BuildInfo, EnemyType, hardFactor, AlwaysBonusGroup, OptionalBonusGroup);
    }
}
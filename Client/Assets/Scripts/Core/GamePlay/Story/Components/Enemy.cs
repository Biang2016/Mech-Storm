using System.Collections.Generic;
using System.Xml;

public class Enemy : Level
{
    public BuildInfo BuildInfo;
    public EnemyType EnemyType;
    public List<BonusGroup> BonusGroups;
    public List<CardCombo> CardComboList; // sorted by combo priority
    public CardPriority CardPriority;

    public Enemy(int levelPicID, SortedDictionary<string, string> levelNames, int difficultyLevel, BuildInfo buildInfo, EnemyType enemyType, List<BonusGroup> bonusGroups, List<CardCombo> cardComboList, CardPriority cardPriority)
        : base(LevelTypes.Enemy, levelPicID, levelNames, difficultyLevel)
    {
        BuildInfo = buildInfo;
        EnemyType = enemyType;
        BonusGroups = bonusGroups;
        CardComboList = cardComboList;
        CardPriority = cardPriority;
    }

    public override Level Clone()
    {
        Enemy enemy = new Enemy(LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), DifficultyLevel, BuildInfo.Clone(), EnemyType, CloneVariantUtils.List(BonusGroups), CloneVariantUtils.List(CardComboList), CardPriority);
        enemy.LevelID = LevelID;
        return enemy;
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
        BuildInfo.ExportToXML(enemy_ele);

        XmlElement bonusGroupInfos_ele = doc.CreateElement("BonusGroupInfos");
        enemy_ele.AppendChild(bonusGroupInfos_ele);

        foreach (BonusGroup bg in BonusGroups)
        {
            bg.ExportToXML(bonusGroupInfos_ele);
        }

        CardPriority.ExportToXML(enemy_ele);

        XmlElement cardComboList_ele = doc.CreateElement("CardComboList");
        enemy_ele.AppendChild(cardComboList_ele);

        foreach (CardCombo cc in CardComboList)
        {
            cc.ExportToXML(cardComboList_ele);
        }
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
        writer.WriteSInt32((int) EnemyType);

        writer.WriteSInt32(BonusGroups.Count);
        foreach (BonusGroup bonus in BonusGroups)
        {
            bonus.Serialize(writer);
        }

        CardPriority.Serialize(writer);

        writer.WriteSInt32(CardComboList.Count);
        foreach (CardCombo cc in CardComboList)
        {
            cc.Serialize(writer);
        }
    }

    public static string GetEnemyTypeDesc(EnemyType enemyType)
    {
        return LanguageManager_Common.GetText("EnemyType_" + enemyType);
    }
}
using System;
using System.Collections.Generic;

public class Enemy : ChapterPace
{
    public string Name;
    public string BuildName_Soldier;
    public string BuildName_Elite;
    public string BuildName_Boss;
    public int EnemyPicID;
    public EnemyType EnemyType;
    private int hardFactor;
    public List<BonusGroup> AlwaysBonusGroup;
    public List<BonusGroup> OptionalBonusGroup;

    private Enemy()
    {
    }

    public Enemy(string name, string buildName_Soldier,string buildName_Elite, string buildName_Boss, int enemyPicId, EnemyType enemyType, int hardFactor, List<BonusGroup> alwaysBonusGroup, List<BonusGroup> optionalBonusGroup)
    {
        Name = name;
        BuildName_Soldier = buildName_Soldier;
        BuildName_Elite = buildName_Elite;
        BuildName_Boss = buildName_Boss;
        EnemyPicID = enemyPicId;
        EnemyType = enemyType;
        this.hardFactor = hardFactor;
        AlwaysBonusGroup = alwaysBonusGroup;
        OptionalBonusGroup = optionalBonusGroup;
    }

    public override ChapterPace LeftChapterPace { get; }
    public override ChapterPace RightChapterPace { get; }
    public override ChapterPace UpperChapterPace { get; }
    public override ChapterPace LowerChapterPace { get; }

    public override ChapterPace Clone()
    {
        return new Enemy(Name, BuildName_Soldier, BuildName_Elite, BuildName_Boss, EnemyPicID, EnemyType, hardFactor, CloneVariantUtils.List(AlwaysBonusGroup), CloneVariantUtils.List(OptionalBonusGroup));
    }

    public override ChapterPace Variant()
    {
        //TODO
        return new Enemy();
    }

    public override void Serialize(DataStream writer)
    {
        writer.WriteString8(Name);
        writer.WriteString8(BuildName_Soldier);
        writer.WriteString8(BuildName_Elite);
        writer.WriteString8(BuildName_Boss);
        writer.WriteSInt32(EnemyPicID);
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

    public static Enemy Deserialize(DataStream reader)
    {
        Enemy newEnemy = new Enemy();
        newEnemy.Name = reader.ReadString8();
        newEnemy.BuildName_Soldier = reader.ReadString8();
        newEnemy.BuildName_Elite = reader.ReadString8();
        newEnemy.BuildName_Boss = reader.ReadString8();
        newEnemy.EnemyPicID = reader.ReadSInt32();
        newEnemy.EnemyType = (EnemyType) (reader.ReadSInt32());
        newEnemy.hardFactor = reader.ReadSInt32();

        int alwaysBonusCount = reader.ReadSInt32();
        newEnemy.AlwaysBonusGroup = new List<BonusGroup>();
        for (int i = 0; i < alwaysBonusCount; i++)
        {
            newEnemy.AlwaysBonusGroup.Add(BonusGroup.Deserialize(reader));
        }

        int optionalBonusCount = reader.ReadSInt32();
        newEnemy.OptionalBonusGroup = new List<BonusGroup>();
        for (int i = 0; i < optionalBonusCount; i++)
        {
            newEnemy.OptionalBonusGroup.Add(BonusGroup.Deserialize(reader));
        }

        return newEnemy;
    }

}


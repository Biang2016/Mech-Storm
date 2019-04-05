using System;
using System.Collections.Generic;

public class Enemy : StoryPace
{
    public override Story M_Story { get; set; }
    public BuildInfo BuildInfo;
    public int EnemyPicID;
    public EnemyType EnemyType;
    private int hardFactor;
    public List<BonusGroup> AlwaysBonusGroup;
    public List<BonusGroup> OptionalBonusGroup;

    private Enemy()
    {
    }

    public Enemy(string name, BuildInfo buildInfo, int enemyPicId, EnemyType enemyType, int hardFactor, List<BonusGroup> alwaysBonusGroup, List<BonusGroup> optionalBonusGroup)
    {
        Name = name;
        BuildInfo = buildInfo;
        EnemyPicID = enemyPicId;
        EnemyType = enemyType;
        this.hardFactor = hardFactor;
        AlwaysBonusGroup = alwaysBonusGroup;
        OptionalBonusGroup = optionalBonusGroup;
    }

    public override StoryPace Clone()
    {
        return new Enemy(Name, BuildInfo, EnemyPicID, EnemyType, hardFactor, CloneVariantUtils.List(AlwaysBonusGroup), CloneVariantUtils.List(OptionalBonusGroup));
    }

    public override StoryPace Variant()
    {
        //TODO
        return new Enemy();
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
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
        newEnemy.BuildInfo = BuildInfo.Deserialize(reader);
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
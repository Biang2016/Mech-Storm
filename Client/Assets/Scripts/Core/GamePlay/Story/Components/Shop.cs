using System.Collections.Generic;

public class Shop : Level
{
    public List<int> ShopCardIDs = new List<int>();

    public Shop(LevelThemeCategory levelThemeCategory,int levelPicId,  SortedDictionary<string, string> levelNames) : base(LevelType.Shop, levelThemeCategory, levelPicId, levelNames)
    {
    }

    public override Level Clone()
    {
        //TODO
        return this;
    }

    public override Level Variant()
    {
        //TODO
        return null;
    }

    public override void Serialize(DataStream writer)
    {
        //TODO
    }

    public static Shop Deserialize(DataStream reader)
    {
        //TODO
        return null;
    }
}
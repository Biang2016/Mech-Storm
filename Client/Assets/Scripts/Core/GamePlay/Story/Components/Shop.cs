using System.Collections.Generic;

public class Shop : Level
{
    public Shop(int levelPicId, SortedDictionary<string, string> levelNames) : base(LevelType.Shop, levelPicId, levelNames)
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
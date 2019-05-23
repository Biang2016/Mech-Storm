using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class Level : IClone<Level>, IVariant<Level>
{
    public LevelType LevelType;
    public int LevelID;
    public int LevelPicID;
    public SortedDictionary<string, string> LevelNames;

    protected Level(LevelType levelType, int levelPicId, SortedDictionary<string, string> levelNames)
    {
        LevelType = levelType;
        LevelPicID = levelPicId;
        LevelNames = levelNames;
    }

    public Story M_Story;
    public abstract Level Clone();
    public abstract Level Variant();

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
    }

    public static Level BaseDeserialize(DataStream reader)
    {
        LevelType type = (LevelType) reader.ReadSInt32();
        int levelID = reader.ReadSInt32();
        int levelPicID = reader.ReadSInt32();
        Level res = null;
        switch (type)
        {
            case LevelType.Enemy:
                res = Enemy.Deserialize(reader);
                break;
            case LevelType.Shop:
                res = Shop.Deserialize(reader);
                break;
        }

        if (res != null)
        {
            res.LevelID = levelID;
            res.LevelPicID = levelPicID;
            int levelNameCount = reader.ReadSInt32();
            SortedDictionary<string, string> LevelNames = new SortedDictionary<string, string>();
            for (int i = 0; i < levelNameCount; i++)
            {
                string ls = reader.ReadString8();
                string value = reader.ReadString8();
                LevelNames[ls] = value;
            }

            res.LevelNames = LevelNames;
            return res;
        }
        else
        {
            return null;
        }
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum LevelType
{
    Enemy,
    Shop
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class StoryPace : IClone<StoryPace>, IVariant<StoryPace>
{
    public int StoryPaceID;
    public string Name;
    public StoryPaceType StoryPaceType;
    public abstract Story M_Story { get; set; }
    public abstract StoryPace Clone();
    public abstract StoryPace Variant();

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) StoryPaceType);
        writer.WriteSInt32(StoryPaceID);
        writer.WriteString8(Name);
    }

    public static StoryPace BaseDeserialize(DataStream reader)
    {
        int storyPaceID = reader.ReadSInt32();
        string name = reader.ReadString8();

        StoryPaceType type = (StoryPaceType) reader.ReadSInt32();
        switch (type)
        {
            case StoryPaceType.Enemy:
                Enemy enemy = Enemy.Deserialize(reader);
                enemy.StoryPaceID = storyPaceID;
                enemy.Name = name;
                return enemy;
            case StoryPaceType.Shop:
                Shop shop = Shop.Deserialize(reader);
                shop.StoryPaceID = storyPaceID;
                shop.Name = name;
                return shop;
            default:
                break;
        }

        return null;
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StoryPaceType
{
    Enemy,
    Shop
}
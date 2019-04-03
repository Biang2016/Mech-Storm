using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.CSharp;

public abstract class ChapterPace : IClone<ChapterPace>, IVariant<ChapterPace>
{
    public abstract ChapterPace LeftChapterPace { get; }
    public abstract ChapterPace RightChapterPace { get; }
    public abstract ChapterPace UpperChapterPace { get; }
    public abstract ChapterPace LowerChapterPace { get; }

    public abstract ChapterPace Clone();
    public abstract ChapterPace Variant();

    public abstract void Serialize(DataStream writer);

    public static ChapterPace BaseDeserialize(DataStream reader)
    {
        ChapterPaceType type = (ChapterPaceType) reader.ReadSInt32();
        switch (type)
        {
            case ChapterPaceType.Enemy:
                Enemy enemy = Enemy.Deserialize(reader);
                return enemy;
            case ChapterPaceType.Shop:
                Shop shop = Shop.Deserialize(reader);
                return shop;
            default:
                break;
        }

        return null;
    }

    protected enum ChapterPaceType
    {
        Enemy,
        Shop
    }
}
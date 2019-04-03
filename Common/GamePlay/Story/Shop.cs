
public class Shop : ChapterPace
{
    public int ShopID;

    private Shop()
    {
    }

    public override ChapterPace Clone()
    {
        return this;
    }

    public override ChapterPace Variant()
    {
        Shop newShop = new Shop();
        //TODO
        return newShop;
    }

    public override ChapterPace LeftChapterPace { get; }
    public override ChapterPace RightChapterPace { get; }
    public override ChapterPace UpperChapterPace { get; }
    public override ChapterPace LowerChapterPace { get; }

    public override void Serialize(DataStream writer)
    {
        //TODO
    }

    public static Shop Deserialize(DataStream reader)
    {
        Shop newShop = new Shop();

        //TODO
        return newShop;
    }
}


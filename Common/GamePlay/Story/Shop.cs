
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



public class Shop : StoryPace
{
    private Shop()
    {
    }

    public override Story M_Story { get; set; }

    public override StoryPace Clone()
    {
        //TODO
        return this;
    }

    public override StoryPace Variant()
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


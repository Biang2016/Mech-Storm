
public  class ShopItem : IClone<ShopItem>
{
    public enum ShopItemTypes
    {
        Card = 0,
        LifeUpperLimit = 1,
        LifeHeal = 2,
        EnergyUpperLimit = 3,
        Budget = 4,
    }

    public ShopItemTypes ShopItemType;

    public int Price;

    public ShopItem Clone()
    {
        ShopItem shopItem = new ShopItem_Card();
        
        return new ShopItem(ShopItemType, Price);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) ShopItemType);
        writer.WriteSInt32(Price);
    }

    public static ShopItem BaseDeserialize(DataStream reader)
    {
        ShopItemTypes shopItemType = (ShopItemTypes) reader.ReadSInt32();
        int price = reader.ReadSInt32();
        ShopItem shopItem = Deserialize(reader);
        shopItem.ShopItemType = shopItemType;
        shopItem.Price = price;
        return shopItem;
    }

    protected static virtual ShopItem Deserialize(DataStream reader)
    {

    }
}
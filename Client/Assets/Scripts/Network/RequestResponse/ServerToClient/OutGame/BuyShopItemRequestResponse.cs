public class BuyShopItemRequestResponse : ServerRequestBase
{
    public ShopItem ShopItem;

    public BuyShopItemRequestResponse()
    {
    }

    public BuyShopItemRequestResponse(ShopItem shopItem)
    {
        ShopItem = shopItem;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BUY_SHOP_ITEM_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        ShopItem.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ShopItem = ShopItem.Deserialize(reader);
    }
}
public class BuyShopItemRequest : ClientRequestBase
{
    public ShopItem ShopItem;

    public BuyShopItemRequest() : base()
    {
    }

    public BuyShopItemRequest(int clientId, ShopItem shopItem) : base(clientId)
    {
        ShopItem = shopItem;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BUY_SHOP_ITEM_REQUEST;
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
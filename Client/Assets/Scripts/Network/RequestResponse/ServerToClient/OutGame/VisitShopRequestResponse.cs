using System.Collections.Generic;

public class VisitShopRequestResponse : ServerRequestBase
{
    public int LevelID;
    public Shop Shop;

    public VisitShopRequestResponse()
    {
    }

    public VisitShopRequestResponse(int levelID, Shop shop)
    {
        LevelID = levelID;
        Shop = shop;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.VISIT_SHOP_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelID);
        Shop.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        LevelID = reader.ReadSInt32();
        Shop = (Shop) Level.BaseDeserialize(reader);
    }
}
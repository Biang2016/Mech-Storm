public class LeaveShopRequest : ClientRequestBase
{
    public int ChapterID;
    public int LevelID;

    public LeaveShopRequest()
    {
    }

    public LeaveShopRequest(int clientId, int chapterID, int levelID) : base(clientId)
    {
        ChapterID = chapterID;
        LevelID = levelID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.LEAVE_SHOP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(LevelID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ChapterID = reader.ReadSInt32();
        LevelID = reader.ReadSInt32();
    }
}
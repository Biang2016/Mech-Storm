public class EndBattleRequest : ClientRequestBase
{
    public int ChapterID;
    public int LevelID;

    public EndBattleRequest()
    {
    }

    public EndBattleRequest(int clientId, int chapterID, int levelID) : base(clientId)
    {
        ChapterID = chapterID;
        LevelID = levelID;
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

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_BATTLE_REQUEST;
    }
}
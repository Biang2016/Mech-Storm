public class BeatLevelRequest : ServerRequestBase
{
    public int LevelID;

    public BeatLevelRequest()
    {
    }

    public BeatLevelRequest(int levelID)
    {
        LevelID = levelID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_LEVEL_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        LevelID = reader.ReadSInt32();
    }
}
public class BeatEnemyRequest : ServerRequestBase
{
    public int LevelID;

    public BeatEnemyRequest()
    {
    }

    public BeatEnemyRequest(int levelID)
    {
        LevelID = levelID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_ENEMY_REQUSET;
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
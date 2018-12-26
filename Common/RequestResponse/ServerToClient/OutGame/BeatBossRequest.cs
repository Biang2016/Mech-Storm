public class BeatBossRequest : ServerRequestBase
{
    public int LevelID;
    public int BossPicID;

    public BeatBossRequest()
    {
    }

    public BeatBossRequest(int levelID, int bossPicID)
    {
        LevelID = levelID;
        BossPicID = bossPicID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_BOSS_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(BossPicID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        LevelID = reader.ReadSInt32();
        BossPicID = reader.ReadSInt32();
    }
}
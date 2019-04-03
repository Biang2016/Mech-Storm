public class BeatBossRequest : ServerRequestBase
{
    public int ChapterID;
    public int BossPicID;

    public BeatBossRequest()
    {
    }

    public BeatBossRequest(int chapterID, int bossPicID)
    {
        ChapterID = chapterID;
        BossPicID = bossPicID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_BOSS_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(BossPicID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ChapterID = reader.ReadSInt32();
        BossPicID = reader.ReadSInt32();
    }
}
public class BeatEnemyRequest : ServerRequestBase
{
    public int ChapterID;
    public int EnemyPicID;

    public BeatEnemyRequest()
    {
    }

    public BeatEnemyRequest(int chapterID, int enemyPicID)
    {
        ChapterID = chapterID;
        EnemyPicID = enemyPicID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_ENEMY_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(EnemyPicID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ChapterID = reader.ReadSInt32();
        EnemyPicID = reader.ReadSInt32();
    }
}
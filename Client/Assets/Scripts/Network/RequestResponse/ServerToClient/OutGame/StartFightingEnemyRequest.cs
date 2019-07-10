public class StartFightingEnemyRequest : ServerRequestBase
{
    public int LevelID;

    public StartFightingEnemyRequest()
    {
    }

    public StartFightingEnemyRequest(int levelID)
    {
        LevelID = levelID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.START_FIGHTING_ENEMY_REQUEST;
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
public class BeatEnemyRequest : ServerRequestBase
{
    public int StoryPaceID;

    public BeatEnemyRequest()
    {
    }

    public BeatEnemyRequest(int storyPaceID)
    {
        StoryPaceID = storyPaceID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_ENEMY_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(StoryPaceID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        StoryPaceID = reader.ReadSInt32();
    }
}
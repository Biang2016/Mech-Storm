public class MatchStandAloneRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int StoryPaceID;


    public MatchStandAloneRequest()
    {
    }

    public MatchStandAloneRequest(int clientID, int buildID, int storyPaceID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        StoryPaceID = storyPaceID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MATCH_STANDALONE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuildID);
        writer.WriteSInt32(StoryPaceID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        StoryPaceID = reader.ReadSInt32();
    }
}
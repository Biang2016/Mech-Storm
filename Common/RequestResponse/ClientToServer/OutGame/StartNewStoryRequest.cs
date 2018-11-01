public class StartNewStoryRequest : ClientRequestBase
{
    public StartNewStoryRequest()
    {
    }

    public StartNewStoryRequest(int clientID) : base(clientID)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.START_NEW_STORY_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "START_NEW_STORY_REQUEST";
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}
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
}
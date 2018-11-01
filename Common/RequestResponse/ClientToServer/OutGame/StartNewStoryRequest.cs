public class StartNewStoryRequest : ClientRequestBase
{
    public override NetProtocols GetProtocol()
    {
        return NetProtocols.START_NEW_STORY_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "START_NEW_STORY_REQUEST";
    }
}
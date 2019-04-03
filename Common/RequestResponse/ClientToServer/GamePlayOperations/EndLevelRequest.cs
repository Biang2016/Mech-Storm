public class EndChapterRequest : ClientRequestBase
{
    public EndChapterRequest()
    {
    }

    public EndChapterRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_CHAPTER_REQUEST;
    }

}
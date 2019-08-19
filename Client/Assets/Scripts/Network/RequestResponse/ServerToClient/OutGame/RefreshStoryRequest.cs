public class RefreshStoryRequest : ServerRequestBase
{
    public Story Story;

    public RefreshStoryRequest()
    {
    }

    public RefreshStoryRequest(Story story)
    {
        Story = story;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.REFRESH_STORY_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        Story.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        Story = Story.Deserialize(reader);
    }
}
using System.Data;

public class StartNewStoryRequestResponse : ServerRequestBase
{
    public Story Story;

    public StartNewStoryRequestResponse()
    {
    }

    public StartNewStoryRequestResponse(Story story)
    {
        Story = story;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.START_NEW_STORY_REQUEST_RESPONSE;
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
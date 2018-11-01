public class StartNewStoryRequestResponse : ServerRequestBase
{
    public GamePlaySettings GamePlaySettings;
    public BuildInfo DefaultBuildInfo;
    public BuildInfo UnlockedBuildInfo;

    public StartNewStoryRequestResponse()
    {
    }

    public StartNewStoryRequestResponse(GamePlaySettings gamePlaySettings, BuildInfo defaultBuildInfo, BuildInfo unlockedBuildInfo)
    {
        GamePlaySettings = gamePlaySettings;
        DefaultBuildInfo = defaultBuildInfo;
        UnlockedBuildInfo = unlockedBuildInfo;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.START_NEW_STORY_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "START_NEW_STORY_REQUEST_RESPONSE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        GamePlaySettings.Serialize(writer);
        DefaultBuildInfo.Serialize(writer);
        UnlockedBuildInfo.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        GamePlaySettings = GamePlaySettings.Deserialize(reader);
        DefaultBuildInfo = BuildInfo.Deserialize(reader);
        UnlockedBuildInfo = BuildInfo.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [GamePlaySettings]=" + GamePlaySettings.DeserializeLog();
        log += " [DefaultBuildInfo]=" + DefaultBuildInfo.DeserializeLog();
        log += " [UnlockedBuildInfo]=" + UnlockedBuildInfo.DeserializeLog();
        return log;
    }
}
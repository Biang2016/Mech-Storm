using System.Collections.Generic;

public class ClientBuildInfosRequest : ServerRequestBase
{
    public SortedDictionary<int, BuildInfo> OnlineBuildInfos = new SortedDictionary<int, BuildInfo>();
    public GamePlaySettings OnlineGamePlaySettings;

    public bool HasStory;
    public Story Story;

    public ClientBuildInfosRequest()
    {
    }

    public ClientBuildInfosRequest(List<BuildInfo> onlineBuildInfos, GamePlaySettings onlineGamePlaySettings, bool hasStory)
    {
        foreach (BuildInfo bi in onlineBuildInfos)
        {
            OnlineBuildInfos.Add(bi.BuildID, bi);
        }

        OnlineGamePlaySettings = onlineGamePlaySettings;
        HasStory = hasStory;
    }

    public ClientBuildInfosRequest(List<BuildInfo> onlineBuildInfos, GamePlaySettings onlineGamePlaySettings, bool hasStory, Story story)
    {
        foreach (BuildInfo bi in onlineBuildInfos)
        {
            OnlineBuildInfos.Add(bi.BuildID, bi);
        }

        OnlineGamePlaySettings = onlineGamePlaySettings;
        HasStory = hasStory;
        Story = story;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CLIENT_BUILDINFOS_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(OnlineBuildInfos.Count);
        foreach (BuildInfo buildInfo in OnlineBuildInfos.Values)
        {
            buildInfo.Serialize(writer);
        }

        OnlineGamePlaySettings.Serialize(writer);

        writer.WriteByte((byte) (HasStory ? 0x01 : 0x00));

        if (HasStory)
        {
            Story.Serialize(writer);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int count = reader.ReadSInt32();
        OnlineBuildInfos = new SortedDictionary<int, BuildInfo>();
        for (int i = 0; i < count; i++)
        {
            BuildInfo bi = BuildInfo.Deserialize(reader);
            OnlineBuildInfos.Add(bi.BuildID, bi);
        }

        OnlineGamePlaySettings = GamePlaySettings.Deserialize(reader);

        HasStory = reader.ReadByte() == 0x01;
        if (HasStory)
        {
            Story = Story.Deserialize(reader);
        }
    }
}
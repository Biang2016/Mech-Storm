using System.Collections.Generic;

public class ClientBuildInfosRequest : ServerRequestBase
{
    public List<BuildInfo> OnlineBuildInfos;
    public GamePlaySettings OnlineGamePlaySettings;

    public bool HasStory;
    public List<BuildInfo> StoryBuildInfos;
    public GamePlaySettings StoryGamePlaySettings;


    public ClientBuildInfosRequest()
    {
    }

    public ClientBuildInfosRequest(List<BuildInfo> onlineBuildInfos, GamePlaySettings onlineGamePlaySettings, bool hasStory, List<BuildInfo> storyBuildInfos, GamePlaySettings storyGamePlaySettings)
    {
        OnlineBuildInfos = onlineBuildInfos;
        OnlineGamePlaySettings = onlineGamePlaySettings;
        HasStory = hasStory;
        StoryBuildInfos = storyBuildInfos;
        StoryGamePlaySettings = storyGamePlaySettings;
    }

    public ClientBuildInfosRequest(List<BuildInfo> onlineBuildInfos, GamePlaySettings onlineGamePlaySettings, bool hasStory)
    {
        OnlineBuildInfos = onlineBuildInfos;
        OnlineGamePlaySettings = onlineGamePlaySettings;
        HasStory = hasStory;
        StoryBuildInfos = null;
        StoryGamePlaySettings = null;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CLIENT_BUILDINFOS_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "CLIENT_BUILDINFOS_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(OnlineBuildInfos.Count);
        foreach (BuildInfo buildInfo in OnlineBuildInfos)
        {
            buildInfo.Serialize(writer);
        }

        OnlineGamePlaySettings.Serialize(writer);

        writer.WriteByte((byte) (HasStory ? 0x01 : 0x00));

        if (HasStory)
        {
            writer.WriteSInt32(StoryBuildInfos.Count);
            foreach (BuildInfo buildInfo in StoryBuildInfos)
            {
                buildInfo.Serialize(writer);
            }

            StoryGamePlaySettings.Serialize(writer);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        OnlineBuildInfos = new List<BuildInfo>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            BuildInfo bi = BuildInfo.Deserialize(reader);
            OnlineBuildInfos.Add(bi);
        }

        OnlineGamePlaySettings = GamePlaySettings.Deserialize(reader);

        HasStory = reader.ReadByte() == 0x01;
        if (HasStory)
        {
            count = reader.ReadSInt32();
            for (int i = 0; i < count; i++)
            {
                BuildInfo bi = BuildInfo.Deserialize(reader);
                StoryBuildInfos.Add(bi);
            }

            StoryGamePlaySettings = GamePlaySettings.Deserialize(reader);
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [OnlineBuildInfoNum]=" + OnlineBuildInfos.Count;
        foreach (BuildInfo buildInfo in OnlineBuildInfos)
        {
            log += buildInfo.DeserializeLog();
        }

        log += " [OnlineGamePlaySettings]=" + OnlineGamePlaySettings.DeserializeLog();
        log += " [HasStory]=" + HasStory;
        if (HasStory)
        {
            log += " [StoryBuildInfoNum]=" + OnlineBuildInfos.Count;
            foreach (BuildInfo buildInfo in StoryBuildInfos)
            {
                log += buildInfo.DeserializeLog();
            }

            log += " [StoryGamePlaySettings]=" + StoryGamePlaySettings.DeserializeLog();
        }

        return log;
    }
}
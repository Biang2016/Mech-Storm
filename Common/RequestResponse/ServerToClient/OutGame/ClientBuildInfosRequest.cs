using System.Collections.Generic;

public class ClientBuildInfosRequest : ServerRequestBase
{
    public List<BuildInfo> buildInfos;

    public ClientBuildInfosRequest()
    {
    }

    public ClientBuildInfosRequest(List<BuildInfo> buildInfos)
    {
        this.buildInfos = buildInfos;
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
        writer.WriteSInt32(buildInfos.Count);
        foreach (BuildInfo buildInfo in buildInfos)
        {
            buildInfo.Serialize(writer);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildInfos = new List<BuildInfo>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            BuildInfo bi = BuildInfo.Deserialize(reader);
            buildInfos.Add(bi);
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [buildInfosNum]=" + buildInfos.Count;
        foreach (BuildInfo buildInfo in buildInfos)
        {
            log += buildInfo.DeserializeLog();
        }

        return log;
    }
}
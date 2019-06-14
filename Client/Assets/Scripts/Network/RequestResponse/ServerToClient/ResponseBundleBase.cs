using System.Collections.Generic;

/// <summary>
/// 一种附带有多条子Response的响应
/// </summary>
public abstract class ResponseBundleBase : ServerRequestBase
{
    public List<ServerRequestBase> AttachedRequests = new List<ServerRequestBase>();

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttachedRequests.Count);
        foreach (ServerRequestBase serverRequestBase in AttachedRequests)
        {
            serverRequestBase.Serialize(writer);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int attachedRequestsCount = reader.ReadSInt32();
        for (int i = 0; i < attachedRequestsCount; i++)
        {
            ServerRequestBase request = (ServerRequestBase) Common.ProtoManager.TryDeserialize(reader, null);
            AttachedRequests.Add(request);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ClientOperationResponseBase : ServerRequestBase
{
    public List<ServerRequestBase> SideEffects = new List<ServerRequestBase>();

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(SideEffects.Count);
        foreach (ServerRequestBase serverRequestBase in SideEffects)
        {
            serverRequestBase.Serialize(writer);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int sideEffestCount = reader.ReadSInt32();
        for (int i = 0; i < sideEffestCount; i++)
        {
            ServerRequestBase request = (ServerRequestBase)ProtoManager.TryDeserialize(reader, null);
            SideEffects.Add(request);
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [SideEffects] {";
        foreach (var serverRequestBase in SideEffects)
        {
            log += serverRequestBase.DeserializeLog();
        }

        log += "}";
        return log;
    }
}
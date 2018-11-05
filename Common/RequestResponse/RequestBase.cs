﻿using Newtonsoft.Json;

public abstract class RequestBase
{
    public string ProtocolName
    {
        get { return GetProtocol().ToString(); }
    }
    public string CreateAt;
    public int RequestId;
    private static int RequestIdGenerator = 10000;

    private int GenerateRequestId()
    {
        return RequestIdGenerator++;
    }

    protected RequestBase()
    {
        CreateAt = System.DateTime.Now.ToLongTimeString();
        RequestId = GenerateRequestId();
    }

    public abstract NetProtocols GetProtocol();

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) GetProtocol());
        writer.WriteString8(CreateAt);
        writer.WriteSInt32(RequestId);
    }

    public virtual void Deserialize(DataStream reader)
    {
        CreateAt = reader.ReadString8();
        RequestId = reader.ReadSInt32();
    }

    public virtual string DeserializeLog()
    {
        return "[" + ProtocolName + "]:\n" + JsonConvert.SerializeObject(this, Formatting.Indented, Utils.JsonSettings);
    }
}
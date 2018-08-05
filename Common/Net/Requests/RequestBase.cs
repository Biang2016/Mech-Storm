using System.Collections;

public abstract class RequestBase
{
    public string CreateAt;
    public int RequestId;

    protected RequestBase()
    {
        CreateAt = System.DateTime.Now.ToLongTimeString();
    }

    public abstract int GetProtocol();

    public abstract string GetProtocolName();

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32(GetProtocol());
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
        string log = " <" + GetProtocolName() + "> ";
        //log += " [CreateAt]=" + CreateAt;
        log += " [RequestId]=" + RequestId;
        return log;
    }
}
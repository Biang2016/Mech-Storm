using System.Collections;

public abstract class Request
{
    public string CreateAt;

    protected Request()
    {
        CreateAt = System.DateTime.Now.ToLongTimeString();
    }

    public virtual int GetProtocol()
    {
        return 0;
    }

    public virtual string GetProtocolName()
    {
        return "NoName";
    }

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32(GetProtocol());
        writer.WriteString16(CreateAt);
    }

    public virtual void Deserialize(DataStream reader)
    {
        CreateAt = reader.ReadString16();
    }

    public virtual string DeserializeLog()
    {
        string log = " [CreateAt] " + CreateAt;
        return log;
    }
}
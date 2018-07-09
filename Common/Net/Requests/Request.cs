using System.Collections;

public abstract class Request
{
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
    }

    public virtual void Deserialize(DataStream reader){

    }

    public virtual string DeserializeLog()
    {
        return "";
    }
}
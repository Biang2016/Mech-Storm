using UnityEngine;
using System.Collections;

public abstract class Request
{
    public virtual int GetProtocol()
    {
        Debug.LogError("can't get Protocol");
        return 0;
    }

    public virtual string GetProtocolName()
    {
        Debug.LogError("can't get Protocol Name");
        return "NoName";
    }

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32(GetProtocol());
    }

    public virtual string DeserializeLog()
    {
        return "";
    }
}
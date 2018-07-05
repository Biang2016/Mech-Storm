using UnityEngine;
using System.Collections;

public abstract class Response {

	public virtual int GetProtocol(){
        Debug.LogError("can't get Protocol");
		return 0;
	}

    public virtual string GetProtocolName()
    {
        Debug.LogError("can't get Protocol Name");
        return "NoName";
    }

	public virtual void Deserialize(DataStream reader)
	{

	}

    public virtual string DeserializeLog()
    {
        return "";
    }

}

using System.Collections;

public abstract class Response {

	public virtual int GetProtocol(){
        //Debug.LogError("can't get Protocol");
		return 0;
	}

    public abstract string GetProtocolName();

	public virtual void Deserialize(DataStream reader)
	{

	}

    public virtual string DeserializeLog()
    {
        return "";
    }

}

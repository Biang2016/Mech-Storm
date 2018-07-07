using System.Collections;
using System.Collections.Generic;

public class ServerInfoResponse : Response
{
    public int infoNumber;

    public override int GetProtocol()
    {
        return NetProtocols.INFO_NUMBER;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.infoNumber = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[infoNumber]" + infoNumber;
        return log;
    }
}
using System.Collections;
using System.Collections.Generic;

public class ServerInfoRequest : Request
{
    public int infoNumber;

    public ServerInfoRequest(int infoNumber)
    {
        this.infoNumber = infoNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.INFO_NUMBER;
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(infoNumber);
    }
}

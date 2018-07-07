using System.Collections;
using System.Collections.Generic;

public class ServerWarningRequest : Request
{
    public int warningNumber;

    public ServerWarningRequest(int warningNumber)
    {
        this.warningNumber = warningNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.WARNING_NUMBER;
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(warningNumber);
    }
}

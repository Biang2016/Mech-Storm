using System.Collections;
using System.Collections.Generic;

public class ServerWarningResponse : Response
{
    public int warningNumber;

    public override int GetProtocol()
    {
        return NetProtocols.WARNING_NUMBER;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.warningNumber = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[warningNumber]" + warningNumber;
        return log;
    }
}
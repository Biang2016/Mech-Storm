using System.Collections;
using System.Collections.Generic;

public class ClientInfoRequest : Request
{
    public int clientId;
    public int infoNumber;

    public ClientInfoRequest()
    {

    }

    public ClientInfoRequest(int clientId, int infoNumber)
    {
        this.clientId = clientId;
        this.infoNumber = infoNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.INFO_NUMBER;
    }

    public override string GetProtocolName()
    {
        return "INFO_NUMBER";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(infoNumber);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        infoNumber = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [clientId] " + clientId;
        log += " [infoNumber] " + infoNumber;
        return log;
    }
}
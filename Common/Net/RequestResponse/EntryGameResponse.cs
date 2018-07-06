using System.Collections;
using System.Collections.Generic;

public class EntryGameResponse : Response
{
    public int clientId;

    public override int GetProtocol()
    {
        return NetProtocols.ENTRY_GAME;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.clientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        return log;
    }
}
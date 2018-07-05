using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryGameResponse : Response
{
    public long clientId;

    public override int GetProtocol()
    {
        return NetProtocols.ENTRY_GAME;
    }

    public override string GetProtocolName()
    {
        return this.GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = (long) reader.ReadInt64();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        return log;
    }
}
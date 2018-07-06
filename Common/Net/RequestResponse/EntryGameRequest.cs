using System.Collections;
using System.Collections.Generic;

public class EntryGameRequest : Request
{
    public int clientId;

    public override int GetProtocol()
    {
        return NetProtocols.ENTRY_GAME;
    }

    public EntryGameRequest(int clientId)
    {
        this.clientId = clientId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }
}
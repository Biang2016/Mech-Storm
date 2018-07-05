using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryGameRequest : Request
{
    long clientId;

    public override int GetProtocol()
    {
        return NetProtocols.ENTRY_GAME;
    }

    public EntryGameRequest(long clientId)
    {
        this.clientId = clientId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt64(clientId);
    }
}
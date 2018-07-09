using System.Collections;
using System.Collections.Generic;

public class GameStateRequest : Request
{
    public int clientId;
    public enum GameStates
    {
        Matched=0,
        Initialized=1,
        Begin=1,
    }

    public GameStateRequest()
    {
    }

    public GameStateRequest(int clientId)
    {
        this.clientId = clientId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.GAME_BEGIN;
    }

    public override string GetProtocolName()
    {
        return "GAME_BEGIN";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId=reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [clientId] " + clientId;
        return log;
    }
}

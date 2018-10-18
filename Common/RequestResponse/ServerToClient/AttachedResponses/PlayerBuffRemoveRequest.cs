public class PlayerBuffRemoveRequest : ServerRequestBase
{
    public int clientId;
    public int playerBuffId;

    public PlayerBuffRemoveRequest()
    {
    }

    public PlayerBuffRemoveRequest(int clientId, int playerBuffId)
    {
        this.clientId = clientId;
        this.playerBuffId = playerBuffId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_BUFF_REMOVE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_BUFF_REMOVE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(playerBuffId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        playerBuffId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [playerBuffId]=" + playerBuffId;
        return log;
    }
}
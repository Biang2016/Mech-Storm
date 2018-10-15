public class PlayerBuffUpdateRequest : ServerRequestBase
{
    public int clientId;
    public int playerBuffId;
    public int remainTimes;

    public PlayerBuffUpdateRequest()
    {
    }

    public PlayerBuffUpdateRequest(int clientId, int playerBuffId,  int remainTimes)
    {
        this.clientId = clientId;
        this.playerBuffId = playerBuffId;
        this.remainTimes = remainTimes;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_BUFF_UPDATE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_BUFF_UPDATE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(playerBuffId);
        writer.WriteSInt32(remainTimes);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        playerBuffId = reader.ReadSInt32();
        remainTimes = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [playerBuffId]=" + playerBuffId;
        log += " [remainTimes]=" + remainTimes;
        return log;
    }
}
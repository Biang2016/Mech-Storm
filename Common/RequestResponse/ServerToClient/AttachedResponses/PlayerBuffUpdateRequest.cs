public class PlayerBuffUpdateRequest : ServerRequestBase
{
    public int clientId;
    public int buffId;
    public string buffName;
    public int value;

    public PlayerBuffUpdateRequest()
    {
    }

    public PlayerBuffUpdateRequest(int clientId, int buffId, string buffName, int value)
    {
        this.clientId = clientId;
        this.buffId = buffId;
        this.buffName = buffName;
        this.value = value;
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
        writer.WriteSInt32(buffId);
        writer.WriteString8(buffName);
        writer.WriteSInt32(value);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        buffId = reader.ReadSInt32();
        buffName = reader.ReadString8();
        value = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [buffId]=" + buffId;
        log += " [buffName]=" + buffName;
        log += " [value]=" + value;
        return log;
    }
}
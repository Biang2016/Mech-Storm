public class PlayerBuffRemoveRequest : ServerRequestBase
{
    public int clientId;
    public int buffId;
    public string buffName;

    public PlayerBuffRemoveRequest()
    {
    }

    public PlayerBuffRemoveRequest(int clientId, int buffId, string buffName)
    {
        this.clientId = clientId;
        this.buffId = buffId;
        this.buffName = buffName;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_BUFF_REMOVE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(buffId);
        writer.WriteString8(buffName);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        buffId = reader.ReadSInt32();
        buffName = reader.ReadString8();
    }
}
public class PlayerBuffUpdateRequest : ServerRequestBase
{
    public int clientId;
    public int buffId;
    public SideEffectExecute buff;
    public int value;

    public PlayerBuffUpdateRequest()
    {
    }

    public PlayerBuffUpdateRequest(int clientId, int buffId, SideEffectExecute buff, int value)
    {
        this.clientId = clientId;
        this.buffId = buffId;
        this.buff = buff;
        this.value = value;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_BUFF_UPDATE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(buffId);
        buff.Serialize(writer);
        writer.WriteSInt32(value);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        buffId = reader.ReadSInt32();
        buff = SideEffectExecute.Deserialze(reader);
        value = reader.ReadSInt32();
    }

}
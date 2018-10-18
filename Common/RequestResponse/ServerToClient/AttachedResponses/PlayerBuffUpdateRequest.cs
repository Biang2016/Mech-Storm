public class PlayerBuffUpdateRequest : ServerRequestBase
{
    public int clientId;
    public int playerBuffId;
    public int value;
    public int picId;
    public bool hasNumberShow;
    public bool canPiled;
    public bool singleton;

    public PlayerBuffUpdateRequest()
    {
    }

    public PlayerBuffUpdateRequest(int clientId, int playerBuffId, int value, int picId, bool hasNumberShow, bool canPiled, bool singleton)
    {
        this.clientId = clientId;
        this.playerBuffId = playerBuffId;
        this.value = value;
        this.picId = picId;
        this.hasNumberShow = hasNumberShow;
        this.canPiled = canPiled;
        this.singleton = singleton;
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
        writer.WriteSInt32(value);
        writer.WriteSInt32(picId);
        writer.WriteByte((byte) (hasNumberShow ? 0x01 : 0x00));
        writer.WriteByte((byte) (canPiled ? 0x01 : 0x00));
        writer.WriteByte((byte) (singleton ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        playerBuffId = reader.ReadSInt32();
        value = reader.ReadSInt32();
        picId = reader.ReadSInt32();
        hasNumberShow = reader.ReadByte() == 0x01;
        canPiled = reader.ReadByte() == 0x01;
        singleton = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [playerBuffId]=" + playerBuffId;
        log += " [value]=" + value;
        log += " [picId]=" + picId;
        log += " [hasNumberShow]=" + hasNumberShow;
        log += " [canPiled]=" + canPiled;
        log += " [singleton]=" + singleton;
        return log;
    }
}
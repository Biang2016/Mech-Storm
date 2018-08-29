public class SetPlayerRequest : ServerRequestBase
{
    public string username;
    public int clientId;
    public int costLeft;
    public int costMax;
    public int lifeLeft;
    public int lifeMax;
    public int magicLeft;
    public int magicMax;

    public SetPlayerRequest()
    {
    }

    public SetPlayerRequest(string username, int clientId, int costLeft, int costMax, int lifeLeft, int lifeMax, int magicLeft, int magicMax)
    {
        this.username = username;
        this.clientId = clientId;
        this.costLeft = costLeft;
        this.costMax = costMax;
        this.lifeLeft = lifeLeft;
        this.lifeMax = lifeMax;
        this.magicLeft = magicLeft;
        this.magicMax = magicMax;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_SET_PLAYER;
    }

    public override string GetProtocolName()
    {
        return "SE_SET_PLAYER";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString16(username);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(costLeft);
        writer.WriteSInt32(costMax);
        writer.WriteSInt32(lifeLeft);
        writer.WriteSInt32(lifeMax);
        writer.WriteSInt32(magicLeft);
        writer.WriteSInt32(magicMax);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        clientId = reader.ReadSInt32();
        costLeft = reader.ReadSInt32();
        costMax = reader.ReadSInt32();
        lifeLeft = reader.ReadSInt32();
        lifeMax = reader.ReadSInt32();
        magicLeft = reader.ReadSInt32();
        magicMax = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        log += " [clientId]=" + clientId;
        log += " [costLeft]=" + costLeft;
        log += " [costMax]=" + costMax;
        log += " [lifeLeft]=" + lifeLeft;
        log += " [lifeMax]=" + lifeMax;
        log += " [magicLeft]=" + magicLeft;
        log += " [magicMax]=" + magicMax;
        return log;
    }
}
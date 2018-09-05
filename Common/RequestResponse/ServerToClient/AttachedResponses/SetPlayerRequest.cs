public class SetPlayerRequest : ServerRequestBase
{
    public string username;
    public int clientId;
    public int metalLeft;
    public int metalMax;
    public int lifeLeft;
    public int lifeMax;
    public int energyLeft;
    public int energyMax;

    public SetPlayerRequest()
    {
    }

    public SetPlayerRequest(string username, int clientId, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax)
    {
        this.username = username;
        this.clientId = clientId;
        this.metalLeft = metalLeft;
        this.metalMax = metalMax;
        this.lifeLeft = lifeLeft;
        this.lifeMax = lifeMax;
        this.energyLeft = energyLeft;
        this.energyMax = energyMax;
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
        writer.WriteSInt32(metalLeft);
        writer.WriteSInt32(metalMax);
        writer.WriteSInt32(lifeLeft);
        writer.WriteSInt32(lifeMax);
        writer.WriteSInt32(energyLeft);
        writer.WriteSInt32(energyMax);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        clientId = reader.ReadSInt32();
        metalLeft = reader.ReadSInt32();
        metalMax = reader.ReadSInt32();
        lifeLeft = reader.ReadSInt32();
        lifeMax = reader.ReadSInt32();
        energyLeft = reader.ReadSInt32();
        energyMax = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        log += " [clientId]=" + clientId;
        log += " [metalLeft]=" + metalLeft;
        log += " [metalMax]=" + metalMax;
        log += " [lifeLeft]=" + lifeLeft;
        log += " [lifeMax]=" + lifeMax;
        log += " [energyLeft]=" + energyLeft;
        log += " [energyMax]=" + energyMax;
        return log;
    }
}
public class RetinueShieldDefenceRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public int decreaseValue;
    public int shieldValue;

    public RetinueShieldDefenceRequest()
    {
    }

    public RetinueShieldDefenceRequest(int clientId, int retinueId, int decreaseValue, int shieldValue)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.decreaseValue = decreaseValue;
        this.shieldValue = shieldValue;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_SHIELD_DEFENCE;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_SHIELD_DEFENCE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(decreaseValue);
        writer.WriteSInt32(shieldValue);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        decreaseValue = reader.ReadSInt32();
        shieldValue = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [retinueId]=" + retinueId;
        log += " [decreaseValue]=" + decreaseValue;
        log += " [shieldValue]=" + shieldValue;
        return log;
    }
}
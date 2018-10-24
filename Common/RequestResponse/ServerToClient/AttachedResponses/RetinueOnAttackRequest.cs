public class RetinueOnAttackRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public WeaponTypes weaponType;

    public RetinueOnAttackRequest()
    {
    }

    public RetinueOnAttackRequest(int clientId, int retinueId, WeaponTypes weaponType)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.weaponType = weaponType;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_ONATTACK;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_ONATTACK";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32((int) weaponType);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        weaponType = (WeaponTypes) reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [retinueId]=" + retinueId;
        log += " [weaponType]=" + weaponType;
        return log;
    }
}
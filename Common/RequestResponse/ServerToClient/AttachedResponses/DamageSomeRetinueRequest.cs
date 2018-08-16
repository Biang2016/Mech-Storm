public class DamageSomeRetinueRequest : ServerRequestBase
{
    public int beDamagedRetinueClientId;
    public int beDamagedRetinueId;
    public int value;

    public DamageSomeRetinueRequest()
    {
    }

    public DamageSomeRetinueRequest(int beDamagedRetinueClientId, int beDamagedRetinueId,int value)
    {
        this.beDamagedRetinueClientId = beDamagedRetinueClientId;
        this.beDamagedRetinueId = beDamagedRetinueId;
        this.value = value;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_DAMAGE_SOME_RETINUE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_DAMAGE_SOME_RETINUE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(beDamagedRetinueClientId);
        writer.WriteSInt32(beDamagedRetinueId);
        writer.WriteSInt32(value);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        beDamagedRetinueClientId = reader.ReadSInt32();
        beDamagedRetinueId = reader.ReadSInt32();
        value = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [beDamagedRetinueClientId]=" + beDamagedRetinueClientId;
        log += " [beDamagedRetinueId]=" + beDamagedRetinueId;
        log += " [value]=" + value;
        return log;
    }
}
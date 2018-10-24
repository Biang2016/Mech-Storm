public class RetinueCanAttackRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public bool canAttack;

    public RetinueCanAttackRequest()
    {
    }

    public RetinueCanAttackRequest(int clientId, int retinueId, bool canAttack)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.canAttack = canAttack;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_CANATTACK;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_CANATTACK";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteByte((byte) (canAttack ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        canAttack = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [retinueId]=" + retinueId;
        log += " [canAttack]=" + canAttack;
        return log;
    }
}
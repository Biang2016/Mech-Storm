public class RetinueAttackShipRequest : ClientRequestBase
{
    public int AttackRetinueId;

    public RetinueAttackShipRequest()
    {
    }

    public RetinueAttackShipRequest(int clientId, int retinueId) : base(clientId)
    {
        AttackRetinueId = retinueId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.RETINUE_ATTACK_SHIP_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "RETINUE_ATTACK_SHIP_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackRetinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackRetinueId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [AttackRetinueId]=" + AttackRetinueId;
        return log;
    }
}
public class RetinueAttackRetinueRequest : ClientRequestBase
{
    public int AttackRetinueClientId;
    public int AttackRetinueId;
    public int BeAttackedRetinueClientId;
    public int BeAttackedRetinueId;

    public RetinueAttackRetinueRequest()
    {
    }

    public RetinueAttackRetinueRequest(int clientId, int attackRetinueClientId, int retinueId, int beAttackedRetinueClientId, int beAttackedRetinueId) : base(clientId)
    {
        AttackRetinueClientId = attackRetinueClientId;
        AttackRetinueId = retinueId;
        BeAttackedRetinueClientId = beAttackedRetinueClientId;
        BeAttackedRetinueId = beAttackedRetinueId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.RETINUE_ATTACK_RETINUE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "RETINUE_ATTACK_RETINUE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackRetinueClientId);
        writer.WriteSInt32(AttackRetinueId);
        writer.WriteSInt32(BeAttackedRetinueClientId);
        writer.WriteSInt32(BeAttackedRetinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackRetinueClientId = reader.ReadSInt32();
        AttackRetinueId = reader.ReadSInt32();
        BeAttackedRetinueClientId = reader.ReadSInt32();
        BeAttackedRetinueId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [AttackRetinueClientId]=" + AttackRetinueClientId;
        log += " [AttackRetinueId]=" + AttackRetinueId;
        log += " [BeAttackedRetinueClientId]=" + BeAttackedRetinueClientId;
        log += " [BeAttackedRetinueId]=" + BeAttackedRetinueId;
        return log;
    }
}
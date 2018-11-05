public class RetinueAttackRetinueRequest : ClientRequestBase
{
    public int AttackRetinueId;
    public int BeAttackedRetinueClientId;
    public int BeAttackedRetinueId;

    public RetinueAttackRetinueRequest()
    {
    }

    public RetinueAttackRetinueRequest(int clientId, int retinueId, int beAttackedRetinueClientId, int beAttackedRetinueId) : base(clientId)
    {
        AttackRetinueId = retinueId;
        BeAttackedRetinueClientId = beAttackedRetinueClientId;
        BeAttackedRetinueId = beAttackedRetinueId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.RETINUE_ATTACK_RETINUE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackRetinueId);
        writer.WriteSInt32(BeAttackedRetinueClientId);
        writer.WriteSInt32(BeAttackedRetinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackRetinueId = reader.ReadSInt32();
        BeAttackedRetinueClientId = reader.ReadSInt32();
        BeAttackedRetinueId = reader.ReadSInt32();
    }

}
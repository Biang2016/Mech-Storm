public class RetinueAttackRetinueServerRequest : ServerRequestBase
{
    public int AttackRetinueClientId;
    public int AttackRetinueId;
    public int BeAttackedRetinueClientId;
    public int BeAttackedRetinueId;

    public RetinueAttackRetinueServerRequest()
    {
    }

    public RetinueAttackRetinueServerRequest(int attackRetinueClientId, int attackRetinueId, int beAttackedRetinueClientId, int beAttackedRetinueId)
    {
        AttackRetinueClientId = attackRetinueClientId;
        AttackRetinueId = attackRetinueId;
        BeAttackedRetinueClientId = beAttackedRetinueClientId;
        BeAttackedRetinueId = beAttackedRetinueId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST";
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
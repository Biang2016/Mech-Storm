public class RetinueEffectRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public EffectType effectType;


    public RetinueEffectRequest()
    {
    }

    public RetinueEffectRequest(int clientId, int retinueId, EffectType effectType)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.effectType = effectType;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_RETINUE_EFFECT;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_EFFECT";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32((int) effectType);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        effectType = (EffectType) reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [retinueId]=" + retinueId;
        log += " [effectType]=" + effectType;
        return log;
    }

    public enum EffectType
    {
        OnSummon = 0,
        OnDie = 1,
    }
}
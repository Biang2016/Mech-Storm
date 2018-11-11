public class RetinueImmuneStateRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public int immuneRounds; //是否获得免疫效果

    public RetinueImmuneStateRequest()
    {
    }

    public RetinueImmuneStateRequest(int clientId, int retinueId, int immuneRounds)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.immuneRounds = immuneRounds;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_IMMUNE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(immuneRounds);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        immuneRounds = reader.ReadSInt32();
    }
}
public class RetinueDodgeRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;

    public RetinueDodgeRequest()
    {
    }

    public RetinueDodgeRequest(int clientId,  int retinueId)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_DODGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
    }

}
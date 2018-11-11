public class RetinueInactivityStateRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public int inactivityRounds; //是否获得无法行动

    public RetinueInactivityStateRequest()
    {
    }

    public RetinueInactivityStateRequest(int clientId, int retinueId, int inactivityRounds)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.inactivityRounds = inactivityRounds;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_INACTIVITY;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(inactivityRounds);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        inactivityRounds = reader.ReadSInt32();
    }
}
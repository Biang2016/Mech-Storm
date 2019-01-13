public class EndRoundRequest_ResponseBundle : ResponseBundleBase
{
    public int ClientID;

    public EndRoundRequest_ResponseBundle()
    {
    }

    public EndRoundRequest_ResponseBundle(int clientID)
    {
        ClientID = clientID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_ROUND_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ClientID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ClientID = reader.ReadSInt32();
    }
}
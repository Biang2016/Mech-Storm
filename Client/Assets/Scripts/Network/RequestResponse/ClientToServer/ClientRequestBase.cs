public abstract class ClientRequestBase : RequestBase
{
    public int clientId;

    public ClientRequestBase()
    {

    }

    public ClientRequestBase(int clientId)
    {
        this.clientId = clientId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
    }
}
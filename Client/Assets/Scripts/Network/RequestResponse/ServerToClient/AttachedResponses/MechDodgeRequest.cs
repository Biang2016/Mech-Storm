public class MechDodgeRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;

    public MechDodgeRequest()
    {
    }

    public MechDodgeRequest(int clientId,  int mechId)
    {
        this.clientId = clientId;
        this.mechId = mechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_DODGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
    }

}
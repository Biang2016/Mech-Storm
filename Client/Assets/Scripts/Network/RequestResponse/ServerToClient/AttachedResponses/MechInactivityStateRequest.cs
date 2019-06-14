public class MechInactivityStateRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public int inactivityRounds; //是否获得无法行动

    public MechInactivityStateRequest()
    {
    }

    public MechInactivityStateRequest(int clientId, int mechId, int inactivityRounds)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.inactivityRounds = inactivityRounds;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_INACTIVITY;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(inactivityRounds);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        inactivityRounds = reader.ReadSInt32();
    }
}
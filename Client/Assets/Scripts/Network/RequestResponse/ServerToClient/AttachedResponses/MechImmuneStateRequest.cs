public class MechImmuneStateRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public int immuneRounds; //是否获得免疫效果

    public MechImmuneStateRequest()
    {
    }

    public MechImmuneStateRequest(int clientId, int mechId, int immuneRounds)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.immuneRounds = immuneRounds;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_IMMUNE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(immuneRounds);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        immuneRounds = reader.ReadSInt32();
    }
}
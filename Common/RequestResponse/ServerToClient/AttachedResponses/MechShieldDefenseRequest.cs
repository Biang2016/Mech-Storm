public class MechShieldDefenseRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public int decreaseValue;
    public int shieldValue;

    public MechShieldDefenseRequest()
    {
    }

    public MechShieldDefenseRequest(int clientId, int mechId, int decreaseValue, int shieldValue)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.decreaseValue = decreaseValue;
        this.shieldValue = shieldValue;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_SHIELD_DEFENSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(decreaseValue);
        writer.WriteSInt32(shieldValue);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        decreaseValue = reader.ReadSInt32();
        shieldValue = reader.ReadSInt32();
    }
 
}
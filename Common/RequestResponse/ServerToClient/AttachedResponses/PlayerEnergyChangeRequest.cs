public class PlayerEnergyChangeRequest : ServerRequestBase
{
    public int clinetId;
    public int energy_left;
    public int energy_max;
    public bool isOverflow;

    public PlayerEnergyChangeRequest()
    {
    }

    public PlayerEnergyChangeRequest(int clinetId, int energy_left, int energy_max, bool isOverflow = false)
    {
        this.clinetId = clinetId;
        this.energy_left = energy_left;
        this.energy_max = energy_max;
        this.isOverflow = isOverflow;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_ENERGY_CHANGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(energy_left);
        writer.WriteSInt32(energy_max);
        writer.WriteByte((byte) (isOverflow ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        energy_left = reader.ReadSInt32();
        energy_max = reader.ReadSInt32();
        isOverflow = reader.ReadByte() == 0x01;
    }

}
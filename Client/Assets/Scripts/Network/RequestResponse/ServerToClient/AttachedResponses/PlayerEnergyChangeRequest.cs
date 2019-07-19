public class PlayerEnergyChangeRequest : ServerRequestBase
{
    public int ClientID;
    public int Energy_left;
    public int Energy_max;
    public bool IsOverflow;

    public PlayerEnergyChangeRequest()
    {
    }

    public PlayerEnergyChangeRequest(int clientID, int energy_left, int energy_max, bool isOverflow)
    {
        ClientID = clientID;
        Energy_left = energy_left;
        Energy_max = energy_max;
        IsOverflow = isOverflow;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_ENERGY_CHANGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ClientID);
        writer.WriteSInt32(Energy_left);
        writer.WriteSInt32(Energy_max);
        writer.WriteByte((byte) (IsOverflow ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ClientID = reader.ReadSInt32();
        Energy_left = reader.ReadSInt32();
        Energy_max = reader.ReadSInt32();
        IsOverflow = reader.ReadByte() == 0x01;
    }
}
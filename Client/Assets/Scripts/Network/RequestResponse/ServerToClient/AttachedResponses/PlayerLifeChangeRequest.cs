public class PlayerLifeChangeRequest : ServerRequestBase
{
    public int ClientID;
    public int Life_left;
    public int Life_max;
    public bool IsOverflow;

    public PlayerLifeChangeRequest()
    {
    }

    public PlayerLifeChangeRequest(int clientID, int life_left, int life_max, bool isOverflow)
    {
        ClientID = clientID;
        Life_left = life_left;
        Life_max = life_max;
        IsOverflow = isOverflow;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_LIFE_CHANGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ClientID);
        writer.WriteSInt32(Life_left);
        writer.WriteSInt32(Life_max);
        writer.WriteByte((byte) (IsOverflow ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ClientID = reader.ReadSInt32();
        Life_left = reader.ReadSInt32();
        Life_max = reader.ReadSInt32();
        IsOverflow = reader.ReadByte() == 0x01;
    }
}
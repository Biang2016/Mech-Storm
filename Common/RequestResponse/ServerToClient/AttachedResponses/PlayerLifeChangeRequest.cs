public class PlayerLifeChangeRequest : ServerRequestBase
{
    public int clinetId;
    public int life_left;
    public int life_max;

    public PlayerLifeChangeRequest()
    {
    }

    public PlayerLifeChangeRequest(int clinetId, int life_left, int life_max)
    {
        this.clinetId = clinetId;
        this.life_left = life_left;
        this.life_max = life_max;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_LIFE_CHANGE;
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(life_left);
        writer.WriteSInt32(life_max);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        life_left = reader.ReadSInt32();
        life_max = reader.ReadSInt32();
    }

}
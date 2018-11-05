public class PlayerMetalChangeRequest : ServerRequestBase
{
    public int clinetId;
    public int metal_left;
    public int metal_max;

    public PlayerMetalChangeRequest()
    {
    }

    public PlayerMetalChangeRequest(int clinetId, int metal_left, int metal_max)
    {
        this.clinetId = clinetId;
        this.metal_left = metal_left;
        this.metal_max = metal_max;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_METAL_CHANGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(metal_left);
        writer.WriteSInt32(metal_max);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        metal_left = reader.ReadSInt32();
        metal_max = reader.ReadSInt32();
    }

}
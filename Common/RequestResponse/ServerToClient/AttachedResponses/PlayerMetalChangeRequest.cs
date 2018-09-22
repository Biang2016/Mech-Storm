public class PlayerMetalChangeRequest : ServerRequestBase
{
    public int clinetId;
    public MetalChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int addMetal_left;
    public int addMetal_max;

    public PlayerMetalChangeRequest()
    {
    }

    public PlayerMetalChangeRequest(int clinetId, MetalChangeFlag change, int addMetal_left = 0, int addMetal_max = 0)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.addMetal_left = addMetal_left;
        this.addMetal_max = addMetal_max;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_METAL_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_COST_CHANGE";
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteByte((byte) change);
        if (change == MetalChangeFlag.Both)
        {
            writer.WriteSInt32(addMetal_left);
            writer.WriteSInt32(addMetal_max);
        }
        else if (change == MetalChangeFlag.Left)
        {
            writer.WriteSInt32(addMetal_left);
        }
        else if (change == MetalChangeFlag.Max)
        {
            writer.WriteSInt32(addMetal_max);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (MetalChangeFlag) reader.ReadByte();
        if (change == MetalChangeFlag.Both)
        {
            addMetal_left = reader.ReadSInt32();
            addMetal_max = reader.ReadSInt32();
        }
        else if (change == MetalChangeFlag.Left)
        {
            addMetal_left = reader.ReadSInt32();
        }
        else if (change == MetalChangeFlag.Max)
        {
            addMetal_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [change]=" + change;
        log += " [addMetal_left]=" + addMetal_left;
        log += " [addMetal_max]=" + addMetal_max;
        return log;
    }

    public enum MetalChangeFlag
    {
        Both = 0x00,
        Left = 0x01,
        Max = 0x02
    }
}
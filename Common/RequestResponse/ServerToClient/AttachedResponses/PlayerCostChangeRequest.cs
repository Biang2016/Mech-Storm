public class PlayerCostChangeRequest : ServerRequestBase
{
    public int clinetId;
    public CostChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int addCost_left;
    public int addCost_max;

    public PlayerCostChangeRequest()
    {
    }

    public PlayerCostChangeRequest(int clinetId, CostChangeFlag change, int addCost_left = 0, int addCost_max = 0)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.addCost_left = addCost_left;
        this.addCost_max = addCost_max;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_PLAYER_COST_CHANGE;
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
        if (change == CostChangeFlag.Both)
        {
            writer.WriteSInt32(addCost_left);
            writer.WriteSInt32(addCost_max);
        }
        else if (change == CostChangeFlag.Left)
        {
            writer.WriteSInt32(addCost_left);
        }
        else if (change == CostChangeFlag.Max)
        {
            writer.WriteSInt32(addCost_max);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (CostChangeFlag) reader.ReadByte();
        if (change == CostChangeFlag.Both)
        {
            addCost_left = reader.ReadSInt32();
            addCost_max = reader.ReadSInt32();
        }
        else if (change == CostChangeFlag.Left)
        {
            addCost_left = reader.ReadSInt32();
        }
        else if (change == CostChangeFlag.Max)
        {
            addCost_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [change]=" + change;
        log += " [addCost_left]=" + addCost_left;
        log += " [addCost_max]=" + addCost_max;
        return log;
    }

    public enum CostChangeFlag
    {
        Both = 0x00,
        Left = 0x01,
        Max = 0x02
    }
}
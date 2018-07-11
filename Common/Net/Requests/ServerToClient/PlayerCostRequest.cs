using System.Collections;
using System.Collections.Generic;

public class PlayerCostRequest : ServerRequestBase
{
    public int clinetId;
    public CostChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int sign_left; //Sign为+1则为增加，为-1则为消耗
    public int addCost_left;
    public int sign_max; //Sign为+1则为增加，为-1则为消耗
    public int addCost_max;

    public PlayerCostRequest()
    {
    }

    public PlayerCostRequest(int clinetId, CostChangeFlag change, int sign_left, int addCost_left, int sign_max, int addCost_max)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.sign_left = sign_left;
        this.addCost_left = addCost_left;
        this.sign_max = sign_max;
        this.addCost_max = addCost_max;
    }

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER_COST_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "PLAYER_COST_CHANGE";
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteByte((byte) change);
        if (change == CostChangeFlag.Both)
        {
            writer.WriteSInt32(sign_left);
            writer.WriteSInt32(addCost_left);
            writer.WriteSInt32(sign_max);
            writer.WriteSInt32(addCost_max);
        }
        else if (change == CostChangeFlag.Left)
        {
            writer.WriteSInt32(sign_left);
            writer.WriteSInt32(addCost_left);
        }
        else if (change == CostChangeFlag.Max)
        {
            writer.WriteSInt32(sign_max);
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
            sign_left = reader.ReadSInt32();
            addCost_left = reader.ReadSInt32();
            sign_max = reader.ReadSInt32();
            addCost_max = reader.ReadSInt32();
        }
        else if (change == CostChangeFlag.Left)
        {
            sign_left = reader.ReadSInt32();
            addCost_left = reader.ReadSInt32();
        }
        else if (change == CostChangeFlag.Max)
        {
            sign_max = reader.ReadSInt32();
            addCost_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId] " + clinetId;
        log += " [change] " + change;
        log += " [sign_left] " + sign_left;
        log += " [addCost_left] " + addCost_left;
        log += " [sign_max] " + sign_max;
        log += " [addCost_max] " + addCost_max;
        return log;
    }
}

public enum CostChangeFlag
{
    Both = 0x00,
    Left = 0x01,
    Max = 0x02
}
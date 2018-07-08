using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerCostResponse : Response
{
    public int clinetId;
    public CostChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int sign_left; //Sign为+1则为增加，为-1则为消耗
    public int addCost_left;
    public int sign_max; //Sign为+1则为增加，为-1则为消耗
    public int addCost_max;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER_COST_CHANGE;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (CostChangeFlag) (reader.ReadByte());
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
        string log = "";
        log += "[clinetId]" + clinetId;
        log += "[change]" + change;
        log += "[sign_left]" + sign_left;
        log += "[addCost_left]" + addCost_left;
        log += "[sign_max]" + sign_max;
        log += "[addCost_max]" + addCost_max;
        return log;
    }
}
using System.Collections;
using System.Collections.Generic;

public class PlayerCostRequest : Request
{
    public int ClinetId;
    public int Sign; //Sign为+1则为增加，为-1则为消耗
    public int AddCost;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER_COST_CHANGE;
    }

    public PlayerCostRequest(int clinetId, int sign, int addCost)
    {
        ClinetId = clinetId;
        AddCost = addCost;
        Sign = sign;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ClinetId);
        writer.WriteSInt32(Sign);
        writer.WriteSInt32(AddCost);
    }
}
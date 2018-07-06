using System.Collections;
using System.Collections.Generic;

public class PlayerRequest : Request
{
    public int ClinetId;
    public int CostLeft;
    public int CostMax;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER;
    }

    public PlayerRequest(int clinetId, int costLeft, int costMax)
    {
        ClinetId = clinetId;
        CostLeft = costLeft;
        CostMax = costMax;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt64(ClinetId);
        writer.WriteSInt32(CostLeft);
        writer.WriteSInt32(CostMax);
    }
}